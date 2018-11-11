using ColossalFramework;
using UnityEngine;
using static BuildingInfo;
using System.Collections.Generic;

namespace BuildingAnarchy
{
    public class BuildingAnarchy : Singleton<BuildingAnarchy>
    {
        private bool initialized;

        internal bool Initialized => initialized;        

        private string lastBuilding;
        
        internal Dictionary<string, CustomizableProperties> globalBuildingData = new Dictionary<string, CustomizableProperties>();

        internal Dictionary<string, CustomizableProperties> savegameBuildingData = new Dictionary<string, CustomizableProperties>();

        private Dictionary<string, CustomizableProperties> originalBuildingData = new Dictionary<string, CustomizableProperties>();

        private ToolBase Tool => ToolsModifierControl.toolController.CurrentTool;

        internal bool IsBuildingTool => Tool is BuildingTool;

        private bool isToolEnabled;

        private BuildingInfo buildingInfo;

        internal BuildingInfo BuildingInfo => buildingInfo;

        private PlacementMode currentMode;

        private PlacementMode CurrentMode
        {
            get
            {
                if (ChangedSelectedBuilding(buildingInfo.name) || isToolEnabled == false)
                {
                    if (!originalBuildingData.TryGetValue(buildingInfo.name, out CustomizableProperties customProperties))
                    {
                        originalBuildingData.Add(buildingInfo.name, new CustomizableProperties(buildingInfo));
                    }

                    currentMode = Mod.Settings.UseSavegameData && savegameBuildingData.TryGetValue(buildingInfo.name, out CustomizableProperties properties) ? properties.m_placementMode : globalBuildingData.TryGetValue(buildingInfo.name, out properties) ? properties.m_placementMode : originalBuildingData[buildingInfo.name].m_placementMode;

                    lastBuilding = buildingInfo.name;

                    return currentMode;
                }               
                else
                {
                    return buildingInfo.m_placementMode;
                }
            }
        }        

        internal void Initialize()
        {
            if (initialized) return;

            initialized = true;

            CacheOriginalData();            

            LoadCustomData(true);

            ToolBase.cursorInfoLabel.textAlignment = ColossalFramework.UI.UIHorizontalAlignment.Left;
        }

        internal void Release()
        {
            initialized = false;
        }

        private void CacheOriginalData()
        {
            originalBuildingData.Clear();

            for (uint index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); index++)
            {
                BuildingInfo building = PrefabCollection<BuildingInfo>.GetLoaded(index);

                if (building == null || building.name == null) continue;

                if (!originalBuildingData.TryGetValue(building.name, out CustomizableProperties originalProperties))
                {
                    originalBuildingData.Add(building.name, building.GetOriginalProperties());
                }
            }
        }

        private void LoadCustomData(bool update)
        {
            for (uint index = 0; index < PrefabCollection<BuildingInfo>.LoadedCount(); index++)
            {
                BuildingInfo building = PrefabCollection<BuildingInfo>.GetLoaded(index);

                if (building == null || building.name == null) continue;

                var collection = Mod.Settings.UseSavegameData ? savegameBuildingData : globalBuildingData;

                if (collection.TryGetValue(building.name, out CustomizableProperties customProperties))
                {
                    building.SetCustomProperties(customProperties);
                }
            }

            if (update)
            {                
                var buffer = BuildingManager.instance.m_buildings.m_buffer;

                for (ushort bufferIndex = 0; bufferIndex < buffer.Length; bufferIndex++)
                {
                    var building = buffer[bufferIndex];

                    if (building.m_flags == Building.Flags.None) continue;

                    try
                    {
                        building.UpdateBuilding(bufferIndex);

                        BuildingManager.instance.UpdateBuildingRenderer(bufferIndex, true);
                    }
                    catch (System.Exception)
                    {
                                                 
                    }                                    
                }                
            }
        }

        private void SaveBuilding(BuildingInfo building)
        {
            var collection = Mod.Settings.UseSavegameData ? savegameBuildingData : globalBuildingData;

            if (!collection.TryGetValue(building.name, out CustomizableProperties customProperties))
            {
                collection.Add(building.name, new CustomizableProperties(building));
            }
            else collection[building.name] = new CustomizableProperties(building);

            if (!Mod.Settings.UseSavegameData && Mod.Settings.SaveGlobalDataOnDataChanged) Mod.Settings.Save();
        }

        private void Update()
        {
            if (!initialized || ToolsModifierControl.toolController.IsInsideUI) return;

            if (!(Tool is BuildingTool))
            {
                isToolEnabled = false;

                return;
            }
            var buildingTool = Tool as BuildingTool;

            if (buildingTool == null) return;

            buildingInfo = buildingTool.m_prefab;

            if (buildingInfo == null) return;

            if (isToolEnabled == false || ChangedSelectedBuilding(buildingInfo.name))
            {
                buildingInfo.SetPlacementMode(CurrentMode);

                lastBuilding = buildingInfo.name;

                isToolEnabled = true;
            }

            if ((Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.UpArrow)) || (!Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.K)))
            {
                var mode = (int)currentMode + 1 > 7 ? 0 : currentMode + 1;
                
                currentMode = mode;

                buildingInfo.SetPlacementMode(currentMode);

                SaveBuilding(buildingInfo);
            }

            if ((Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.DownArrow)) || (!Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.L)))
            {  
                currentMode = currentMode - 1 < 0 ? (PlacementMode)7 : currentMode - 1;                

                buildingInfo.SetPlacementMode(currentMode);

                SaveBuilding(buildingInfo);
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                buildingInfo.m_flattenTerrain = !buildingInfo.m_flattenTerrain;

                SaveBuilding(buildingInfo);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                buildingInfo.m_fullGravel = !buildingInfo.m_fullGravel;

                SaveBuilding(buildingInfo);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                buildingInfo.m_fullPavement = !buildingInfo.m_fullPavement;

                SaveBuilding(buildingInfo);
            }

            if (Input.GetKey(KeyCode.Home))
            {
                if (originalBuildingData.TryGetValue(buildingInfo.name, out CustomizableProperties properties))
                {
                    buildingInfo.m_placementMode = properties.m_placementMode;
                    buildingInfo.m_flattenTerrain = properties.m_flattenTerrain;
                    buildingInfo.m_fullGravel = properties.m_fullGravel;
                    buildingInfo.m_fullPavement = properties.m_fullPavement;
                }

                SaveBuilding(buildingInfo);
            }
        }

        private bool ChangedSelectedBuilding(string building)
        {
            return lastBuilding != building;
        }

        private string State(bool property)
        {
            return property ? " ON" : "OFF";
        }

        internal string GenerateTooltipText()
        {
            string tooltipText = "<color #87d3ff>";

            if (buildingInfo != null)
            {
                if (Mod.Settings.DisplayPlacementMode) tooltipText += $"Placement Mode: {buildingInfo.m_placementMode}";

                if (Mod.Settings.DisplayFlattenTerrain) tooltipText += $"\nFlatten Terrain: {State(buildingInfo.m_flattenTerrain)}";

                if (Mod.Settings.DisplayFullGravel) tooltipText += $"\nFull Gravel: {State(buildingInfo.m_fullGravel)}";

                if (Mod.Settings.DisplayFullPavement) tooltipText += $"\nFull Pavement: {State(buildingInfo.m_fullPavement)}";                
            }

            tooltipText += "</color>";

            return tooltipText;
        }        
    }
}

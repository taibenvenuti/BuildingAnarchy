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

        private ToolBase tool => ToolsModifierControl.toolController.CurrentTool;

        private bool isToolEnabled;

        private BuildingInfo buildingInfo;        

        private PlacementMode currentMode;

        private PlacementMode CurrentMode
        {
            get
            {
                if (ChangedSelectedBuilding(buildingInfo.name) || isToolEnabled == false)
                {
                    CustomizableProperties customProperties;

                    if (!originalBuildingData.TryGetValue(buildingInfo.name, out customProperties))
                    {
                        originalBuildingData.Add(buildingInfo.name, new CustomizableProperties(buildingInfo));
                    }
                    
                    CustomizableProperties properties;

                    currentMode = Mod.Settings.UseSavegameData  && savegameBuildingData .TryGetValue(buildingInfo.name, out properties) ? properties.m_placementMode : globalBuildingData.TryGetValue(buildingInfo.name, out properties) ? properties.m_placementMode : originalBuildingData[buildingInfo.name].m_placementMode;                                                            

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

                CustomizableProperties originalProperties;

                if (!originalBuildingData.TryGetValue(building.name, out originalProperties))
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

                CustomizableProperties customProperties;

                var collection = Mod.Settings.UseSavegameData ? savegameBuildingData : globalBuildingData;

                if (collection.TryGetValue(building.name, out customProperties))
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
            CustomizableProperties customProperties;

            var collection = Mod.Settings.UseSavegameData ? savegameBuildingData : globalBuildingData;

            if (!collection.TryGetValue(building.name, out customProperties))
            {
                collection.Add(building.name, new CustomizableProperties(building));
            }

            else collection[building.name] = new CustomizableProperties(building);

            if (!Mod.Settings.UseSavegameData && Mod.Settings.SaveGlobalDataOnDataChanged) Mod.Settings.Save();
        }

        private void Update()
        {
            if (!initialized) return;

            if (!(tool is BuildingTool))
            {
                isToolEnabled = false;

                return;
            }
            var buildingTool = tool as BuildingTool;

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
                var mode = (int)currentMode + 1 > 6 ? 0 : currentMode + 1;
                
                currentMode = mode;

                buildingInfo.SetPlacementMode(currentMode);

                SaveBuilding(buildingInfo);
            }

            if ((Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.DownArrow)) || (!Mod.Settings.UseArrowKeys && Input.GetKeyDown(KeyCode.L)))
            {  
                currentMode = currentMode - 1 < 0 ? (PlacementMode)6 : currentMode - 1;                

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
                CustomizableProperties properties;

                if (originalBuildingData.TryGetValue(buildingInfo.name, out properties))
                {
                    buildingInfo.m_placementMode = properties.m_placementMode;
                    buildingInfo.m_flattenTerrain = properties.m_flattenTerrain;
                    buildingInfo.m_fullGravel = properties.m_fullGravel;
                    buildingInfo.m_fullPavement = properties.m_fullPavement;
                    buildingInfo.m_useColorVariations = properties.m_useColorVariations;
                    buildingInfo.m_color0 = properties.m_color0;
                    buildingInfo.m_color1 = properties.m_color1;
                    buildingInfo.m_color2 = properties.m_color2;
                    buildingInfo.m_color3 = properties.m_color3;
                }
            }

            ToolBase.cursorInfoLabel.text += GenerateTooltipText();
        }

        private bool ChangedSelectedBuilding(string building)
        {
            return lastBuilding != building;
        }

        private string State(bool property)
        {
            return property ? " ON" : "OFF";
        }

        private string GenerateTooltipText()
        {
            string tooltipText = "<color #40d0ff>";

            if (Mod.Settings.DisplayPlacementMode) tooltipText += $"\n\nPlacement Mode: " + $"{buildingInfo.m_placementMode}".PadLeft(PlacementMode.ShorelineOrGround.ToString().Length - buildingInfo.m_placementMode.ToString().Length, char.Parse(" "));

            if (Mod.Settings.DisplayFlattenTerrain) tooltipText += $"\nFlatten Terrain: {State(buildingInfo.m_flattenTerrain)}";

            if (Mod.Settings.DisplayFullGravel) tooltipText += $"\nFull Gravel: {State(buildingInfo.m_fullGravel)}";

            if (Mod.Settings.DisplayFullPavement) tooltipText += $"\nFull Pavement: {State(buildingInfo.m_fullPavement)}";

            return tooltipText;
        }        
    }
}
/*
\nPlacement Mode:  ShorelineOrGround    
\nFlatten Terrain:               OFF
\nFull Gravel:                    ON
\nFull Pavement:                  ON
 */

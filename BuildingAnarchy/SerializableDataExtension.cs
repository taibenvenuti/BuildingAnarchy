using ICities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace BuildingAnarchy
{
    public class SerializableDataExtension : SerializableDataExtensionBase
    {
        private BuildingAnarchy Instance => BuildingAnarchy.instance;

        private static readonly string m_dataID = "BUILDING_ANARCHY_DATA";

        private List<BuildingEntry> SavegameBuildingData
        {
            get
            {
                var list = new List<BuildingEntry>();
                if (Instance.savegameBuildingData != null)
                    foreach (var item in Instance.savegameBuildingData)
                        list.Add(item);
                return list;

            }
            set
            {
                var collection = new Dictionary<string, CustomizableProperties>();
                if (value != null)
                    foreach (var item in value)                
                        collection.Add(item.Key, item.Value);
                Instance.savegameBuildingData = collection;                
            }
        }
        

        public override void OnSaveData()
        {
            base.OnSaveData();

            if (!Mod.Settings.UseSavegameData && !Mod.Settings.SaveGlobalDataOnDataChanged) Mod.Settings.Save();

            if (!Mod.Settings.UseSavegameData || Instance.savegameBuildingData == null) return;

            using (var memoryStream = new MemoryStream())
            {
                try
                {
                    var binaryFormatter = new BinaryFormatter();

                    binaryFormatter.Serialize(memoryStream, SavegameBuildingData);

                    serializableDataManager.SaveData(m_dataID, memoryStream.ToArray());
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }
        }

        public override void OnLoadData()
        {
            base.OnLoadData();

            if (!Mod.Settings.UseSavegameData) return;

            var data = serializableDataManager.LoadData(m_dataID);

            if (data == null || data.Length == 0) return;

            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream(data))
            {
                try
                {
                    SavegameBuildingData = binaryFormatter.Deserialize(memoryStream) as List<BuildingEntry>;
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception);
                }
            }            
        }
    }    
}

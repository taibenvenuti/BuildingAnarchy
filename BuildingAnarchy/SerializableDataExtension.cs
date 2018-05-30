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
        private BuildingAnarchy instance => BuildingAnarchy.instance;

        private static readonly string m_dataID = "BUILDING_ANARCHY_DATA";

        private List<BuildingEntry> savegameBuildingData
        {
            get
            {
                var list = new List<BuildingEntry>();
                if (instance.savegameBuildingData != null)
                    foreach (var item in instance.savegameBuildingData)
                        list.Add(item);
                return list;

            }
            set
            {
                var collection = new Dictionary<string, CustomizableProperties>();
                if (value != null)
                    foreach (var item in value)                
                        collection.Add(item.Key, item.Value);
                instance.savegameBuildingData = collection;                
            }
        }
        

        public override void OnSaveData()
        {
            base.OnSaveData();

            if (!Mod.Settings.UseSavegameData && !Mod.Settings.SaveGlobalDataOnDataChanged) Mod.Settings.Save();

            if (!Mod.Settings.UseSavegameData || instance.savegameBuildingData == null) return;

            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(memoryStream, savegameBuildingData);

                serializableDataManager.SaveData(m_dataID, memoryStream.ToArray());
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
                savegameBuildingData = binaryFormatter.Deserialize(memoryStream) as List<BuildingEntry>;
            }            
        }
    }    
}

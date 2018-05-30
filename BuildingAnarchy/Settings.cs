using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace BuildingAnarchy
{
    [XmlRoot("BuildingAnarchySettings")]
    public class Settings
    {
        [XmlIgnore]
        private static readonly string configurationPath = Path.Combine(DataLocation.localApplicationData, "BuildingAnarchy.xml");
        
        public bool UseSavegameData = true;

        public bool SaveGlobalDataOnDataChanged = false;

        public List<BuildingEntry> Entries = new List<BuildingEntry>();        

        public bool DisplayPlacementMode = true;

        public bool DisplayFlattenTerrain = true;

        public bool DisplayFullGravel = true;

        public bool DisplayFullPavement = true;

        public bool UseArrowKeys = false;

        public static string ConfigurationPath
        {
            get
            {
                return configurationPath;
            }
        }

        public Settings() { }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { }

        public void Save()
        {
            
            Entries.Clear();

            foreach (var entry in BuildingAnarchy.instance.globalBuildingData)
                if (entry.Value != null)
                {                
                    Entries.Add(entry);
                }
            
            var fileName = ConfigurationPath;

            var config = Mod.Settings;

            var serializer = new XmlSerializer(typeof(Settings));

            using (var writer = new StreamWriter(fileName))
            {
                config.OnPreSerialize();

                serializer.Serialize(writer, config);
            }
        }


        public static Settings Load()
        {
            var fileName = ConfigurationPath;

            var serializer = new XmlSerializer(typeof(Settings));

            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var config = serializer.Deserialize(reader) as Settings;

                    var collection = BuildingAnarchy.instance.globalBuildingData;

                    collection.Clear();

                    foreach (var entry in config.Entries)
                        if (entry != null)
                        {
                            collection.Add(entry.Key, entry.Value);
                        }

                    return config;
                }
            }
            catch (Exception)
            {
                return new Settings();
            }
        }
    }
}

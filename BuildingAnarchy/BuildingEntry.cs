using System;
using System.Collections.Generic;

namespace BuildingAnarchy
{
    [Serializable]
    public class BuildingEntry
    {
        public string Key;

        public CustomizableProperties Value;

        public BuildingEntry()
        {
        }

        public BuildingEntry(string key, CustomizableProperties value)
        {
            Key = key;
            Value = value;
        }

        public static implicit operator BuildingEntry(KeyValuePair<string, CustomizableProperties> keyValuePair)
        {
            return new BuildingEntry(keyValuePair.Key, keyValuePair.Value);
        }

        public static implicit operator KeyValuePair<string, CustomizableProperties>(BuildingEntry entry)
        {
            return new KeyValuePair<string, CustomizableProperties>(entry.Key, entry.Value);
        }
    }
}

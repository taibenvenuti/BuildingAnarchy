using System;
using static BuildingInfo;

namespace BuildingAnarchy
{
    [Serializable]
    public class CustomizableProperties
    {
        public string m_name;

        public PlacementMode m_placementMode;

        public bool m_flattenTerrain;

        public bool m_fullGravel;

        public bool m_fullPavement;

        public bool m_useColorVariations;

        public SerializableColor m_color0;

        public SerializableColor m_color1;

        public SerializableColor m_color2;

        public SerializableColor m_color3;

        public CustomizableProperties()
        {

        }

        public CustomizableProperties(BuildingInfo building)
        {
            m_name = building.name;

            m_placementMode = building.m_placementMode;

            m_flattenTerrain = building.m_flattenTerrain;

            m_fullGravel = building.m_fullGravel;

            m_fullPavement = building.m_fullPavement;

            m_useColorVariations = building.m_useColorVariations;

            m_color0 = building.m_color0;

            m_color1 = building.m_color1;

            m_color2 = building.m_color2;

            m_color3 = building.m_color3;
        }

        public BuildingEntry GetBuildingEntry()
        {
            return new BuildingEntry(m_name, this);
        }
    }
}

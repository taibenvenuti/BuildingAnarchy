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

        public bool m_useColorVariations;

        public bool m_fullGravel;

        public bool m_fullPavement;

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
        }

        public BuildingEntry GetBuildingEntry()
        {
            return new BuildingEntry(m_name, this);
        }
    }
}

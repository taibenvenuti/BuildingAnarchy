namespace BuildingAnarchy
{
    public static class BuildingInfoExtension
    {
        public static void SetPlacementMode(this BuildingInfo building, BuildingInfo.PlacementMode placementMode)
        {
            building.m_placementMode = placementMode;
        }

        public static CustomizableProperties GetOriginalProperties(this BuildingInfo building)
        {
            return new CustomizableProperties(building);
        }

        public static BuildingInfo GetCustomizedInfo(this BuildingInfo building)
        {
            CustomizableProperties customProperties;
            if (BuildingAnarchy.instance.savegameBuildingData.TryGetValue(building.name, out customProperties))
            {
                building.m_placementMode = customProperties.m_placementMode;
                building.m_fullGravel = customProperties.m_fullGravel;
                building.m_fullPavement = customProperties.m_fullPavement;
                building.m_flattenTerrain = customProperties.m_flattenTerrain;
            }
            return building;        
        }

        public static void SetCustomProperties(this BuildingInfo building, CustomizableProperties customProperties)
        {
            building.m_placementMode = customProperties.m_placementMode;
            building.m_fullGravel = customProperties.m_fullGravel;
            building.m_fullPavement = customProperties.m_fullPavement;
            building.m_flattenTerrain = customProperties.m_flattenTerrain;
        }
    }
}

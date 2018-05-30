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
                building.m_useColorVariations = customProperties.m_useColorVariations;
                building.m_color0 = customProperties.m_color0;
                building.m_color1 = customProperties.m_color1;
                building.m_color2 = customProperties.m_color2;
                building.m_color3 = customProperties.m_color3;
            }
            return building;        
        }

        public static void SetCustomProperties(this BuildingInfo building, CustomizableProperties customProperties)
        {
            building.m_placementMode = customProperties.m_placementMode;
            building.m_fullGravel = customProperties.m_fullGravel;
            building.m_fullPavement = customProperties.m_fullPavement;
            building.m_flattenTerrain = customProperties.m_flattenTerrain;
            building.m_useColorVariations = customProperties.m_useColorVariations;
            building.m_color0 = customProperties.m_color0;
            building.m_color1 = customProperties.m_color1;
            building.m_color2 = customProperties.m_color2;
            building.m_color3 = customProperties.m_color3;
        }

        public static BuildingInfo GetCustomClone(this BuildingInfo building, CustomizableProperties customProperties)
        {
            var clone = UnityEngine.Object.Instantiate(building);
            clone.name = customProperties.m_name;
            clone.m_placementMode = customProperties.m_placementMode;
            clone.m_fullGravel = customProperties.m_fullGravel;
            clone.m_fullPavement = customProperties.m_fullPavement;
            clone.m_flattenTerrain = customProperties.m_flattenTerrain;
            clone.m_useColorVariations = customProperties.m_useColorVariations;
            clone.m_color0 = customProperties.m_color0;
            clone.m_color1 = customProperties.m_color1;
            clone.m_color2 = customProperties.m_color2;
            clone.m_color3 = customProperties.m_color3;
            return clone;
        }
    }
}

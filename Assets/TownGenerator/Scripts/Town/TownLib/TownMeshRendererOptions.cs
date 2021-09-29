using UnityEngine;

namespace Town
{
    [System.Serializable]
    public class TownMeshRendererOptions
    {
        public Transform Root;
        public Transform MapRoot;

        // we include this here since the TownModelOverlays need them
        public GameObject signPrefab;

        public Material BuildingMaterial;
        public Material RoadMaterial;
        public Material WallMaterial;
        public Material TowerMaterial;
        public Material GateMaterial;
        public Material OuterCityGroundMaterial;
        public Material TownMarkerCubeMaterial;
        public Material CityCenterGround;
        public Material WithinWallsGroundMaterial;
        public Material CastleGroundMaterial;
        public Material WaterMaterial;
        public Material PoorArea;
        public Material RichArea;
        public Material FarmArea;

        public Material HiddenCity;
        public Material TownModelsOverlay;

    }
}
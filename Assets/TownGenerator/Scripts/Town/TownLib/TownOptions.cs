using UnityEngine;

namespace Town
{
    [System.Serializable]
    public class TownOptions
    {

        [HideInInspector]
        public bool Overlay = false;
        public bool Walls = true;
        public bool Towers = true;
        [HideInInspector]
        public bool Farm = false;
        [HideInInspector]
        public bool Water = false;
        [HideInInspector]
        public bool Roads = false;

        // [Range (2, 200)]
        [HideInInspector]
        public int Patches = 50;
        public int Seed = 4074;

        [HideInInspector]
        public Den.Tools.Coord coord;

        public Geom.Vector2 mapOffset;
        public Geom.Vector2 townOffset; // { get { return mapOffset.ToTileSizeTownGeomVector2(); }
            //private set {; } }

        public static TownOptions Default => new TownOptions { Patches = 50 };

        [HideInInspector]
        public bool CityDetail = true;

        [HideInInspector]
        public bool IOC = true;
    }
}
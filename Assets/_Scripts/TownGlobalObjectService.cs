using System;
using UnityEngine;
using Den.Tools;
using Twobob;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;


// REF: https://dotnetfiddle.net/6P71ow

[DefaultExecutionOrder(-501)]
public class TownGlobalObjectService : MonoBehaviour
{

    internal static int PatchCap = 50;

    internal static int PatchFloor = 15;

    internal const int QuestSpread = 5;

    internal const float DiagonalRatio = .71f;
    static int initCiySize = 9;
    static int secondCiySize = 30;

    public static float WorldMultiplier = 1;  

    public static List<TownRequest> TownRequests = new List<TownRequest>() {

        new TownRequest("Hometon", new Vector2(0,0), initCiySize)
        ,
       // new TownRequest("Magestown", new Vector2( ReturnNegativeOneToPositiveOne() ,QuestSpread + ReturnNegativeTwoToPositiveTwo()), (int)(secondCiySize))/*  * Give1to2()))  */
       // ,
       // new TownRequest("Farville", new Vector2(ReturnNegativeOneToPositiveOne(),-QuestSpread +ReturnNegativeTwoToPositiveTwo()), (int)(secondCiySize)) /*  * Give1to2())) */
       // ,

       // new TownRequest("Edgerster", new Vector2(-QuestSpread +ReturnNegativeTwoToPositiveTwo(),ReturnNegativeOneToPositiveOne()) , (int)(secondCiySize)) /* * Give1to2())) */
       // ,

       new TownRequest("Strongis", new Vector2(QuestSpread +ReturnNegativeTwoToPositiveTwo(),ReturnNegativeOneToPositiveOne()) , (int)(secondCiySize)) /*  * Give1to2())) */
        

       // new TownRequest("NorthEast", new Vector2( QuestSpread * DiagonalRatio , QuestSpread * DiagonalRatio), (int)(secondCiySize * Give1to2())) //,
       // ,
       // new TownRequest("NorthWest", new Vector2( -QuestSpread * DiagonalRatio , QuestSpread * DiagonalRatio), (int)(secondCiySize * Give1to2())) //,
       // ,
       // new TownRequest("SouthEast", new Vector2( QuestSpread * DiagonalRatio , -QuestSpread * DiagonalRatio), (int)(secondCiySize * Give1to2())) //,
       // ,
       // new TownRequest("SouthWest", new Vector2( -QuestSpread * DiagonalRatio , -QuestSpread * DiagonalRatio), (int)(secondCiySize * Give1to2())) //,
       // ,
       // new TownRequest("North", new Vector2(0,QuestSpread), secondCiySize)
       // ,
       //new TownRequest("South", new Vector2(0,-QuestSpread), secondCiySize)
       // ,
       //new TownRequest("East", new Vector2(QuestSpread,0), secondCiySize)
       // ,
       //new TownRequest("West", new Vector2(-QuestSpread,0), secondCiySize)
    };

    private static float ReturnNegativeOneToPositiveOne()
    {
        return Mathf.Max(1, ReturnNegativeTwoToPositiveTwo() * 0.5f);
    }

    private static float ReturnNegativeTwoToPositiveTwo()
    {
        return ((RandomGen.FlipACoin()) ? -Give1to2() : Give1to2());
    }


    [Range(9, 70)]
    public int MaxCitySpreadreq = 30;

    [Range(8, 69)]
    public int MinCitySpreadreq = 15;

    [Range(1, 30)]
    public float worldMultiplier = 6f;


    public Town.TownOptions options;

    // [Range(500, 500)]
    public static int WorldHeight = 500;



    public static Town.TownMeshRendererOptions rendererOptions;
    public Town.TownMeshRendererOptions LiveRendererOptions;


    public static Queue<string> NamesQueue;

    public static Coord CoordFromVec3(Vector3 vec) { return new Coord((int)vec.x, (int)vec.y); }
    public static Coord CoordForTileFromVec3(Vector3 vec) { return new Coord((int)(vec.x * 0.001f), (int)(vec.y * 0.001f)); }

    public MapMagic.Core.MapMagicObject MapMagicObjectReference;
    public static MapMagic.Core.MapMagicObject MapMagicObjectRef;

    public static RandomName namegenref;

   

    public static List<string> CurrentDistricts;
    public static List<string> RequestedDistricts;

    public RandomName namegen;

    private Vector2 WorldZero;   // 1up
    public static Vector3 WorldStart { get; private set; }   // one player only
    public static Coord WorldHomeCoord { get; private set; }

    public Transform ObjectToTrack;  // etc.

    public TextMeshProUGUI districtNameText;

   
    //[Range(2, 30)]
    //public int CityTileInterstitialSpread = 6;

    public static bool ProduceOverlay = false;



    public GameObject MapIconPrefab;

   // public GameObject HUDPrefab;

   // public GameObject FenceSplineFormer;

    public GameObject SplineMeshFenceMasterSpawner;

    public GameObject SplineMeshWallMasterSpawner;

    public GameObject SplineMeshGatehouseMasterSpawner;




    // this largely just handles the onscreen districts display
    void Update()
    {

        ProduceOverlay = options.Overlay;

        if (MaxCitySpreadreq < MinCitySpreadreq)
            MinCitySpreadreq = MaxCitySpreadreq;

        if (LiveRendererOptions != rendererOptions)
            rendererOptions = LiveRendererOptions;


        WorldMultiplier = worldMultiplier;
        //   TownGlobalObject.CityTileModulo = CityTileInterstitialSpread;
        PatchCap = MaxCitySpreadreq;
        PatchFloor = MinCitySpreadreq;

        if (RequestedDistricts != null)
        {
            if (!CurrentDistricts.SequenceEqual(RequestedDistricts))
            {
                CurrentDistricts.Clear();
                CurrentDistricts.AddRange(RequestedDistricts);

                RequestedDistricts.Clear();
                districtNameText.text = string.Join(" ", CurrentDistricts);
                CurrentDistricts.Clear();
            }
        }



    }

    // We Always do the checks.
    void Awake()
    {
        DoChecks();
    }

    // We Always do the checks.
    void Start()
    {
        // check some stuff
        DoChecks();

        // Hard keep this
        WorldZero = new Vector2(ObjectToTrack.position.x, ObjectToTrack.position.z);
        WorldStart = ObjectToTrack.position;
        WorldHomeCoord = TownGlobalObject.GetIndexAtCoord(CoordForTileFromVec3(ObjectToTrack.position));

        //     WorldHomeCoord = new Coord(0);

    }

    // We Always do the checks.
    void OnEnable()
    {

        DoChecks();

    }



    void DoChecks()
    {
        WorldMultiplier = worldMultiplier;

        initCiySize = MinCitySpreadreq;
        secondCiySize = MaxCitySpreadreq;

        if (FindObjectOfType<MapMagic.Core.MapMagicObject>() == null)
        {
            return;
        }

        ProduceOverlay = options.Overlay;
        //  if (TownGlobalObject.bundles == null || TownGlobalObject.townsData == null)
        //  {
        //   TownGlobalObject.bundles = new Dictionary<Coord, AOTABundle>();
        //   TownGlobalObject.townsData = new Dictionary<Coord, Town.Town>();
        // 
        //  Debug.LogFormat("INITITALISED TownGlobalObject.bundles and .townsdata");
        //   }



        // setup district name
        CurrentDistricts = new List<string>();

        // assign statics
        if (PatchCap != MaxCitySpreadreq)
            PatchCap = MaxCitySpreadreq;

        if (PatchFloor != MinCitySpreadreq)
            PatchFloor = MinCitySpreadreq;

        districtNameText.text = "";

        if (TownGlobalObjectService.namegenref != namegen)
        { TownGlobalObjectService.namegenref = namegen; }

        int allnames = 200;

        if (NamesQueue == null)
        {
            NamesQueue = new Queue<string>(allnames);
        }

        TownGlobalObjectService.NamesQueue.Enqueue("Hometon");

        if (NamesQueue.Count < 200)
        {
            for (int i = 0; i < allnames; i++)
            {
                var tempname = "Hometon";
                while (TownGlobalObjectService.NamesQueue.Contains(tempname))
                {
                    tempname = namegen.GeneratePlace();

                }
                TownGlobalObjectService.NamesQueue.Enqueue(tempname);


            }
        }

        //    Debug.Log(NamesQueue.Peek());

        if (MapMagicObjectRef != MapMagicObjectReference)
            MapMagicObjectRef = MapMagicObjectReference;

        if (LiveRendererOptions != rendererOptions)
            rendererOptions = LiveRendererOptions;


     


    }

    public Coord ObjectToTrackPositionInTileCoords
    {
        get
        {
            return new Vector3(
          ObjectToTrack.position.x * 0.001f,
         ObjectToTrack.position.y * 0.001f,
         ObjectToTrack.position.z * 0.001f).ToCoord();
        }
        private set { }
    }

    public Coord LocalTownCoords
    {
        get { return TownGlobalObject.GetIndexAtCoord(ObjectToTrackPositionInTileCoords); }
        private set { }
    }

    public Coord LocalTown
    {
        get

        {
            Debug.Log(ObjectToTrack.position.ToCoord());
            return TownGlobalObject.GetIndexAtCoord(
          ObjectToTrack.position.ToCoord()
          );
        }
        private set { }
    }


    public static float Give1to2()
    {

        // var ret = 1 + Mathf.Clamp((RandomGen.Next() * (1 / int.MaxValue)), 0, 1);

        var ret = (float)(1 + (RandomGen.Next(10, 0) * 0.1f));

        // Debug.Log(ret);

        return ret;

    }

    public TownGlobalObjectService Instance => GetInstance();

    public static float BigFactor => Give1to2();

    public TownGlobalObjectService GetInstance()
    {
        return this;
    }

}





/// <summary>
/// Don't do this
/// </summary>
public static class TownHolder
{
    private static TownGlobalObjectService instance;



    public static TownGlobalObjectService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Component.FindObjectOfType<TownGlobalObjectService>();
               
            }
            return instance;
        }

        private set { }
       
    }
}
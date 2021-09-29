using Den.Tools;
using MapMagic.Products;
using MapMagic.Terrains;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownTilePreRender : MonoBehaviour
{

    public void Awake()
    {
        //public static Action<TerrainTile, TileData> OnBeforeTileStart;
        //public static Action<TerrainTile, TileData> OnBeforeTilePrepare;
        //public static Action<TerrainTile, TileData, StopToken> OnBeforeTileGenerate;
        //public static Action<TerrainTile, TileData, StopToken> OnTileFinalized; //tile event
        //public static Action<TerrainTile, TileData, StopToken> OnTileApplied;  //TODO: rename to OnTileComplete. OnTileApplied should be called before switching lod
        //public static Action<MapMagicObject> OnAllComplete;
        //public static Action<TerrainTile, bool, bool> OnLodSwitched;
        //public static Action<TileData> OnPreviewAssigned; //preview tile changed
        //public static Action<TerrainTile> OnTileMoved;

        //public static Action<TerrainTile> OnBeforeResetTerrain; //mainly for Lock, to save and return stored data
        //public static Action<TerrainTile> OnAfterResetTerrain;

        TryRegisterHandlers();

    }

    public void OnEnable()
    {
        TryRegisterHandlers();
    }


    public void Start()
    {
        TryRegisterHandlers();
    }


    private void TryRegisterHandlers()
    {
        if (TerrainTile.OnBeforeTileGenerate != OnBeforeTileGenerateCheckFirst)
        {
            TerrainTile.OnBeforeTileGenerate += OnBeforeTileGenerateCheckFirst;
        }

        if (TerrainTile.OnBeforeTilePrepare != OnBeforeTileGenerateCheck)
        {
            TerrainTile.OnBeforeTilePrepare += OnBeforeTileGenerateCheck;
        }

        if (TerrainTile.OnBeforeTileStart != OnBeforeTileGenerateCheck)
        {
            TerrainTile.OnBeforeTileStart += OnBeforeTileGenerateCheck;
        }
    }

    public void OnBeforeTileGenerateCheckFirst(TerrainTile tile, TileData data, StopToken stop)
    {
        OnBeforeTileGenerateCheck( tile,  data);


    }



    private AOTABundle bundle;

    public void OnBeforeTileGenerateCheck(TerrainTile tile, TileData data)
    {
        if (!TownGlobalObject.InitialTownsGenerated)
        {
            Debug.Log("***** !TownGlobalObject.InitialTownsGenerated THIS IS BAD*****");
            return;
        }

        // This will ALLLLLways return something... it SHOULD be the town.
        Coord locality = TownGlobalObject.GetIndexAtCoord(data.area.Coord);


        if (!TownGlobalObject.bundles.ContainsKey(data.area.Coord))  // By this point  !bundle.isBundled && !bundle.isTileDataInBundle  - basically the bundle is a blank.
        {



            // WE DID NOT HIT THE BUNDLE

            //Debug.LogFormat("bundle not found for tile {1}, town {0} during the {2} phase", locality, data.area.Coord,
            //    (data.isDraft) ? "Draft" : "Main"
            //    );



            TownTileRenderer mrMaker = new TownTileRenderer();

            // But the actual town should be stored at the "Locality"  

            bundle = mrMaker.MakeATileBundleWithThisTown(data.area.Coord, false, false, locality);

            // by here all our splineSys and transition lists should be filled.

            // Bless it.

            bundle.MarkisBundledTrue();

            bundle.MarkIsTileDataInBundleTrue();


            TownGlobalObject.bundles[data.area.Coord] = bundle;



        }
        // WE DID HIT THE BUNDLE

        bundle = TownGlobalObject.bundles[data.area.Coord];


        if (locality == data.area.Coord && !bundle.isTileDataInBundle)
        {
            Debug.LogFormat("This is a town tile. Is the locality Town stored? {0}", TownGlobalObject.townsData.ContainsKey(locality));


        }



        // Handle the WE HAVE THE TILE DATA ALREADY case
        if (bundle.isBundled && bundle.isTileDataInBundle)
        {

            return;


        }
        // The Unrendered case. Lets double check.
        else if (bundle.isBundled && !bundle.isTileDataInBundle)
        {

            TownTileRenderer mrMaker = new TownTileRenderer();
            bundle = mrMaker.MakeATileBundle(data.area.Coord, false, false);

            Debug.LogFormat("rewrite for UNRENDERED {0}, {1}, {2} during {3} phase and {4} and {5}",
               string.Format("{0}:{1}", bundle.town.mapOffset.x, bundle.town.mapOffset.y),
                bundle.town.name,
                bundle.name,
                (data.isDraft) ? "DRAFT" : "MAIN",
                 (bundle.isBundled) ? " is bundled :)" : "is not bundled :(",
                    (bundle.isTileDataInBundle) ? " Tile Data IS In Bundle :)" : "TILE DATA MISSING!!!!!!!!!!"
                );


            // by here all our splineSys and transition lists should be filled.

            // Bless it.

            bundle.MarkisBundledTrue();



            TownGlobalObject.bundles[data.area.Coord] = bundle;

            //   var bundletest = TownGlobalObject.GetTownBundle(data.area.Coord);


            var bundletest = TownGlobalObject.bundles[data.area.Coord];

            //  if (bundle2test == bundletest)
            //  {
            Debug.LogFormat(
         " FINAL TEST {0} is null = {1}, {2} is {3}, {4} .isBundled is {5}",
           string.Format("{0}:{1}", data.area.Coord.x, data.area.Coord.z),

         bundletest == null,
         nameof(bundletest),
         bundletest,
         nameof(bundletest),
       (bundletest.isBundled) ? " Tile Data IS In Bundle :)" : "TILE DATA MISSING!!!!!!!!!!"
         );


            //   RenderOutlet(data, bundle, OutletFullList);
            bundle.MarkIsTileDataInBundleTrue();
            //   bundle.isTileDataInBundle = true;
            return;
            //  }

        }
    }
}
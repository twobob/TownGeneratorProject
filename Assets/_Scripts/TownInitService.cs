using Den.Tools;
using ExtensionMethods;
using MeshUtils;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Town;
using UnityEngine;

public static class TownInitValues {
    public static int XzSpread;
    public static int totalCities;

    public static Coord WorldZeroCoord = new Coord((int)(TownInitValues.XzSpread * 0.5), (int)(TownInitValues.XzSpread * 0.5));
}

[DefaultExecutionOrder (-400)]
public class TownInitService : MonoBehaviour
{
    private const float CityRatioGrowthApproximationmultiplier = 5f;
    private int MapMargin = 10;

    [HideInInspector]
    public int MapSpread = 30;
    public static int __extraCities = 10;

    public int ExtraCities = 10;

    [HideInInspector]
    public bool AllowTileMerging = false;

    private int ExpandXZsAfterTries = 300;

    [HideInInspector]
    public bool InitTradeScripts = false;

    // THIS IS MEASURED IN TILES
    [Range(2, 20)]
    public int MinimumDistance = 4;

    private float compressionExpansion = 3.0f;

    public const int MaxTileRender = 4;
    public const int MinTileRender = 2;

    void CreateCities
        (   int XzSpread, 
            int extraCities,
            string HomeTownName = "Hometon")
        {

        int timesThru = 0;

        int layerMask = 1 << LayerMask.NameToLayer("MAPGEN");

        __extraCities = extraCities;

        // We service the town requests first
        for (int i = 0; i < (extraCities + TownGlobalObjectService.TownRequests.Count); i++)
        {
            // The new Town
            Coord newlocality = new Coord();

            // New random choices for the towns coords
            int newXmaybe = 0;
            int newZmaybe = 0;


            int NextValidRandomPatchAmountFromTGOSRange = 
            RandomGen.Next(TownGlobalObjectService.PatchCap, TownGlobalObjectService.PatchFloor);


            // Get A valid amount for the patch choice
            int randomPatches = NextValidRandomPatchAmountFromTGOSRange;

            // For "Distance" comparison
            float closest = 0;

            if (i >= TownGlobalObjectService.TownRequests.Count)
            {
                newXmaybe = (int)(GetRandomXZrangedInt() * 1) + (RandomGen.FlipACoin() ? (int)MinimumDistance : -(int)MinimumDistance);
                newZmaybe = (int)(GetRandomXZrangedInt() * 1) + (RandomGen.FlipACoin() ? (int)MinimumDistance : -(int)MinimumDistance);


                // Test if the hit is closer than we would like, if it is then we need to try again

                timesThru = 0;
                
                while (closest < MinimumDistance + ((RandomGen.FlipACoin())?0:1))
                {

                    if (timesThru >= ExpandXZsAfterTries)
                    {
                        timesThru = 0;
                        Debug.Log("**********BAILED ON COUNT OUT**************");
                        XzSpread++;
                        // closest = MinimumDistance +1;
                        // continue;    
                    }



                    // ADDED +1!
                    newXmaybe += (RandomGen.FlipACoin() ? (int)MinimumDistance  : -(int)MinimumDistance ) ;
                    newZmaybe += (RandomGen.FlipACoin() ? (int)MinimumDistance  : -(int)MinimumDistance ) ;


                    // wraps the modulo regardless of sign
                    //newXmaybe = (newXmaybe < 0) ? newXmaybe = -(Mathf.Abs(newXmaybe) % XzSpread) : newXmaybe % XzSpread;
                    //newZmaybe = (newZmaybe < 0) ? newZmaybe = -(Mathf.Abs(newZmaybe) % XzSpread) : newZmaybe % XzSpread;


                    // find a random coord with a magnitude high enough
                    newlocality = new Coord(newXmaybe, newZmaybe);

                    // convert it to 1000* size and 500 Y
                    Vector3 p1 = newlocality.ToTileSizeCeilVector3();
                    //   Debug.LogFormat("{0}", p1);

                    // get hits
                    //   Collider[] hitColliders = Physics.OverlapSphere(p1, (MinimumDistance * ratio) * 1000f, layerMask);//, LayerMask.NameToLayer("MAPGEN"));

                  ///  Collider[] hitColliders = Physics.OverlapSphere(p1, (MinimumDistance) * 500f, layerMask);//, LayerMask.NameToLayer("MAPGEN"));


                    Coord closetTownFound = TownGlobalObject.GetClosest(newlocality, TownGlobalObject.townsData.Keys.ToList());


                    //Scale the result based on city size.  allow for 1.41  within reason

                    var tinyCityApproxPatchCount = 100;

                    var scalar = (0.002f * CityRatioGrowthApproximationmultiplier * Mathf.Min(TownGlobalObject.townsData[closetTownFound].Patches.Count - tinyCityApproxPatchCount, 10 ));


                    closest =  Coord.Distance(
                        newlocality, 
                            TownGlobalObject.GetClosest(
                                newlocality, 
                                TownGlobalObject.townsData.Keys.ToList()
                                )
                            )
                        - scalar  ;                 //// if we didn't hit at all just place it otherwise check the lengths

                    //if (hitColliders.Length == 0)
                    //{
                    //    // Just skip the next bit and place this directly.
                    //    closest = MinimumDistance + 1;
                    //    //     Debug.LogFormat("{0} is okay as a placement", newlocality);
                    //    break;
                    //} 

                    //foreach (var hitCollider in hitColliders)
                    //{

                    //    var distance = (p1 - hitCollider.gameObject.transform.position).sqrMagnitude * 0.001f;

                    //    if (distance < MinimumDistance)
                    //    {
                    //        closest = distance;
                    //        //   Debug.LogFormat(hitCollider.gameObject, "{0} for {1} from {2} at {3}", closest, hitCollider.gameObject.name, "newlocality", p1);
                    //    }
                    //    //  closest = (distance < MinimumDistance) ? distance : closest;
                    //}
                    ////   closest = TownGlobalObject.GetClosestMagnitude (newlocality , placesWePlaced);

                    timesThru++;

                }

            }
            else  // i < TownGlobalObjectService.TownRequests.Count
            {
                //home North etc...

                    newlocality = new Coord(
                                             (int)TownGlobalObjectService.TownRequests[i].Coord.x,
                                             (int)TownGlobalObjectService.TownRequests[i].Coord.y); // Hometon
               
            }


            TownTileRenderer mrnewlocalityMaker = new TownTileRenderer();


            // 0 triggers a random
            int amount = 0;


            if (i < TownGlobalObjectService.TownRequests.Count)
            {

                if (i == 0)
                {
                    amount += TownHolder.Instance.MinCitySpreadreq;
                }
                else
                {
                    amount += TownGlobalObjectService.TownRequests[i].PatchesInSize;
                }
            }
            // Fill it with Joy
            AOTABundle newlocalityBundle  = mrnewlocalityMaker.MakeATileBundle(newlocality, true, false, amount);



          //  Debug.Log(TownGlobalObjectService.WorldMultiplier);


            //Town.Town newlocalitylazyTown;

            ////Handle the homes case
            //if (i ==0 )
            //{
            //   newlocalitylazyTown = TownGlobalObject.towns.GetOrAdd(newlocality, k => TownGlobalObject.MakeTown(k, k.x, k.z, 0, 60));

            //}
            //else
            //{
            //  newlocalitylazyTown = TownGlobalObject.towns.GetOrAdd(newlocality, k => TownGlobalObject.MakeTown(k, k.x, k.z, 0, randomPatches));

            //}

            //    TownGlobalObject.townsData[locality] = TownGlobalObject.MakeTown(locality, locality.x, locality.z);

            // THIS CONTAINS THE TOWN DATA
            TownGlobalObject.townsData[newlocality] = newlocalityBundle.town;

          
            
            if (i < TownGlobalObjectService.TownRequests.Count)
            {
                TownGlobalObject.townsData[newlocality].name = TownGlobalObjectService.TownRequests[i].Name;
                newlocalityBundle.town.coord = new Coord(
                    (int)TownGlobalObjectService.TownRequests[i].Coord.x, 
                    (int)TownGlobalObjectService.TownRequests[i].Coord.y);
            }

            //handle the home case.
            if (i == 0 && newlocality == new Coord(0))
            {
                TownGlobalObject.townsData[newlocality].name = HomeTownName;
                TownGlobalObject.homeTown = TownGlobalObject.townsData[newlocality];


            }




            //  Debug.Log(TownGlobalObject.townsData[newlocality].name);

            // Bless it.

            // TODO: ADD  METHOD WERE WE CAN PASS A TOWN



            newlocalityBundle.MarkisBundledTrue();

            newlocalityBundle.MarkIsTileDataInBundleTrue();

            TownGlobalObject.bundles[newlocality] = newlocalityBundle;

    

            AOTABundle reffedBundle = TownGlobalObject.bundles[newlocality];

            //Debug.LogFormat(
            //            "INIT TEST: bundle found for tile {5} town {0}: {7} : {3} with {1} SplineSysBundles and {2} TransitionsListBundles and {4} and {6}",
            //           string.Format("{0}:{1}", newlocality.x, newlocality.z),
            //            reffedBundle.SplineSysBundle.Count(),
            //            reffedBundle.TransitionsListBundle.Count(),
            //            reffedBundle.name,
            //            (reffedBundle.isBundled) ? " is bundled :)" : "is not bundled :(",
            //           newlocality,
            //             (reffedBundle.isTileDataInBundle) ? " Tile Data IS In Bundle :)" : "TILE DATA MISSING!!!!!!!!!!",
            //             reffedBundle.town.name
            //            );

            //  in here we could then span every X|Z and prerender every likely tile to have it's data


            float size = CityRatioGrowthApproximationmultiplier * reffedBundle.town.Patches.Count;

          
            // rendering 36 tiles by default at max per city
            int roughTileGuess = (int)  Mathf.Max(MinTileRender,  Mathf.Min(MaxTileRender,         Mathf.Ceil(size * 0.004f)));



            //return the even above, we will split this in half and use that as out "theoretical middle";
            roughTileGuess = (roughTileGuess % 2 == 1) ? roughTileGuess+ 1 : roughTileGuess;


            //    Debug.LogFormat("approximate city size is {0} for {2} so roughly a {1} tile square", size , roughTileGuess, reffedBundle.name);


            // back assign the bundle names from the town for consistency in the editor (manually named and generated cities)

            reffedBundle.name = reffedBundle.town.name;

            // assign the non manually assigned cases
            if (i >= TownGlobalObjectService.TownRequests.Count)
                reffedBundle.town.coord = reffedBundle.coord;


            if (TownGlobalObject.townsData[reffedBundle.town.coord].TownGameObject == null)
                TownGlobalObject.townsData[reffedBundle.town.coord].TownGameObject = new GameObject(reffedBundle.town.name);

            //create or use the holder now it has the right name.
            var go =  TownGlobalObject.townsData[reffedBundle.town.coord].TownGameObject;

            GameObject CityHolder = new GameObject("MAP GEN COLLION HOLDER");

            CityHolder.transform.parent = go.transform;
            CityHolder.transform.localPosition = Vector3.zero;

            CityHolder.transform.position = new Vector3(newlocality.x * 1000, 400, newlocality.z * 1000);


            //GameObject CityHolder = Instantiate<GameObject>(Temp, Temp.transform.position, Quaternion.identity);

            //Debug.LogFormat(CityHolder, "newlocalityBundle is {0} with {1} cells at {2} closest is {3}",
            //      TownGlobalObject.bundles[newlocality].name,
            //      TownGlobalObject.bundles[newlocality].town.Patches.Count,
            //      newlocality,
            //      closest);


            // add something for every city.


            float halfsize = size * 0.01f;

            // Add an item for the HUD
          //  var HUD = Instantiate(TownHolder.Instance.HUDPrefab,CityHolder.transform);
         //  HUD.transform.localPosition = Vector3.zero;


            var   collisionCube = new Cube(reffedBundle.name, 
             TownMeshRendererUtils.GetVertices((int)size, (int)size, -halfsize, -halfsize), halfsize, 
             null, 
             CityHolder.transform, false, false);

           

           collisionCube.Transform.localPosition = Vector3.zero;
          //  collisionCube.GameObject.layer = LayerMask.NameToLayer("MAPGEN");

            BoxCollider col = collisionCube.GameObject.AddComponent<BoxCollider>();
            col.size = new Vector3(size, halfsize, size);

            // Register our BoxCollider for Disabling later.

            TownGlobalObject.renderedBoxColliders.Add(col);

            TownMeshRendererOptions rendererOptions = TownGlobalObjectService.rendererOptions;

            TownOptions skeletonOptions = TownGlobalObject.bundles[newlocality].town.Options;

            skeletonOptions.IOC = false;
            skeletonOptions.Farm = false;
            skeletonOptions.Roads = false;
            skeletonOptions.Walls = false;
            skeletonOptions.CityDetail = false;



                //TownGlobalObject.MeshRenderer = new TownMeshRenderer (
                //        reffedBundle.town, 
                //        skeletonOptions, 
                //        rendererOptions);
                
                //// This does the fancy  world map colored sections
                TownGlobalObject.MeshRenderer.GenerateOverlay();
              
            
            //  Debug.LogFormat("{0} {1} ", reffedBundle.name, reffedBundle.town.name);
         

         
      

            // This does the fancy city overlay over the world map colored sections
                  RenderTownMeshes(ref reffedBundle);




            //   TownGlobalObject.bundles[newlocality].isTileDataInBundle = true;

            //  Destroy(CityHolder);


            // Assign it back?
            TownGlobalObject.SetBundle(newlocality, reffedBundle);

         //   TownGlobalObject.bundles[newlocality] = reffedBundle;


         //   UnityEngine.Assertions.Assert.IsTrue( TownGlobalObject.bundles.ContainsKey(newlocality));




        // COULD HAVE LOOPED HERE
        


        }




        //var myList = TownGlobalObject.townsData.ToList();

        //myList.Sort((pair1, pair2) => pair1.Value.NumPatches.CompareTo(pair2.Value.NumPatches));



        if (true)
        {




            var sortedDict = from entry in TownGlobalObject.townsData orderby entry.Value.Patches.Count descending select entry;







            foreach (var item in sortedDict)
            {

                float size = CityRatioGrowthApproximationmultiplier * item.Value.Patches.Count;


                // rendering 36 tiles by default at max per city
                int roughTileGuess = (int)Mathf.Max(MinTileRender, Mathf.Min(MaxTileRender, Mathf.Ceil(size * 0.003f)));


                //Coord
                //    item.Key

                //town
                //  item.Value



                // so walk over the location from -half the rough guess to +half the rough guess verticall and horizontally as a raster and make the tiles.
                for (int row = 0; row < roughTileGuess; row++)
                {
                    int halfrow;
                    int halfcolumn = halfrow = (int)Mathf.Ceil(roughTileGuess * 0.5f);

                    for (int column = 0; column < roughTileGuess; column++)
                    {

                        Coord nextlocality = new Coord(item.Key.x + (column - halfcolumn), item.Key.z + (row - halfrow));


                        //List<Coord> ExclusionList = new List<Coord>();


                        //foreach (var thisitem in sortedDict)
                        //{
                        //    // Is this exactly the town we are sweeping on this loop
                        //    if (thisitem.Key == item.Key)
                        //        continue;
                        //    ExclusionList.Add(thisitem.Key);
                        //    ExclusionList.Add(new Coord(thisitem.Key.x - 1, thisitem.Key.z - 1));
                        //    ExclusionList.Add(new Coord(thisitem.Key.x - 1, thisitem.Key.z));
                        //    ExclusionList.Add(new Coord(thisitem.Key.x, thisitem.Key.z - 1));
                        //}


                        //if (ExclusionList.Contains(nextlocality))
                        //{
                        //    continue;

                        //}






                        // Don't do the town twice.
                        //    if (item.Key == nextlocality)
                        //    {
                        //        continue;
                        //    }



                        //foreach (var townlocation in TownGlobalObject.townsData.Keys)
                        //{




                        //    // It's not our town and we are right next to it
                        //    //    if (Coord.DistanceSq( townlocation , item.Key ) < 6 && item.Key != nextlocality)
                        //    //{
                        //    //    if (townlocation != item.Key)
                        //    //    {

                        //    //         Debug.LogFormat("Skipped {0} for {1}", item.Key, townlocation);
                        //    //    }
                        //    //    continue;
                        //    //}

                        //}



                        TownTileRenderer nextlocalityMaker = new TownTileRenderer();


                        // We already have a processed tile for this and it's not the town center
                        if (TownGlobalObject.bundles.ContainsKey(nextlocality) && AllowTileMerging)
                        {

                            // We need to create a method that can just "append" to an existing bundles lists  - maybe via reference


                            //  AOTABundle nextlocalityBundle = TownGlobalObject.bundles[nextlocality];

                            AOTABundle nextlocalityBundleReplaced = nextlocalityMaker.MakeATileBundleWithThisTown(nextlocality, false, true, item.Key);


                            TownGlobalObject.bundles[nextlocality] = nextlocalityBundleReplaced;

                            // it's already bundled, and rolled and marked it a B, so no need to put it in the oven ...
                            //nextlocalityBundle.MarkisBundledTrue();
                            //nextlocalityBundle.MarkIsTileDataInBundleTrue();


                        }
                        else
                        {
                            // Just render this first pass

                            AOTABundle nextlocalityBundle;

                            // Fill it with Joy

                            //  if (TownGlobalObject.bundles.ContainsKey(nextlocality))
                            //  {
                            nextlocalityBundle = nextlocalityMaker.MakeATileBundleWithThisTown(nextlocality, false, false, item.Key); //, data.area.active);
                            //  }
                            //  else
                            //  {

                            //      nextlocalityBundle = nextlocalityMaker.MakeATileBundle(nextlocality, false, false);
                            //  }

                            nextlocalityBundle.MarkisBundledTrue();


                            nextlocalityBundle.MarkIsTileDataInBundleTrue();

                            TownGlobalObject.bundles[nextlocality] = nextlocalityBundle;



                            AOTABundle nextReffedBundle = TownGlobalObject.bundles[nextlocality];

                            //Debug.LogFormat(
                            //            "INIT TEST: bundle found for tile {5} town {0}: {7} : {3} with {1} SplineSysBundles and {2} TransitionsListBundles and {4} and {6}",
                            //           string.Format("{0}:{1}", nextlocality.x, nextlocality.z),
                            //            nextReffedBundle.SplineSysBundle.Count(),
                            //            nextReffedBundle.TransitionsListBundle.Count(),
                            //            nextReffedBundle.name,
                            //            (nextReffedBundle.isBundled) ? " is bundled :)" : "is not bundled :(",
                            //           nextlocality,
                            //             (nextReffedBundle.isTileDataInBundle) ? " Tile Data IS In Bundle :)" : "TILE DATA MISSING!!!!!!!!!!",
                            //             nextReffedBundle.town.name
                            //            );

                            //if (!TownGlobalObject.renderedTowns.Contains(nextReffedBundle.coord))

                            //    TownGlobalObject.renderedTowns.Add(nextReffedBundle.coord);

                        }
                    }
                }
            }
        }

        TownGlobalObject.InitialTownsGenerated = true;

   //   if(InitTradeScripts)  InitTrade.GeneratePrefabsForTradersInCitiesStatic();

        // Here was could disable all the MAPGEN colliders if we are going for a fixed city count...

        TownGlobalObject.renderedBoxColliders.ForEach(col => col.enabled = false);

    }

    private void RenderTownMeshes(ref AOTABundle bundle)
    {
        bundle.isTileDataInBundle = true;

        TownGlobalObject.TownsWaitingToRender.Enqueue(new TownMeshRenderer(bundle.town, bundle.town.Options, TownGlobalObjectService.rendererOptions));

    }

    private int GetRandomXZrangedInt()
    {
        return (RandomGen.FlipACoin()) ? -RandomGen.Next(MapSpread - MapMargin) : RandomGen.Next(MapSpread - MapMargin + 1);
    }

    void DoTownInitValuesChecks() {

       // Debug.Log(TownGlobalObjectService.NamesQueue.Count);

        ExpandXZsAfterTries = Mathf.Max(10, ExtraCities * 2);
        TownInitValues.totalCities = ExtraCities;
        MapSpread = (int)(ExtraCities * 0.5f);
        TownInitValues.XzSpread = MapSpread;

        TownGlobalObject.bundles = new Dictionary<Coord, AOTABundle>();
        TownGlobalObject.townsData = new Dictionary<Coord, Town.Town>();

        TownGlobalObject.splinesNodesDataForTile = new ConcurrentDictionary<Coord, List<TypedSpline>>(TownGlobalObject.concurrencyLevel, TownGlobalObject.initialCapacity);
        TownGlobalObject.splinesNodesDataForTileConcrete = new Dictionary<Coord, List<TypedSpline>>();
        TownGlobalObject.isSplinesMeshRenderedOnTile = new Dictionary<Coord, bool>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectOfType<MapMagic.Core.MapMagicObject>() == null)
        {
            return;
        }

        DoTownInitValuesChecks();
        CreateCities(TownInitValues.XzSpread, TownInitValues.totalCities);
    }

}

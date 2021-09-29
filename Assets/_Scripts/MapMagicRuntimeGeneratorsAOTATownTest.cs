using UnityEngine;
using System.Collections.Generic;
using Den.Tools;
using Den.Tools.Splines;
using Den.Tools.GUI;
using Den.Tools.Matrices;
using MapMagic.Products;
using Town;
using MapMagic.Terrains;
using System.Linq;
using Town.Geom;
using System;

namespace MapMagic.Nodes.MatrixGenerators
{

    //TODO Make it honour the town.mapoffset

    [System.Serializable]
    [GeneratorMenu(menu = "Map/Initial", name = "TownGen", iconName = "GeneratorIcons/Constant", disengageable = true,
        helpLink = "https://gitlab.com/denispahunov/mapmagic/-/wikis/MatrixGenerators/Town")]
    public class TownGen200 : Generator, IMultiOutlet // IOutlet<MatrixWorld>
    {
     


        #region outputs and inputs

        //  [Val("Height", "Inlet")]
        // public readonly Inlet<MatrixWorld> heightIn = new Inlet<MatrixWorld>();

        //   [Val("Offset", "Inlet")]
        //  public readonly Inlet<MatrixWorld> OffsetIn = new Inlet<MatrixWorld>();

        // public IEnumerable<IInlet<object>> Inlets() { yield return heightIn; yield return OffsetIn; }

        //private Dictionary<string, Outlet<SplineSys>> OutletSplineSys;
        //private Dictionary<string, Outlet<SplineSys>> OutletMatrixWorld;
        //private Dictionary<string, Outlet<SplineSys>> OutletsTransitionsList;

        //[Val("Height", "Outlet")]
        // public readonly Outlet<MatrixWorld> heightOut = new Outlet<MatrixWorld>();

        //   [Val("FootPrint", "Outlet")]
        //   public readonly Outlet<MatrixWorld> footPrintOut = new Outlet<MatrixWorld>();

        [Val("Roads", "Spline")]
        public readonly Outlet<SplineSys> roadsplineOut = new Outlet<SplineSys>();

        [Val("Streets", "Spline")]
        public readonly Outlet<SplineSys> streetsplineOut = new Outlet<SplineSys>();

        [Val("Districts", "Spline")]
        public readonly Outlet<SplineSys> splineDistrictsOut = new Outlet<SplineSys>();


        [Val("TownWall", "Spline")]
        public readonly Outlet<SplineSys> townWallSplineOut = new Outlet<SplineSys>();

        [Val("Gatehouses", "Spline")]
        public readonly Outlet<SplineSys> gatehouseSplineOut = new Outlet<SplineSys>();

        

       // [Val("TwnWllEdg", "Spline")]
       // public readonly Outlet<SplineSys> townWallEdgeSplineOut = new Outlet<SplineSys>();

       //  [Val("CasWall", "Spline")]
       //  public readonly Outlet<SplineSys> castleWallSplineOut = new Outlet<SplineSys>();

       [Val("CasWllEdg", "Spline")]
        public readonly Outlet<SplineSys> castleWallEdgeSplineOut = new Outlet<SplineSys>();

        [Val("Gates", "Positions")]
        public readonly Outlet<TransitionsList> gatesOut = new Outlet<TransitionsList>();

        [Val("Towers", "Positions")]
        public readonly Outlet<TransitionsList> towersOut = new Outlet<TransitionsList>();


        //[Val("Centroid Poor", "Positions")]
        //public readonly Outlet<TransitionsList> poorOut = new Outlet<TransitionsList>();

        //[Val("Centroid Rich", "Positions")]
        //public readonly Outlet<TransitionsList> richOut = new Outlet<TransitionsList>();

        [Val("Buildings", "Positions")]
        public readonly Outlet<TransitionsList> buildingOut = new Outlet<TransitionsList>();

        [Val("RichBuild", "Positions")]
        public readonly Outlet<TransitionsList> richBuildingOut = new Outlet<TransitionsList>();

        [Val("Castle", "Positions")]
        public readonly Outlet<TransitionsList> castleOut = new Outlet<TransitionsList>();

        [Val("CityCenter", "Positions")]
        public readonly Outlet<TransitionsList> centerOut = new Outlet<TransitionsList>();

        [Val("SingleCenter", "Positions")]
        public readonly Outlet<TransitionsList> singlecenterOut = new Outlet<TransitionsList>();

        [Val("PatchCenters", "Positions")]
        public readonly Outlet<TransitionsList> patchOut = new Outlet<TransitionsList>();




        //  [Val("AreaPos", "Positions")]
        //   public readonly Outlet<TransitionsList> AreasobjsOut = new Outlet<TransitionsList>();

        [Val("OtherPos", "Positions")]
        public readonly Outlet<TransitionsList> otherobjsOut = new Outlet<TransitionsList>();


        [Val("RoadEnd", "Positions")]
        public readonly Outlet<TransitionsList> roadEndOut = new Outlet<TransitionsList>();

        [Val("Terminus", "Positions")]
        public readonly Outlet<TransitionsList> terminusOut = new Outlet<TransitionsList>();

        public IEnumerable<IOutlet<object>> Outlets()
        {
            //yield return heightOut;
            //  yield return footPrintOut;

           // yield return townWallEdgeSplineOut;
            yield return castleWallEdgeSplineOut;
            yield return townWallSplineOut; // yield return castleWallSplineOut;
            yield return gatehouseSplineOut;
            yield return roadsplineOut; yield return streetsplineOut;
            yield return splineDistrictsOut;
            //  yield return poorOut;
            //  yield return richOut; 
            yield return towersOut; yield return gatesOut;
            yield return buildingOut;
            yield return richBuildingOut;
            yield return castleOut; yield return centerOut;
            yield return singlecenterOut;
            yield return patchOut;



            //  yield return AreasobjsOut;
            yield return otherobjsOut;

            yield return roadEndOut;
            yield return terminusOut;

        }


        #endregion

        #region declarations

       
        //[HideInInspector]
        [Val("Seed")]
        public int seed = 12345;
       

        public TownOptions townOptions = new TownOptions();

        //  public enum LevelType { Relative, Absolute }
        //  [Val("Level Type")]
        //  public LevelType levelType = LevelType.Relative;


        // ******************* BUNDLE *************

        [HideInInspector]
      //  [Val("Intensity")]
        public float footprintIntensity = 1;
        [HideInInspector]
      //  [Val("Position")]
        public Vector2D footprintPosition;
        [HideInInspector]
       // [Val("Radius")]
        public float footprintRadius = 30;
        [HideInInspector]
       // [Val("Hardness")]
        public float footprintHardness = 0.5f;

        [Val("JustSplines")]
        public bool RoadOnly = false;

        private AOTABundle bundle;

        #endregion

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(TownGen200));
#endif

        private void InitNamedOutputsDicts(out Dictionary<string, ulong> OutletFullList)
        {


            OutletFullList = new Dictionary<string, ulong>();

            // TODO : I HATE THIS
            // Until we figure out a more direct reference we just make a list ourselves.


            //   Debug.Log("Init The ouput list");
            OutletFullList = new Dictionary<string, ulong>
            {
                { nameof(roadsplineOut), roadsplineOut.Id },
                { nameof(streetsplineOut), streetsplineOut.Id },
                { nameof(splineDistrictsOut), splineDistrictsOut.Id },
                { nameof(townWallSplineOut), townWallSplineOut.Id },
                { nameof(gatehouseSplineOut), gatehouseSplineOut.Id },
               // { nameof(townWallEdgeSplineOut), townWallEdgeSplineOut.Id },
               // { nameof(castleWallSplineOut), castleWallSplineOut.Id },
                { nameof(castleWallEdgeSplineOut), castleWallEdgeSplineOut.Id },

                { nameof(gatesOut), gatesOut.Id },
                { nameof(towersOut), towersOut.Id },
                //OutletFullList.Add(nameof(poorOut), poorOut.Id);
                //OutletFullList.Add(nameof(richOut), richOut.Id);
                { nameof(buildingOut), buildingOut.Id },
                { nameof(richBuildingOut), richBuildingOut.Id },
                { nameof(otherobjsOut), otherobjsOut.Id },
                { nameof(castleOut), castleOut.Id },
                { nameof(centerOut), centerOut.Id },
                { nameof(singlecenterOut), singlecenterOut.Id },
                { nameof(patchOut), patchOut.Id },
                { nameof(roadEndOut), roadEndOut.Id },
                { nameof(terminusOut), terminusOut.Id }

            };

            //      [Val("Gates", "Positions")]
            //public readonly Outlet<TransitionsList> gatesOut = new Outlet<TransitionsList>();

            //[Val("Towers", "Positions")]
            //public readonly Outlet<TransitionsList> towersOut = new Outlet<TransitionsList>();

            //[Val("Centroid Poor", "Positions")]
            //public readonly Outlet<TransitionsList> poorOut = new Outlet<TransitionsList>();

            //[Val("Centroid Rich", "Positions")]
            //public readonly Outlet<TransitionsList> richOut = new Outlet<TransitionsList>();

            //[Val("Centroid Other", "Positions")]
            //public readonly Outlet<TransitionsList> otherOut = new Outlet<TransitionsList>();

            //[Val("Castle", "Positions")]
            //public readonly Outlet<TransitionsList> castleOut = new Outlet<TransitionsList>();

            //[Val("CityCenter", "Positions")]
            //public readonly Outlet<TransitionsList> centerOut = new Outlet<TransitionsList>();





            //[Val("Roads", "Spline")]
            //public readonly Outlet<SplineSys> roadsplineOut = new Outlet<SplineSys>();

            //[Val("Streets", "Spline")]
            //public readonly Outlet<SplineSys> streetsplineOut = new Outlet<SplineSys>();

            //[Val("Districts", "Spline")]
            //public readonly Outlet<SplineSys> splineDistrictsOut = new Outlet<SplineSys>();


            //[Val("TownWall", "Spline")]
            //public readonly Outlet<SplineSys> townWallSplineOut = new Outlet<SplineSys>();

            //[Val("TwnWllEdg", "Spline")]
            //public readonly Outlet<SplineSys> townWallEdgeSplineOut = new Outlet<SplineSys>();

            //[Val("CasWall", "Spline")]
            //public readonly Outlet<SplineSys> castleWallSplineOut = new Outlet<SplineSys>();

            //[Val("CasWllEdg", "Spline")]
            //public readonly Outlet<SplineSys> castleWallEdgeSplineOut = new Outlet<SplineSys>();




        }



        public override void Generate(TileData data, StopToken stop)
        {

            if (stop.stop || !enabled)
            {
                return;
            }

            if (!TownGlobalObject.InitialTownsGenerated)
            {
                Debug.Log("***** !TownGlobalObject.InitialTownsGenerated THIS IS BAD*****");
                return;
            }


            Dictionary<string, ulong> OutletFullList; // = new Dictionary<string, ulong>();

            InitNamedOutputsDicts(out OutletFullList);

            //// Town stuff  /////////// ********

            // This will ALLLLLways return something... it SHOULD be the town.
            Coord locality = TownGlobalObject.GetIndexAtCoord(data.area.Coord);


            if (!TownGlobalObject.bundles.ContainsKey(data.area.Coord))  // By this point  !bundle.isBundled && !bundle.isTileDataInBundle  - basically the bundle is a blank.
            {
                Debug.Log("***** !TownGlobalObject.bundles.ContainsKey(data.area.Coord) THIS IS BAD*****");
                return;

            }
            // WE DID HIT THE BUNDLE

           
               

            bundle = TownGlobalObject.bundles[data.area.Coord];


            if (locality == data.area.Coord && !bundle.isTileDataInBundle  && !RoadOnly)
            {
                Debug.LogFormat("This is a town tile. Is the locality Town stored? {0}", TownGlobalObject.townsData.ContainsKey(locality));


            }


            // Handle the WE HAVE THE TILE DATA ALREADY case
            if (bundle.isBundled && bundle.isTileDataInBundle)
            {

                //Debug.LogFormat(
                //       "{0} is {1}, {2} is {3}, {4} is {5},  ",
                //       "outlet GUID",
                //       item.splineSys.GetType().GUID,
                //       "name",
                //       item.name,
                //       nameof(item.outlet.Id),
                //       item.outlet.id);

                // Write it to the outputs and exit.

                RenderOutlet(data, bundle, OutletFullList);

                return;

           

            }
            // The Unrendered case. Lets double check.
            else if (bundle.isBundled && !bundle.isTileDataInBundle)
            {


                RenderOutlet(data, bundle, OutletFullList);
                bundle.MarkIsTileDataInBundleTrue();
             //   bundle.isTileDataInBundle = true;
                return;
         


            }

       

        }

        private void RenderTownMeshes(ref AOTABundle bundle)
        {
          //  bundle.isTileDataInBundle = true;

        TownGlobalObject.TownsWaitingToRender.Enqueue(new TownMeshRenderer(bundle.town, bundle.town.Options, TownGlobalObjectService.rendererOptions)) ;

        }
     
        private void RenderOutlet(TileData data, AOTABundle bundle, Dictionary<string, ulong> OutletFullList)
        {

          
            ulong matchingId;

            foreach (var splineSys in bundle.SplineSysBundle)
            {
                // Check the list is properly formatted. Could be removed... if you are confident.
                if (!OutletFullList.ContainsKey(splineSys.outletName))
                {
                    Debug.LogFormat("{0} {1} not found initialised in the {2}", nameof(matchingId), splineSys.outletName, nameof(OutletFullList));
                    continue;
                }
                else
                {
                    // item.outletName      // should match the entry in OutletFullList
                    matchingId = OutletFullList[splineSys.outletName];
                }


                if (splineSys.splineSys.lines.Length > 0)
                {
                    //Debug.LogFormat("{6} has {7} lines at tile {8}, first line goes from {0},{1},{2} to {3},{4},{5} ",


                    //     splineSys.splineSys.lines[0].segments[0].start.pos.x,
                    //     splineSys.splineSys.lines[0].segments[0].start.pos.y,
                    //     splineSys.splineSys.lines[0].segments[0].start.pos.z,
                    //     splineSys.splineSys.lines[0].segments[0].end.pos.x,
                    //     splineSys.splineSys.lines[0].segments[0].end.pos.y,
                    //     splineSys.splineSys.lines[0].segments[0].end.pos.z,
                    //     splineSys.outletName,
                    //     splineSys.splineSys.lines.Length,
                    //     data.area.Coord
                    //     );

                    // now we know the Id. go populate it.
                    foreach (var outlet in Outlets())
                    {
                        if (outlet.Id == matchingId)
                        {
                            //    splineSys.splineSys.Clamp((Vector3)data.area.full.worldPos, (Vector3)data.area.full.worldSize);

                            data.StoreProduct(outlet, splineSys.splineSys);
                        }
                    }
                }
                //  else
                //      Debug.LogFormat("Outlet {0} has {1} lines", splineSys.outletName, splineSys.splineSys.lines.Length);
            }

            // We only render the objects in the last gasp. (Main Pass)

            //if (data.isDraft)
            //{
            //    return;
            //}


            

            foreach (var objectList in bundle.TransitionsListBundle)
            {
                if (RoadOnly) continue;

                if (OutletFullList.ContainsKey(objectList.outletName))
                {
                    // item.outletName      // should match the entry in OutletFullList
                    matchingId = OutletFullList[objectList.outletName];
                }
                else
                {
                    Debug.LogFormat("{0} {1} not found initialised in the {2}", nameof(matchingId), objectList.outletName, nameof(OutletFullList));
                    continue;
                }

                if (objectList.transitionsList.count > 0)
                {
                   
                    // now we know the Id. go populate it.
                    foreach (var outlet in Outlets())
                    {
                        if (outlet.Id == matchingId)
                        {
                            // Bodge the city size into the object  // 0,0 is shared between the corner of 4 tiles. so expect at least 4
                            if (objectList.outletName ==  "centerOut")

                            {
                                var hinum = (double) Mathf.Max((TownGlobalObjectService.PatchCap + 10), (TownGlobalObject.bundles[data.area.Coord].town.Options.Patches));
                                var magic = (float) (1/ (hinum / (double)TownGlobalObject.bundles[data.area.Coord].town.Options.Patches));
                              
                                objectList.transitionsList.arr[0].scale = new Vector3(magic, magic, magic);

                                        
                                // Store the single case. for the absolute town center
                                    if (data.area.Coord == TownGlobalObject.GetIndexAtCoord(data.area.Coord))
                                {

                                    var location = Outlets().Where(x => x.Id == singlecenterOut.Id).First();

                                    objectList.transitionsList.arr = objectList.transitionsList.arr.Truncated(objectList.transitionsList.count);

                                    data.StoreProduct(
                                   location,
                                    objectList.transitionsList);
                                }


                            }

                            objectList.transitionsList.arr.Truncated(objectList.transitionsList.count);

                            //  foreach (var item in objectList.transitionsList.arr)
                            //  {
                            //      if (!data.area.active.Contains(item.pos)) continue;
                            //   }

                            TransitionsList cleaner = new TransitionsList();

                            if (objectList.outletName == "gatehouseSplineOut")
                            {
                                Debug.Log(objectList.transitionsList.arr.Length + " gatehouseSplines on this tile");
                            }


                            //for (int i = 0; i < objectList.transitionsList.arr.Length; i++)
                            //    {
                            //        if (data.area.active.Contains(objectList.transitionsList.arr[i].pos))
                            //        {
                            //            cleaner.Add( objectList.transitionsList.arr[i]);
                            //        }                            
                            //    }


                            

                                data.StoreProduct(outlet, objectList.transitionsList);



                           // data.StoreProduct(outlet, cleaner);


                        }
                    }
                }
              //  else
              //      Debug.LogFormat("Outlet {0} has {1} objects", objectList.outletName, objectList.transitionsList.count);
            }



            // Let's shove it all in the list.

           

        }


        //public Rectangle GetBounds(IEnumerable<Patch> patches)
        //{
        //    var vertices = patches.SelectMany(p => p.Shape.Vertices);
        //    var minX = vertices.Min(v => v.x);
        //    var maxX = vertices.Max(v => v.x);
        //    var minY = vertices.Min(v => v.y);
        //    var maxY = vertices.Max(v => v.y);

        //    return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        //}


        //public void GenerateFootprint(TileData data, StopToken stop)
        //{

        //    if (stop != null && stop.stop) return;
        //    MatrixWorld matrix = new MatrixWorld(data.area.full.rect, data.area.full.worldPos, data.area.full.worldSize, data.globals.height);
        //    MatrixWorld copyof = new MatrixWorld(matrix);

        //    float pixelSize = matrix.PixelSize.x;
        //    Vector2D pixelPos = footprintPosition / pixelSize;


        //    //  bundle.town.CityWall.Circumference.AsEnumerable()

        //    //  var circumference = Town.Town.FindCircumference(bundle.town.Patches.Where(p => p.Area.GetType() == typeof(OutsideWallArea))).Vertices; // about 500f


        //    footprintRadius = 1 * GetBounds(bundle.town.Patches.Where(p => p.Area.GetType() == typeof(OutsideWallArea))).Width;
        //    float pixelRadius = footprintRadius / pixelSize;
        //    //   footprintRadius = GetBounds(bundle.town.Patches.Where(p => p.WithinCity)).Width;
        //    //   footprintPosition = new Vector2D((bundle.town.townOffset.x - (footprintRadius * 0.5f)), bundle.town.townOffset.y - (footprintRadius * 0.5f));

        //    footprintPosition = new Vector2D(bundle.town.townOffset.x, bundle.town.townOffset.y);

        //    copyof.Stroke(pixelPos, pixelRadius, footprintHardness, true, 0, .5f);

        //    if (footprintIntensity < 0.999f || footprintIntensity > 1.001f)
        //        copyof.Multiply(footprintIntensity);

        //    // if (stop != null && stop.stop) return;
        //    //    data.StoreProduct(footPrintOut, copyof);

        //}


    }
}
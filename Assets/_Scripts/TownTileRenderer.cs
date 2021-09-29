using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Den.Tools;
using Den.Tools.Splines;
using Den.Tools.GUI;
using Den.Tools.Matrices;
using MapMagic.Core;
using MapMagic.Products;
using Town;
using Twobob;
using System.Linq;
using System.Reflection;
using MapMagic.Nodes;

public class TownTileRenderer 
{
    enum State {yes, noted, no};

    private const string CamMapLayer = "Map";
    private const string CamPlayerLayer = "NotMap";
    private const string CamWorldLayer = "Default";


    // ******************* BUNDLE *************
    private AOTABundle bundle;
    private AOTABundle localitybundle;

    // Lets make a list
    public List<SplineSysWrapper> SplineSysBundle;

    public List<TransitionsListWrapper> TransitionsListBundle;
    
    AOTABundle GetBundle()   {        return bundle;    }

    #region outputs and inputs reference

    //  [Val("Height", "Inlet")]
    // public readonly Inlet<MatrixWorld> heightIn = new Inlet<MatrixWorld>();

    //   [Val("Offset", "Inlet")]
    //  public readonly Inlet<MatrixWorld> OffsetIn = new Inlet<MatrixWorld>();

    // public IEnumerable<IInlet<object>> Inlets() { yield return heightIn; yield return OffsetIn; }


    //[Val("Height", "Outlet")]
    //public readonly Outlet<MatrixWorld> heightOut = new Outlet<MatrixWorld>();

        

    //  [Val("AreaPos", "Positions")]
    //   public readonly Outlet<TransitionsList> AreasobjsOut = new Outlet<TransitionsList>();

    //  [Val("OtherPos", "Positions")]
    // public readonly Outlet<TransitionsList> OtherobjsOut = new Outlet<TransitionsList>();


    //public IEnumerable<IOutlet<object>> Outlets()
    //{
    //    // yield return heightOut;
    //    yield return townWallEdgeSplineOut; yield return castleWallEdgeSplineOut;
    //    yield return townWallSplineOut; yield return castleWallSplineOut;
    //    yield return towersOut; yield return gatesOut;
    //    yield return poorOut;
    //    yield return richOut; yield return otherOut;
    //    yield return castleOut; yield return centerOut;


    //    //  yield return roadsplineOut; yield return streetsplineOut;
    //    yield return splineDistrictsOut;
    //    //  yield return AreasobjsOut;
    //    //  yield return OtherobjsOut; 
    //}

    #endregion


    public AOTABundle MakeATileBundleWithThisTown(Coord data_area_coords, bool CreateInitialCities, bool AppendToExistingBundle, Coord townLoc)
    {

        localitybundle = TownGlobalObject.bundles[townLoc];
    
        return MakeATileBundle(data_area_coords, CreateInitialCities, AppendToExistingBundle);

    }

    // make these easy to use.

    Vector3 tileLocation;
    Vector3 tileSize;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data_area_coords">The Coords in tile space</param>
    /// <param name="stop">MM stop token, required</param>
    /// <param name="isDraft">optional, defaults to false</param>
    public AOTABundle MakeATileBundle(Coord data_area_coords, bool CreateInitialCities, bool AppendToExistingBundle, int Patches =0)
    {
        if (Patches == 0)
            Patches = RandomGen.NextValidRandomPatchAmountFromTGOSRange();
        //    Debug.Log("*** MAYBE * MAKING A TOWN ***********");


        tileLocation = data_area_coords.ToVector3(1000);
        tileSize = new Vector3(1000, 500, 1000);

        // ASSUMPTIONS: HEIGHT IS 500  , TILE WIDTH IS 1000

        // Create a Coord that is in 000.1f * scale - like the tiles are. 
        // and segment it to the width of the "city spacing"

        // This is the local town

      

        Coord locality;


        if (CreateInitialCities) {
          //  Debug.Log("I DID CreateInitialCities");

            locality = data_area_coords;
        }
        else {

         //   Debug.Log("I DID THIS");
            locality = TownGlobalObject.GetIndexAtCoord(data_area_coords); }

        // CREATE A TOWN blank holder
        Town.Town town;

        if (AppendToExistingBundle)


        {

         //   Town.Geom.Vector2 StoredOffset = TownGlobalObject.bundles[data_area_coords].town.mapOffset;


           Debug.LogFormat("localitybundle coord is {0} for {1}", localitybundle.coord, localitybundle.name);


         //   float tempX = StoredOffset.x;
         //   float tempY = StoredOffset.y;

         //   float scaledX = tempX * 0.001f;
         //   float scaledY = tempY * 0.001f;

         ////   Debug.LogFormat("scaledX {0} : scaledY {1} for {2}:{3}   with {4}:{5}", scaledX, scaledY, StoredOffset.x, StoredOffset.y, tempX, tempY);

         //   Coord OffsetTown = new Coord (
         //      ( data_area_coords.x - (int)scaledX ),
         //       ( data_area_coords.z - (int)scaledY )
         //       );

            

         //   Debug.LogFormat("offset town is {0} for {1} from {2} when AppendToExistingBundle", OffsetTown, data_area_coords, StoredOffset);

         //   localitybundle = TownGlobalObject.bundles[OffsetTown];


            town = localitybundle.town;

            SplineSysBundle = localitybundle.SplineSysBundle.ToList<SplineSysWrapper>();
            TransitionsListBundle = localitybundle.TransitionsListBundle.ToList<TransitionsListWrapper>();

        }
        else
        {
            // Create some lists for our wrapper

            SplineSysBundle = new List<SplineSysWrapper>();
            TransitionsListBundle = new List<TransitionsListWrapper>();
        }
      

       


        //// Home town stuff  /////////// ********
        
        // get town from locality it should exist. 
        if (TownGlobalObject.bundles.ContainsKey(locality) && !AppendToExistingBundle)
        {

            //if (CreateInitialCities)
            //{
            //    Debug.Log("****************************THIS SHOULD HAPPEN********************************************************");
            //}
            //if (!CreateInitialCities)
            //{
            //    Debug.Log("****************************WHAT THE F.....********************************************************");
            //}


            localitybundle = TownGlobalObject.bundles[locality];

        }
        else if (!AppendToExistingBundle)
        {

         //  if not... make one. make a bundle for the locality, attach a town to it.
       
         

            if (!CreateInitialCities)
            {
                Debug.Log("****************************THIS SHOULD NEVER HAPPEN********************************************************");
            }  

          localitybundle = new AOTABundle(locality);




            var lazyTown = TownGlobalObject.towns.GetOrAdd(locality, k => TownGlobalObject.MakeTown(k, k.x, k.z, TownHolder.Instance.options.Seed, Patches) );

           var concrete = lazyTown;

          
                    TownGlobalObject.townsData[locality] = concrete;
               
        

            //    Debug.LogFormat("created locality bundle, Is the town null? {0}", localitybundle.town == null);




            //    Debug.LogFormat("stored HARD locality bundle, Is the town null? {0}", TownGlobalObject.townsData[locality] == null);
            //     Debug.LogFormat("stored locality bundle, Is the town null? {0}", TownGlobalObject.towns[locality].town == null);

            // By here it should be valid.




        }






        // we are not currenly 'at' the above town.
        if (!IsCoordLocality(data_area_coords) && !AppendToExistingBundle)
        {
            // we make a bundle to process
            bundle = new AOTABundle(data_area_coords);
            //      Debug.Log("making data_area_coords bundle");

        }
        else 
        {

            // We  Are at a town. and this May be our first time here

            bundle = localitybundle;
          //  Debug.LogFormat("using locality bundle, {0} for {2}:{3} and CreateInitialCities is {1}", bundle.town.name, CreateInitialCities, data_area_coords.x, data_area_coords.z);
        }
       


        if (!AppendToExistingBundle) bundle.MarkIsTileDataInBundleTrue();

        // By now we have two bundles that should have two sets of data in them
        /*====================================================================================
         * If we are At a town - and the bundle already exists:
         * we should not be here? 
         * but you know. missing data. teleports. future proofing - whatnot.
         * 
         * Or we are at a town and the bundle does not exist (see above)
         * 
         * bundle == localitybundle     and the Town is attached to the locality. (so both)
         * ====================================================================================
         * If we are NOT at a town and the localitybundle Town also did not exist:
         * 
         * bundle = a new bundle, as does the locality. in all cases the town data is hung from the locality.
         * we process the bundle ONLY - only ensuring to elide any town data references in 'bundle' IF it's not a town
         * 
         *  SO BY HERE WE HAVE A 'bundle' that IS going to get processed - and a Town.        * 
         *  
         *  the town does NOT contain the right offset for this TILE. Use THIS tiles offsets - from THAT town to make the slices.
         *  
         */



        town = localitybundle.town;


        // We need to override the town options on the Geometry retrival pass possibly 
        // and Honour the townOffsetX townOffsetZ so as to get a valid tile location.

        TownOptions Options =   town.Options;

        // opt.mapOffset = new Town.Geom.Vector2(index.x * (1000), index.z * 1000);
        //  opt.townOffset = new Town.Geom.Vector2(townOffsetX * (1000), townOffsetZ * 1000);

        // get a vector as an offset from town


        //TODO WORKING HERE *********************************************************

        // TODO FIX THIS
        //Options.townOffset = (

        //    locality.ToVector2(1) -
        //    data_area_coords.ToVector2(1)


        //    ).ToGeomVector2();

        //   Debug.LogFormat("The offset for {0} is {1} on tile {2}", locality, Options.townOffset, data_area_coords);


      if (!AppendToExistingBundle)   Options.townOffset = Options.mapOffset = (data_area_coords.ToVector2(1) - locality.ToVector2(1)).ToGeomVector2();        //    = Options.townOffset;



        //  Town.Geom.Vector2 WorldOffset = town.townOffset.ToTileSizeTownGeomVector2();

        Town.Geom.Vector2 WorldOffset = town.mapOffset;

        // Coord test = locality;// - town.Options.coord;

        // Coord test = data_area_coords.ToVector2(1).ToGeomVector2().ToCoord();

        // Here we setup a rect that we use to test if the tile contains a thing. 
        // (pretty sure denis does this some other way like data_area_coords.active.area.Contains()) but w/e

        CoordRect newOffset = MakeCollisionRectWithOffset(data_area_coords);

        // use our amended options

        // Grab a copy of the geom to walk
        TownGeometry geom = town.GetTownGeometry(Options);

        // setup and handle the onscreen UI of district names
        #region dictrict UI            

        // Create a holder for the simple display UI

        string[] district = { town.name };

        if (TownGlobalObjectService.RequestedDistricts == null)
        {
            TownGlobalObjectService.RequestedDistricts = new List<string>();
        }

        // TODO FIX THIS NAME DISPLAY THING. This is just a placeholder to show it being updated on the main thread.

        // only add it if it doesn't exist to the TownGlobalObjectService.RequestedDistricts to get it shown on the screen in main thread

        if (!TownGlobalObjectService.RequestedDistricts.Contains(town.name))
            TownGlobalObjectService.RequestedDistricts.Add(town.name);

        // Thats enough for that.

        #endregion

        List<Line> SharedSplineLineList;
        Line SharedSplineLine;
        Line[] SharedSplineLineArray;

        #region export Road and street spline


        Wrapper_Spline_Roads(town, WorldOffset, newOffset, out SharedSplineLineList, out SharedSplineLine, out SharedSplineLineArray, AppendToExistingBundle);



        // ADDED TO THE BUNDLE #############


        Wrapper_Spline_Streets(town, WorldOffset, newOffset, out SharedSplineLineList, out SharedSplineLine, out SharedSplineLineArray, AppendToExistingBundle);

        #endregion

        //optimization
        //foreach (Spline spline in splineSys.splines)
        //{
        //    spline.Relax(0.5f, (int)1);
        //    spline.Optimize(1 * 0.5f);
        //}

        // ############## DISTRICT EDGES
        #region district edges
        if (town.Patches.Count > 0)
        {
            Wrapper_Spline_DistrictPatchEdges(town, WorldOffset, newOffset, AppendToExistingBundle);

        }

        #endregion

        #region extended debug

        // CREATE DEBUG FOR THE EXTRA PROPERTIES


        // TODO: MAKE THIS DATA INTO A CLICKABLE MENU FOR EACH TOWN TO HIGHLIGHT AREAS OF THE OVERLAY

        //if (false)
        //{

        //    var estimatedPopulation = (int)(/*_avgPopulationPrBuilding */ 10 * geom.Buildings.Count);

        //    var values = Enum.GetValues(typeof(BuildingType));

        //    List<BuildingStatsAttribute> allBuildings = new List<BuildingStatsAttribute>();

        //    foreach (var value in values)
        //    {
        //        var buildingType = typeof(BuildingType).GetField(value.ToString()).GetCustomAttribute<BuildingStatsAttribute>();
        //        allBuildings.Add(buildingType);
        //    }


        //    // Empty , Home , Shoemakers , Furriers , Maidservants , Tailors , Barbers , Healer , Jewelers , Old-Clothes , Taverns/Restaurants , Masons , Pastrycooks , Shrine , Carpenters , Weavers , Barrel maker (cooper) , Chandlers , Textile trader (mercer) , Bakers , Scabbardmakers , Watercarriers , Wine-Sellers , Hatmakers , Chicken Butchers , Saddlers , Pursemakers , Butchers , Fishmongers , Beer-Sellers , Buckle Makers , Plasterers , Spice Merchants , Blacksmiths , Painters , Doctors , Roofers , Bathers , Locksmiths , Ropemakers , Copyists , Harness-Makers , Inns , Rugmakers , Sculptors , Tanners , Bleachers , Cutlers , Hay Merchants , Glovemakers , Woodcarvers , Woodsellers , Magic-Shops , Bookbinders , Illuminators , Temple , Booksellers , University

        //    var allBuildingTypes = string.Join(" , ", allBuildings.GroupBy(test => test.Description)
        //   .Select(grp => grp.First().Description));


        //    // 1 , 10 , 150 , 250 , 350 , 400 , 500 , 550 , 600 , 700 , 800 , 850 , 900 , 950 , 1000 , 1100 , 1200 , 1400 , 1500 , 1700 , 1800 , 1900 , 2000 , 2100 , 2300 , 2400 , 2800 , 3000 , 3900 , 6000 , 6300 , 10000

        //    var allBuildingPops = string.Join(" | ", allBuildings.GroupBy(test => test.Population)
        //  .Select(grp => grp.First().Population));


        //    var allBuildingMins = string.Join(" , ", allBuildings.GroupBy(test => test.MinSize)
        //  .Select(grp => grp.First().MinSize));


        //    Debug.Log(allBuildingTypes);
        //    Debug.Log(allBuildingPops);
        //    Debug.Log(allBuildingMins);


        //}

        #endregion


        // ###  
        // TODO - THIS DOES NOTHING 
        //  SplineSys TownWallEdges is INACTIVE

        #region Town walls

        //if (town.CityWall.Circumference.Count> 0)
        //{
        //    Wrapper_Spline_TownWalls(WorldOffset, town.CityWall, newOffset);
        //}
        #endregion

        // TO DO   wrap this in an output for objects (Towers / Gates)
        // ########### CASTLE WALL EDGEs  && TOWN WALLS

        #region Castle walls  geom.Walls and Towers
        if (geom.Walls.Count() > 0)
        {
            Wrapper_Spline_CastleWalls_CastleWallTowers(WorldOffset, newOffset, geom, town.CityWall);

        }

        #endregion

        //// OBJECT stuff  /////////// ********


        #region castle locations

        TransitionsList castleLocOut = new TransitionsList();
       

        foreach (var item in geom.Overlay.Where(x => x.HasCastle))
        {
              
            Town.Geom.Vector2 size = new Town.Geom.Vector2((float)(item.Center.x  * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,
                                   (float)(item.Center.y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y);

            if (newOffset.Contains(size.ToCoord()))
            {


                castleLocOut.Add(
                           new Transition(
                               size.x,
                               size.y));
            }
        }
        // ADD TO THE BUNDLE #############
        //        [Val("Castle", "Positions")]
        //public readonly Outlet<TransitionsList> castleOut = new Outlet<TransitionsList>();

        if (castleLocOut.count > 0)
        {

            castleLocOut.RemoveAnyBlanksInTransitionListArray();


            TransitionsListBundle.Add(new TransitionsListWrapper(
                   nameof(castleLocOut), castleLocOut, "castleOut"
                   ));
        }
        #endregion


        //// center stuff  /////////// ********

        #region Center Of Town location

        CoordRect largeroffset = new CoordRect(newOffset.offset, newOffset.size * 2);


        if (largeroffset.Contains(WorldOffset.ToCoord()  ))
        {


            TransitionsList centerLocOut = new TransitionsList();

            centerLocOut.Add(new Transition((float)town.Center.x + WorldOffset.x, (float)town.Center.y + WorldOffset.y));



            // ADD TO THE BUNDLE #############

            //      [Val("CityCenter", "Positions")]
            //public readonly Outlet<TransitionsList> centerOut = new Outlet<TransitionsList>();


            TransitionsListBundle.Add(new TransitionsListWrapper(
                   nameof(centerLocOut), centerLocOut, "centerOut"
                   ));

        }
        // ADDED TO THE BUNDLE #############

        //   Debug.Log("Begin StoreProduct RawAdjustedheightOut");
        //   data.StoreProduct(centerOut, centerLocOut);


        #endregion





        #region Center of every building

        //    Debug.Log("Begin dstObjs");
      

        List<Building> onsite = town.GetTownGeometry(town.Options).Buildings;

        TransitionsList buildingLocOut = new TransitionsList();
        TransitionsList richBuildingLocOut = new TransitionsList();

        //  Debug.Log(onsite.Count+" buildings");

      

        var richArea = geom.Overlay.Where(x => x.Area.ToString() == "Town.RichArea");

      
        int totalRichBuildings = 0;
        int totalOtherBuildings = 0;
        
        
        
        
        
        foreach (var building in onsite)
        {
           
            var place = new Coord(
                (int)((building.Shape.Center.x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x),// town.mapOffset.x), 
                (int)((building.Shape.Center.y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y));//   town.mapOffset.y));
          

            if (newOffset.Contains(place))
            {
               

                bool match = false;
                foreach (var item in richArea)  
                    {

                 

                    var rectangle = item.Shape.GetBoundingBox()  ;
                    var placeascoord = building.Shape.Center.ToCoord();
                    CoordRect testrect = new CoordRect(new Rect(new Vector2(rectangle.Left, rectangle.Bottom), new Vector2(rectangle.Width, rectangle.Height)));

                    if (testrect.Contains(placeascoord))
                    {
                        match = true;
                      
                        //break;
                        goto exit;
                    }

                } 
                
exit:                

                if (match)
                {
                    richBuildingLocOut.Add(new Transition(place.x, place.z));
                    totalRichBuildings += 1;
                }
                else
                {
                    buildingLocOut.Add(new Transition(place.x, place.z));
                    totalOtherBuildings += 1;
                }
            }

            
            //  Debug.LogFormat("{0} accepted for {1} with {2} | {3} | {4}", place, WorldOffset, bundle.town.townOffset, town.townOffset, newOffset);
            // place.ClampByRect(new CoordRect(new Coord((int) town.mapOffset.x, (int)town.mapOffset.y), new Coord(1000)));



        }

        //foreach (var item in geom.Overlay.Where( x => x.Area.ToString() ==    ))
        //{

        //}









       
        TransitionsListBundle.Add(new TransitionsListWrapper(
          nameof(buildingLocOut),  buildingLocOut, "buildingOut"
           ));

        richBuildingLocOut.RemoveAnyBlanksInTransitionListArray();

        TransitionsListBundle.Add(new TransitionsListWrapper(
         nameof(richBuildingLocOut), richBuildingLocOut, "richBuildingOut"
          ));

        #endregion


        //// center of every patch stuff  /////////// ********

        #region Center of every patch

        //    Debug.Log("Begin dstObjs");
        TransitionsList patchLocOut = new TransitionsList();

        if (true)
        // if (!isDraft)
        {
            foreach (var item in town.Patches)
            {
                patchLocOut.Add(new Transition(
                    item.Center.x + WorldOffset.x,
                    item.Center.y + WorldOffset.y));
            }
        }

        // ADD TO THE BUNDLE #############

        patchLocOut.RemoveAnyBlanksInTransitionListArray();

       
        TransitionsListBundle.Add(new TransitionsListWrapper(
        nameof( patchLocOut),  patchLocOut, "patchOut"
           ));

        #endregion


        // #####  TOWN WALLS

        #region town.CityWall.Circumference as splines

    //    Wrapper_Spline_TownWallsCircumference(town, WorldOffset);

        // ADDED TO THE BUNDLE #############


        //  data.StoreProduct(townWallSplineOut, townWallSplinesSys);


        #endregion


        //  ########### CASTLE WALL SPLINES
        #region town.Castle.Patch.Edges CIRCUMFERENCE AS WIDTH as splines

  //      Wrapper_Spline_CastleWallCicumference(town, WorldOffset, newOffset);

        // ADDED TO THE BUNDLE #############

        /// data.StoreProduct(castleWallSplineOut, castleWallSplinesSys);


        #endregion


        //// OBJECTS  stuff  /////////// ********

        #region  'OtherObjects' random object 
        //    Debug.Log("Begin dstObjs");
      //  TransitionsList otherobjsOut = new TransitionsList(town.Castle.Wall.Circumference.Count);

        TransitionsList otherobjsOut = new TransitionsList();


        foreach (var item in town.Market.Edges)
        {

            Town.Geom.Vector2 middle = item.A - item.B;

            otherobjsOut.Add(new Transition(
                (middle.x * TownGlobalObjectService.WorldMultiplier)
                + WorldOffset.x,
                (middle.y * TownGlobalObjectService.WorldMultiplier) +
                WorldOffset.y));
        }

        // ADD TO THE BUNDLE #############

        //      [Val("Centroid Other", "Positions")]
        //public readonly Outlet<TransitionsList> otherOut = new Outlet<TransitionsList>();

        otherobjsOut.RemoveAnyBlanksInTransitionListArray();

        TransitionsListBundle.Add(new TransitionsListWrapper(
               nameof(otherobjsOut), otherobjsOut, "otherobjsOut"
               ));

        // ADDED TO THE BUNDLE #############



        //   data.StoreProduct(otherOut, OtherobjsOut);

        #endregion



        //   if (stop != null && stop.stop) return;



        // HERE WE PROBABLY NEED TO FILL AND RETURN A BUNDLE



        bundle.SplineSysBundle = SplineSysBundle.ToArray<SplineSysWrapper>();
        bundle.TransitionsListBundle = TransitionsListBundle.ToArray<TransitionsListWrapper>();

        if (!IsCoordLocality(data_area_coords))
        {
            // Debug.Log("Not a town!!");
            //bundle.town = null;
        }


        bundle.name = town.name;

        // ensure these are PER TILE. NOT PER TOWN.
        bundle.coord = data_area_coords;


        // BUNDLE SHOULD BE READY
        // ADDED TO THE BUNDLE #############


        return bundle;


    }

    private void Wrapper_Spline_CastleWalls_CastleWallTowers(Town.Geom.Vector2 WorldOffset, CoordRect newOffset, TownGeometry geom, Wall passedwall)
    {
        SplineSys CastleWallEdges = new SplineSys();

        List<Line> lineList = new List<Line>();

        var replacedGates = new List<Town.Geom.Vector2>();

        TransitionsList towerLocOut = new TransitionsList();
        TransitionsList gateLocOut = new TransitionsList();

        

        var startingGates = new List<Town.Geom.Vector2>(geom.Gates.Count + passedwall.Gates.Count);// geom.Gates;

        geom.Gates.ForEach(x => startingGates.Add( new Town.Geom.Vector2(x.x,x.y)));

        passedwall.Gates.ForEach(x => startingGates.Add(new Town.Geom.Vector2(x.x, x.y)));

        // geom.Gates.CopyTo(startingGates);

        List<Vector3> gatehouseNodes = new List<Vector3>();

        List<Den.Tools.Splines.Line> gatehouseLines = new List<Line>();

        State matched = State.no;

        // This is the castle walls.
        for (int i = 0; i < geom.Walls.Count; i++)
        {
            //discard the first - castle diameter - wall.
            if (i == 0)
            {
                continue;
            }

            Town.Geom.Edge wall = geom.Walls[i];
            var start = wall.A;
            var end = wall.B;

            if (geom.Gates.Contains(start))
            {
                matched = State.yes;
                replacedGates.Add(start);
                start = start + Town.Geom.Vector2.Scale(end - start, 0.3f);
                wall.A = start;
                geom.Gates.Add(start);

                var tot = new Town.Geom.Vector2(start.x * TownGlobalObjectService.WorldMultiplier + WorldOffset.x,
                    start.y * TownGlobalObjectService.WorldMultiplier + WorldOffset.y);

                gatehouseNodes.Add(new Vector3(tot.x, 499.9f, tot.y));
            }

            if (geom.Gates.Contains(end) || matched == State.yes  )
            {
               
                    matched = State.no;
                    
                    if(geom.Gates.Contains(end))
                    replacedGates.Add(end);

                    end = end - Town.Geom.Vector2.Scale(end - start, 0.3f);
                    wall.B = end;
                    
               if (geom.Gates.Contains(end))
                    geom.Gates.Add(end);



                    var tot = new Town.Geom.Vector2(
                                                     end.x * TownGlobalObjectService.WorldMultiplier + WorldOffset.x,
                                                     end.y * TownGlobalObjectService.WorldMultiplier + WorldOffset.y);

                    var test = new Vector3(tot.x, 499.9f, tot.y);

                    // if it is close enough to the last node we make a pair. if it isn't we delete the last node too. and dont fail on weird edge cases
                    if (gatehouseNodes.Count == 0)
                    {
                        continue;
                    }
                    if (Vector3.SqrMagnitude(gatehouseNodes[gatehouseNodes.Count - 1] - test) < 40000)
                    {

                        gatehouseLines.Add(new Line(gatehouseNodes[gatehouseNodes.Count - 1], test));
                        gatehouseNodes.Clear();

                    }
                    else
                    {
                        // clear for a non match
                        gatehouseNodes.Clear();
                    }

               

            }



            //if (geom.Gates.Contains(end))
            //{
            //    replacedGates.Add(end);
            //    end = end - Town.Geom.Vector2.Scale(end - start, 0.3f);
            //    wall.B = end;
            //    geom.Gates.Add(end);
            //}

            // add a line here

            //    Town.Geom.Vector2 WorldOffset = town.townOffset;

            Vector3 vecA = new Vector3(
                        ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset),
                        TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset));
            Vector3 vecB = new Vector3(
             ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset),
                        TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset));

            //    CoordRect newOffset = MakeCollisionRectWithOffset(WorldOffset);


            // maybe this filter will work?


          //  if (TwoVectorsToTileSpaceBothIsTruncated(ref vecA, ref vecB, newOffset))

                lineList.Add(new Line(vecA, vecB));
            

            //// NOW HANDLING TOWER TRANSITIONS




            var towA = ((start.x + ((end.x - start.x) * 0.5f)) * TownGlobalObjectService.WorldMultiplier) +WorldOffset.x;

            var towB =
                ((start.y + ((end.y - start.y) * 0.5f)) * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y;

            var towLoc = new Coord((int)towA, (int)towB);

            //  towLoc.  ClampByRect(new CoordRect(new Coord((int)WorldOffset.x, (int)WorldOffset.y), new Coord(1000)));

            // Vector3 testVecStart = new Vector3(towA. x, 250f, start.y);
            //  Vector3 testVecEnd = new Vector3(end.x, 250f, end.y);

            if (newOffset.Contains(towLoc))
            {
                if (! ( towA == 0 && towB ==0 ))
                towerLocOut.Add(
                     new Transition(towA, towB)
                    );
            }
       
        }

        if (lineList.Count() > 0)
        {
            CastleWallEdges.lines = lineList.ToArray<Line>();
        }
        // CastleWallEdges.lines = temp.ToArray<Line>();

        // ADD TO THE BUNDLE #############

        //     RelaxOptimiseClampLineArray(CastleWallEdges);

        //[Val("CasWllEdg", "Spline")]
        //public readonly Outlet<SplineSys> castleWallEdgeSplineOut = new Outlet<SplineSys>();


        CastleWallEdges.Clamp(tileLocation, tileSize);

        SplineSysBundle.Add(new SplineSysWrapper(
                   nameof(CastleWallEdges), CastleWallEdges, "castleWallEdgeSplineOut"
                   ));

        // ADDED TO THE BUNDLE #############

        //   Debug.Log("Begin splineStreetSys with streets: " + splineStreetSys.lines.Length);
        //   data.StoreProduct(castleWallEdgeSplineOut, CastleWallEdges);

        //foreach (var replacedGate in replacedGates.Distinct())
        //{
        //    geom.Gates.Remove(replacedGate);
        //}

        //foreach (var tower in geom.Towers)
        //{
        //    // Do objects.

        //    //    cube = new Cube("Tower", GetVertices(4, 4, tower.x - 2, tower.y - 2), 0.1f, rendererOptions.TowerMaterial, Walls.transform);
        //    //    cube.Transform.localPosition = Vector3.zero;
        //    //    cube = new Cube("TowerMesh", GetVertices(4, 4, tower.x - 2, tower.y - 2), 8, rendererOptions.TowerMaterial, WallsMesh.transform, false);
        //    //    cube.Transform.localPosition = Vector3.zero;
        //    //}

        //}
        //foreach (var gate in geom.Gates)
        //{
        //    // Do objects.

        //    //cube = new Cube("Gate", GetVertices(4, 4, gate.x - 2, gate.y - 2), 0.1f, rendererOptions.GateMaterial, Walls.transform);
        //    //cube.Transform.localPosition = Vector3.zero;
        //    //cube = new Cube("GateMesh", GetVertices(4, 4, gate.x - 2, gate.y - 2), 6, rendererOptions.GateMaterial, WallsMesh.transform, false);
        //    //cube.Transform.localPosition = Vector3.zero;
        //}


        // NOW DO TOWN WALL

        SplineSys TownWallEdges = new SplineSys();


        // Also do the gatehouses

        SplineSys GatehouseEdges = new SplineSys();


        // List<Line> 
        // reset temp
        lineList = new List<Line>();

        // var replacedGates = new List<Town.Geom.Vector2>();

        // TransitionsList towerLocOut = new TransitionsList();

        //foreach (var section in wall.Towers)
        //{

        //// you could just assume the tower sit on the edge. and connect them [n]->[n+1]

        //}


      // passedwall.Gates

      

        IEnumerable<Town.Geom.Edge> edges = passedwall.GetEdges();
        int loops =0;
         matched = State.no;


        foreach (var edge in edges.Reverse())
        {
           
            var start = edge.A;
            var end = edge.B;

          

            // determine what is a gate and add it.
            // great place to determine node pairing for gatehouse splines...
            if (passedwall.Gates.Contains(start))
            {
                matched = State.yes;
                replacedGates.Add(start);
                start = start + Town.Geom.Vector2.Scale(end - start, 0.3f);
                edge.A = start;
                geom.Gates.Add(start);


                var tot = new Town.Geom.Vector2(start.x * TownGlobalObjectService.WorldMultiplier + WorldOffset.x,
                    start.y * TownGlobalObjectService.WorldMultiplier + WorldOffset.y);

                gatehouseNodes.Add(new Vector3(tot.x, 499.9f, tot.y));

            }

            if (passedwall.Gates.Contains(end) || matched == State.yes || matched == State.noted)
            {
                // second pass
                if (matched == State.noted || passedwall.Gates.Contains(end))
                {
                    matched = State.no;
                    replacedGates.Add(end);
                    end = end - Town.Geom.Vector2.Scale(end - start, 0.3f);
                    edge.B = end;
                    geom.Gates.Add(end);

                    

                    var tot = new Town.Geom.Vector2(
                                                     end.x * TownGlobalObjectService.WorldMultiplier + WorldOffset.x,
                                                     end.y * TownGlobalObjectService.WorldMultiplier + WorldOffset.y);

                    var test = new Vector3(tot.x, 499.9f, tot.y);

                    // if it is close enough to the last node we make a pair. if it isn't we delete the last node too. and dont fail on weird edge cases
                    if (gatehouseNodes.Count == 0)
                    {
                        continue;
                    }
                    if ( Vector3.SqrMagnitude(gatehouseNodes[gatehouseNodes.Count-1] - test) < 40000    )
                    {

                        gatehouseLines.Add(new Line(gatehouseNodes[gatehouseNodes.Count - 1], test));
                        gatehouseNodes.Clear(); 

                    }
                    else
                    {
                        // clear for a non match
                        gatehouseNodes.Clear();
                    }

                }
                // defer.
                if (matched == State.yes)
                {
                    matched = State.noted;
                }
               
            }



            //if (loops ==0)
            //{
            //    replacedGates.Add(start);
            //    start += Town.Geom.Vector2.Scale(end - start, 0.3f);
            //    edge.A = start;
            //    geom.Gates.Add(start);
            //}

            //if (loops == edges.Count()-1)
            //{
            //    replacedGates.Add(end);
            //    end -= Town.Geom.Vector2.Scale(end - start, 0.3f);
            //    edge.B = end;
            //    geom.Gates.Add(end);
            //}

            // add a line here

            lineList.Add(new Line(
            new Vector3(
                        ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset),
                       TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset)),

             new Vector3(
             ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset),
                       TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset))));


            //    place an object - in the middle -of here



            //      Debug.Log("Begin StoreProduct RawAdjustedheightOut");

            var towA = ((start.x + ((end.x - start.x) * 0.5f)) * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x;

            var towB =
                ((start.y + ((end.y - start.y) * 0.5f)) * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y;

            var towLoc = new Coord((int)towA, (int)towB);
          
            
            if (newOffset.Contains(towLoc))
                towerLocOut.Add(
                     new Transition(towA, towB)
                    );




            //foreach (var gate in geom.Gates)
            //{
            //    // Do objects.

            //    //cube = new Cube("Gate", GetVertices(4, 4, gate.x - 2, gate.y - 2), 0.1f, rendererOptions.GateMaterial, Walls.transform);
            //    //cube.Transform.localPosition = Vector3.zero;
            //    //cube = new Cube("GateMesh", GetVertices(4, 4, gate.x - 2, gate.y - 2), 6, rendererOptions.GateMaterial, WallsMesh.transform, false);
            //    //cube.Transform.localPosition = Vector3.zero;
            //}




            loops += 1;

        }

        if (lineList.Count() > 0)
        {
            TownWallEdges.lines = lineList.ToArray<Line>();
        }
        // CastleWallEdges.lines = temp.ToArray<Line>();

        //   Debug.Log("Begin splineStreetSys with streets: " + splineStreetSys.lines.Length);
        //data.StoreProduct(castleWallEdgeSplineOut, CastleWallEdges);

        TownWallEdges.Clamp(tileLocation, tileSize);


        SplineSysBundle.Add(new SplineSysWrapper(
       nameof(TownWallEdges), TownWallEdges, "townWallSplineOut"));


        if (gatehouseLines.Count > 0)
        {
            GatehouseEdges.lines = gatehouseLines.ToArray<Line>();
        


            GatehouseEdges.Clamp(tileLocation, tileSize);
            if (GatehouseEdges.lines.Length > 0)
            {

                SplineSysBundle.Add(new SplineSysWrapper(
              nameof(GatehouseEdges), GatehouseEdges, "gatehouseSplineOut"));
            }
        }
        // ADD TO THE BUNDLE #############

        //   RelaxOptimiseClampLineArray(towerLocOut);

        //           [Val("Towers", "Positions")]
        //public readonly Outlet<TransitionsList> towersOut = new Outlet<TransitionsList>();

        foreach (var gate in startingGates)
        {


            var gateX = (gate.x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x;

            var gateY =
                (gate.y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y;

            var gateLoc = new Coord((int)gateX, (int)gateY);

          
            if (newOffset.Contains(gateLoc))
                gateLocOut.Add(
                     new Transition(gateX, gateY)
                    );

        }

        // ONLY NOW STORE THESE - 
        // SINCE WE HAVE BOTH LISTS NOW.

        if (towerLocOut.count > 0)
        {


            towerLocOut.RemoveAnyBlanksInTransitionListArray();


            TransitionsListBundle.Add(new TransitionsListWrapper(
                       nameof(towerLocOut), towerLocOut, "towersOut"
                       ));
        }

        if (gateLocOut.count > 0)
        {



            gateLocOut.RemoveAnyBlanksInTransitionListArray();

            TransitionsListBundle.Add(new TransitionsListWrapper(
                      nameof(gateLocOut), gateLocOut, "gatesOut"
                      ));

        }
        // ADDED TO THE BUNDLE #############

        //  data.StoreProduct(towersOut, towerLocOut);
    }

    private void Wrapper_Spline_DistrictPatchEdges(Town.Town town, Town.Geom.Vector2 WorldOffset, CoordRect newOffset, bool AppendToExistingBundle)
    {
        SplineSys DistrictEdges = new SplineSys();

        List<Line> temp = new List<Line>();

        int firstcount = town.Patches.Count;

        List<Patch> temppatch = town.Patches.ToList();
        // int midcount = 0;
        //  int endcount = 0;
        foreach (var item in temppatch)
        {
            //  midcount++;
            foreach (var shape in item.Shape.GetEdges())
            {
                //     endcount++;
                if (shape.A != null && shape.B != null)
                {
                    Town.Geom.Vector2 edgeA = shape.A;
                    Town.Geom.Vector2 edgeB = shape.B;
                    // Town.Geom.Vector2 currentOffset = WorldOffset;
                    //  Town.Geom.Vector2 currentOffset = town.townOffset.ToTileSizeTownGeomVector2();

                    Vector3 VecA = new Vector3(
                        ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, edgeA, WorldOffset),
                      TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, edgeA, WorldOffset));

                    Vector3 VecB = new Vector3(
                           ScaleAndShiftToWorldX(
                               TownGlobalObjectService.WorldMultiplier, edgeB, WorldOffset),
                             TownGlobalObjectService.WorldHeight,
                         ScaleAndShiftToWorldZ(
                             TownGlobalObjectService.WorldMultiplier, edgeB, WorldOffset)); ;
                    //    CoordRect newOffset = MakeCollisionRectWithOffset(WorldOffset);


                    // maybe this filter will work?
                  //  if (TwoVectorsToTileSpaceBothIsTruncated(ref VecA, ref VecB, newOffset))
                 //   {
                        // Now Modulo the value by tile width 1000
                     //   VecA = new Vector3(VecA.x % 1000, VecA.y, VecA.z % 1000);
                     //   VecB = new Vector3(VecB.x % 1000, VecB.y, VecB.z % 1000);

                        temp.Add(new Line(VecA, VecB));

                //    }
                }
            }
        }

        DistrictEdges.lines = temp.ToArray<Line>();

        //  RelaxOptimiseClampLineArray(DistrictEdges);

       

        // ADD TO THE BUNDLE #############

        //          [Val("Districts", "Spline")]
        //public readonly Outlet<SplineSys> splineDistrictsOut = new Outlet<SplineSys>();
        DistrictEdges.Clamp(tileLocation, tileSize);


        ///TODO: IMPLEMENT THIS FOR ALL TYPES.

        if (AppendToExistingBundle)
        {
            int locationInList = SplineSysBundle.FindIndex(x => x.outletName == "splineDistrictsOut");

            int oldlinecount = SplineSysBundle[locationInList].splineSys.lines.Length;

            Debug.LogFormat("is {0} in a list of {1} for {2} with {3}  has {4} lines to begin", locationInList, SplineSysBundle.Count, town.name, bundle.name, oldlinecount);

            SplineSysWrapper currentwrapper = SplineSysBundle[locationInList];

            currentwrapper.splineSys.AddLines(DistrictEdges.lines);
       

            Debug.LogFormat("{0} old lines plus {1} new lines gives {2} line", oldlinecount, DistrictEdges.lines.Length, currentwrapper.splineSys.lines.Length);

            SplineSysBundle[locationInList] = currentwrapper;
        }
        else
        {

            SplineSysBundle.Add(new SplineSysWrapper(
           nameof(DistrictEdges), DistrictEdges, "splineDistrictsOut"
           ));

        }





        //SplineSysBundle.Add(new SplineSysWrapper(
        //            nameof(DistrictEdges), DistrictEdges, "splineDistrictsOut"
        //            ));

        // ADDED TO THE BUNDLE #############

        //     data.StoreProduct(splineDistrictsOut, DistrictEdges);
    }

    private void Wrapper_Spline_Streets(Town.Town town, Town.Geom.Vector2 WorldOffset, CoordRect newOffset, out List<Line> RoadsplineList, out Line RoadSpline, out Line[] Roadsplines , bool AppendToExistingBundle)
    {
        SplineSys StreetsplineSys = new SplineSys();


        RoadsplineList = new List<Line>();
        // Spline mySpline = new Spline();

       //  CoordRect newOffset = MakeCollisionRectWithOffset(WorldOffset);

        RoadSpline = new Line();

        if (town.Streets.Count < 1)
            Debug.LogWarning("No streets in roadways");

        //  int timeThru = 0;

        foreach (var listOfStreetNodes in town.Streets)
        {

            //   Debug.Log(++timeThru);

            //   Debug.Log("Begin street in streets");

            List<Vector3> mySplineNodes = new List<Vector3>();


            // float FUDGE_TO_FIT_MAP = 500f;


            if (town.Streets.Count < 1)
            {
                Debug.LogWarning("No nodes in itemslist");

                continue;
            }

            //    Debug.Log("Begin nodes in street");

            //  We need to know which items connect to which so we will do every other item. 
            // The list should always be divisible by two anyway

            for (int i = 0; i < listOfStreetNodes.Count - 1; i++)
            {

                Vector3 nodeStart =
              new Vector3(
          (listOfStreetNodes[i].x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,
     // Make it almost on the ceiling                                                                           
     499,
          (listOfStreetNodes[i].y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y

          );



                //if (i + 1 > listOfStreetNodes.Count - 1)
                //{
                //    // ODD NUMBER!
                //    Debug.Log("odd number of road nodes.");
                //    continue;
                //}

                Vector3 nodeEnd =
            new Vector3(
        (listOfStreetNodes[i + 1].x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,
   // Make it almost on the ceiling                                                                           
   499,
        (listOfStreetNodes[i + 1].y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y);

                //    Debug.LogFormat("ORIG: {0},{1} to {2}{3} ", listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y);


                //     Debug.LogFormat("ORIG: {0},{1} to {2}{3} PROCESSING FROM from {4} to {5}",listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y, nodeStart, nodeEnd);





                       // if (TwoVectorsToTileSpaceBothIsTruncated(ref nodeStart, ref nodeEnd, newOffset))
                    //   {
                 //   Debug.LogFormat("ACCEPTED from {0} to {1} in a {2}:{3} offset rect FOR STREETS", nodeStart, nodeEnd, newOffset.offset.x, newOffset.offset.z);
                    mySplineNodes.Add(nodeStart);
                mySplineNodes.Add(nodeEnd);
                }
                     

           // }




            Vector3[] nodesMade = mySplineNodes.ToArray<Vector3>();

            //if (nodesMade.Count() > 0)
            //{
            //    Debug.Log("Begin make Road Spline with node [0]: " + nodesMade[0]);
            //}




            for (int i = 0; i < nodesMade.Length - 1; i++)
            {
                Den.Tools.Splines.Line current = new Den.Tools.Splines.Line(nodesMade[i], nodesMade[i + 1]);

                RoadsplineList.Add(current);


            }



        }



        //   Debug.Log("Begin make array from SplineList");
        Roadsplines = RoadsplineList.ToArray<Line>();

        if (Roadsplines.Length < 1)
        {
          //s  Debug.Log("Zero Roadsplines Splines");

            RoadSpline = new Line(new Vector3(509f, .2f, 505f), new Vector3(505f, .2f, 509f));


            Roadsplines = new Line[] { RoadSpline };
        }
        else
        {
            //   Debug.Log(splines.Length + " itemslist nodes processed");
            //  Debug.Log(record.x + " , 0 , " + record.y);
        }

        //   Debug.Log("Begin make Road splineSys from splines");

        foreach (var spli in Roadsplines)
        {
            StreetsplineSys.AddLine(spli);

        }



        //      [Val("Streets", "Spline")]
        //public readonly   Outlet<SplineSys> streetsplineOut = new Outlet<SplineSys>();


        // ADD TO THE BUNDLE #############

        // since we use clamp here we no loger have to ensure it is in pairs.
        // Just pass the array as is.


        StreetsplineSys.Clamp(tileLocation, tileSize);

        if (AppendToExistingBundle)
        {
            int locationInList = SplineSysBundle.FindIndex(x => x.outletName == "streetsplineOut");

            Debug.LogFormat("is {0} in a list of {1} for {2} with {3}", locationInList, SplineSysBundle.Count, town.name, bundle.name);

            SplineSysWrapper currentwrapper = SplineSysBundle[locationInList];

            currentwrapper.splineSys.AddLines(Roadsplines);

            SplineSysBundle[locationInList] = currentwrapper;

        }
        else
        {

            SplineSysBundle.Add(new SplineSysWrapper(
           nameof(StreetsplineSys), StreetsplineSys, "streetsplineOut"
           ));

        }

        // ADDED TO THE BUNDLE #############
    }

    private void Wrapper_Spline_Roads(Town.Town town, Town.Geom.Vector2 WorldOffset, CoordRect newOffset, out List<Line> splineList, out Line mySpline, out Line[] splines, bool AppendToExistingBundle)
    {
        SplineSys RoadsplineSys = new SplineSys();

        TransitionsList RoadEnd = new TransitionsList();

        TransitionsList Terminus = new TransitionsList();


        //   Debug.LogFormat("{0} is {1}", nameof(town.Roads), town.Roads.Count);
        //   Debug.LogFormat("{0} is {1}", nameof(town.Streets), town.Streets.Count);

        // WE NO LONGER USE THE SEGMENTED WORLD OUTPUT HERE.

        //ExtractRoadwayToRefSplineSys(town.Roads, WorldOffset, ref RoadsplineSys);  // Outlet<SplineSys> roadsplineOut

        // if (!isDraft)
        //    ExtractRoadwayToRefSplineSys(town.Roads, WorldOffset.ToCoord(), ref RoadsplineSys);  // Outlet<SplineSys> roadsplineOut



        splineList = new List<Line>();
        // Spline mySpline = new Spline();

       //    CoordRect newOffset = MakeCollisionRectWithOffset(WorldOffset);

        mySpline = new Line();
        if (town.Roads.Count < 1)
            Debug.LogWarning("No road in roads");

        //   int timeThru = 0;

        foreach (var listOfRoadNodes in town.Roads)
        {
            if (listOfRoadNodes.Count < 1)
            {
                continue;
            }
            Town.Geom.Vector2 takelast = listOfRoadNodes.Last();

            Vector3 terminal = new Vector3(
                (takelast.x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,                                                                      
                499f,
                (takelast.y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y
          );

            if (newOffset.Contains(terminal))
            {
                Terminus.Add( new Transition(terminal.x, terminal.y, terminal.z));
            }


            //   Debug.Log(++timeThru);

            //   Debug.Log("Begin street in streets");

            List<Vector3> mySplineNodes = new List<Vector3>();


            // float FUDGE_TO_FIT_MAP = 500f;


            if (listOfRoadNodes.Count < 1)
            {
                Debug.LogWarning("No nodes in itemslist");

                continue;
            }

            //    Debug.Log("Begin nodes in street");

            //  We need to know which items connect to which so we will do every other item. 
            // The list should always be divisible by two anyway

            // Skip the first and wind backwards effectively.

            for (int i = 1; i < listOfRoadNodes.Count; i += 1)
            {

                Vector3 nodeStart =
              new Vector3(
          (listOfRoadNodes[i-1].x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,
     // Make it almost on the ceiling                                                                           
     499,
          (listOfRoadNodes[i-1].y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y

          );



                //if (i + 1 > listOfRoadNodes.Count - 1)
                //{
                //    // ODD NUMBER!
                //    Debug.Log("odd number of road nodes.");
                //    continue;
                //}

                Vector3 nodeEnd =
            new Vector3(
        (listOfRoadNodes[i ].x * TownGlobalObjectService.WorldMultiplier) + WorldOffset.x,
   // Make it almost on the ceiling                                                                           
   499,
        (listOfRoadNodes[i ].y * TownGlobalObjectService.WorldMultiplier) + WorldOffset.y);

                //    Debug.LogFormat("ORIG: {0},{1} to {2}{3} ", listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y);


                //     Debug.LogFormat("ORIG: {0},{1} to {2}{3} PROCESSING FROM from {4} to {5}",listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y, nodeStart, nodeEnd);





                         // if (TwoVectorsToTileSpaceBothIsTruncated(ref nodeStart, ref nodeEnd, newOffset))
                       //   {
                    //  Debug.LogFormat("ACCEPTED from {0} to {1} in a {2}:{3} offset rect", nodeStart, nodeEnd, newOffset.offset.x, newOffset.offset.z);
               //if (!mySplineNodes.Contains(nodeStart))
                        if (i==1)                        
                    mySplineNodes.Add(nodeStart);
                //if(!mySplineNodes.Contains(nodeEnd))
                     mySplineNodes.Add(nodeEnd);
                    //  }




            }





            Vector3[] nodesMade = mySplineNodes.ToArray<Vector3>();

            //if (nodesMade.Count() > 0)
            //{
            //    Debug.Log("Begin make Road Spline with node [0]: " + nodesMade[0]);
            //}




            for (int i = 1; i < nodesMade.Length ; i++)
            {
             //   if (TwoVectorsToTileSpaceBothIsTruncated(ref nodesMade[i - 1], ref nodesMade[i], newOffset))
             //   {
                    Den.Tools.Splines.Line current = new Den.Tools.Splines.Line(nodesMade[i - 1], nodesMade[i]);
                    //  if (current.segments[0].StartEndLength > 0)
                    //   {
                    splineList.Add(current);
                    //  }
              //  }
            }



        }

        bool skipRoadEnd = false;

        //   Debug.Log("Begin make array from SplineList");
        splines = splineList.ToArray<Line>();
        if (splines.Length < 1)
        {
            // return;
            skipRoadEnd = true;
            //  Debug.Log("Zero road Splines");

            mySpline = new Line(new Vector3(509f, .2f, 505f), new Vector3(509f, .2f, 505f));


            splines = new Line[] { mySpline };
        }
        else
        {
            //   Debug.Log(splines.Length + " itemslist nodes processed");
            //  Debug.Log(record.x + " , 0 , " + record.y);
        }

        //   Debug.Log("Begin make Road splineSys from splines");




        if (!skipRoadEnd)
        foreach (var spli in splines)
        {

            Transition end = new Transition(spli.GetNodePos(spli.NodesCount - 1).x, spli.GetNodePos(spli.NodesCount - 1).z);
            RoadEnd.Add(end);


            RoadsplineSys.AddLine(spli);

        }





        //  ExtractRoadwayToRefSplineSys(town.Roads, data_area_coords, ref RoadsplineSys);  // Outlet<SplineSys> roadsplineOut


        //[Val("Roads", "Spline")]
        //  Outlet<SplineSys> roadsplineOut = new Outlet<SplineSys>();

        // ADD TO THE BUNDLE #############

        RoadsplineSys.Clamp(tileLocation, tileSize);

        if (AppendToExistingBundle)
        {
            int locationInList = SplineSysBundle.FindIndex(x => x.outletName == "roadsplineOut");

          //  Debug.LogFormat("is {0} in a list of {1} for {2} with {3}", locationInList, SplineSysBundle.Count, town.name, bundle.name);

            SplineSysWrapper currentwrapper = SplineSysBundle[locationInList];

            currentwrapper.splineSys.AddLines(splines);

            SplineSysBundle[locationInList] = currentwrapper;

        }
        else
        {

            SplineSysBundle.Add(new SplineSysWrapper(
           nameof(RoadsplineSys), RoadsplineSys, "roadsplineOut"
           ));

            TransitionsList roadTrunk = new TransitionsList();
            if (!skipRoadEnd)
            {
                RoadEnd.RemoveAnyBlanksInTransitionListArray();
                //// Let's cleanup
                //roadTrunk = new TransitionsList(RoadEnd.count);

                //for (int i = 0; i < RoadEnd.count; i++)
                //{
                //    roadTrunk.arr[i] = RoadEnd.arr[i];
                //}

            }



            TransitionsListBundle.Add(new TransitionsListWrapper(
            nameof(RoadEnd), RoadEnd, "roadEndOut"
            ));

            Terminus.RemoveAnyBlanksInTransitionListArray();

            TransitionsListBundle.Add(new TransitionsListWrapper(
           nameof(Terminus), Terminus, "terminusOut"
           ));

        }

        //if(!town.Terminus.Contains(Terminus) )
        //town.Terminus.Add(Terminus);

    }

    /// <summary>
    /// Not Used
    /// </summary>
    /// <param name="town"></param>
    /// <param name="WorldOffset"></param>
    private void Wrapper_Spline_TownWallsCircumference(Town.Town town, Town.Geom.Vector2 WorldOffset)
    {
        SplineSys townWallSplinesSys = new SplineSys();
        //    Debug.Log("Begin dstObjs");
        List<Line> townWallSplines = new List<Line>(town.CityWall.Circumference.Count);

        //   if (!isDraft)
        if (true)
        {

            for (int i = 0; i < town.CityWall.Circumference.Count - 1; i = i + 2)
            {

                List<Town.Geom.Vector2> tempwalls = new List<Town.Geom.Vector2>(town.CityWall.Circumference);

                townWallSplines.Add(new Line(
                    new Vector2(
                        (tempwalls[i].x * TownGlobalObjectService.WorldMultiplier)
                        + WorldOffset.x,
                        (tempwalls[i].y * TownGlobalObjectService.WorldMultiplier)
                        + WorldOffset.y),

                    new Vector2(
                        (tempwalls[i + 1].x * TownGlobalObjectService.WorldMultiplier)
                        + WorldOffset.x,
                        (tempwalls[i + 1].y * TownGlobalObjectService.WorldMultiplier)
                        + WorldOffset.y)
                    ));


            }


            //foreach (var item in town.CityWall.Circumference)
            //{

            //    townWallSplines.Add(new Line(item.x + WorldOffset.x, item.y + WorldOffset.y));
            //}
        }

        townWallSplinesSys.lines = townWallSplines.ToArray<Line>();


        // ADD TO THE BUNDLE #############

        //        [Val("TownWall", "Spline")]
        //public readonly Outlet<SplineSys> townWallSplineOut = new Outlet<SplineSys>();

        townWallSplinesSys.Clamp(tileLocation, tileSize);

        SplineSysBundle.Add(new SplineSysWrapper(
               nameof(townWallSplinesSys), townWallSplinesSys, "townWallSplineOut"
               ));
    }

    /// <summary>
    /// Not Used
    /// </summary>
    /// <param name="town"></param>
    /// <param name="WorldOffset"></param>
    /// <param name="newOffset"></param>
    private void Wrapper_Spline_CastleWallCicumference(Town.Town town, Town.Geom.Vector2 WorldOffset, CoordRect newOffset)
    {
        SplineSys castleWallSplinesSys = new SplineSys();
        //    Debug.Log("Begin dstObjs");
        List<Line> castleWallSplines = new List<Line>(town.Castle.Patch.Edges.Count);



        for (int i = 0; i < town.Castle.Patch.Edges.Count - 1; i++)
        {

            List<Town.Geom.Edge> tempcastlewalls = new List<Town.Geom.Edge>(town.Castle.Patch.Edges);

            Vector3 A = new Vector3(
                    (tempcastlewalls[i].A.x * TownGlobalObjectService.WorldMultiplier)
                    + WorldOffset.x,
                    TownGlobalObjectService.WorldHeight,
                    (tempcastlewalls[i].A.y * TownGlobalObjectService.WorldMultiplier)
                    + WorldOffset.y);

            Vector3 B = new Vector3(
                    (tempcastlewalls[i].B.x * TownGlobalObjectService.WorldMultiplier)
                    + WorldOffset.x,
                    TownGlobalObjectService.WorldHeight,
                    (tempcastlewalls[i].B.y * TownGlobalObjectService.WorldMultiplier)
                    + WorldOffset.y);

            //    CoordRect newOffset = MakeCollisionRectWithOffset(WorldOffset);

            if (TwoVectorsToTileSpaceBothIsTruncated(ref A, ref B, newOffset))
                castleWallSplines.Add(new Line(
                    new Vector2(
                        A.x,
                         A.z),

                    new Vector2(
                       B.x,
                        B.z
                    )));


        }

        castleWallSplinesSys.lines = castleWallSplines.ToArray<Line>();

        // ADD TO THE BUNDLE #############

        //      [Val("CasWall", "Spline")]
        //public readonly Outlet<SplineSys> castleWallSplineOut = new Outlet<SplineSys>();


        SplineSysBundle.Add(new SplineSysWrapper(
               nameof(castleWallSplinesSys), castleWallSplinesSys, "castleWallSplineOut"
               ));
    }
    
    /// <summary>
    /// Not Used
    /// </summary>
    /// <param name="WorldOffset"></param>
    /// <param name="wall"></param>
    /// <param name="newOffset"></param>
    private void Wrapper_Spline_TownWalls(Town.Geom.Vector2 WorldOffset, Wall wall, CoordRect newOffset)
    {

        SplineSys TownWallEdges = new SplineSys();

        List<Line> temp = new List<Line>();

        var replacedGates = new List<Town.Geom.Vector2>();

        TransitionsList towerLocOut = new TransitionsList();

        //foreach (var section in wall.Towers)
        //{

        //// you could just assume the tower sit on the edge. and connect them [n]->[n+1]

        //}

        foreach (var edge in wall.GetEdges())
        {



            var start = edge.A;
            var end = edge.B;


            // add a line here

            temp.Add(new Line(
            new Vector3(
                        ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset),
                       TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, start, WorldOffset)),

             new Vector3(
             ScaleAndShiftToWorldX(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset),
                       TownGlobalObjectService.WorldHeight,
                        ScaleAndShiftToWorldZ(
                            TownGlobalObjectService.WorldMultiplier, end, WorldOffset))));


            //    place an object - in the middle -of here



            //      Debug.Log("Begin StoreProduct RawAdjustedheightOut");

            Vector3 testVecStart = new Vector3(start.x, 250f, start.y);
            Vector3 testVecEnd = new Vector3(end.x, 250f, end.y);

            if (TwoVectorsToTileSpaceBothIsTruncated(ref testVecStart, ref testVecEnd, newOffset))

            //  if (TwoVectorsToTileSpaceBothIsTruncated(new Vector3(start.x,250f,start.y), new Vector3(end.x, 250f, end.y), newOffset))
            {
                towerLocOut.Add(
                         new Transition(
                             ((testVecStart.x + ((testVecEnd.x - testVecStart.x) * 0.5f)) * TownGlobalObjectService.WorldMultiplier)
                              + WorldOffset.x,

                              ((testVecStart.z + ((testVecEnd.z - testVecStart.z) * 0.5f)) * TownGlobalObjectService.WorldMultiplier)
                               + WorldOffset.y)

                               );
            }
        }

        if (temp.Count() > 0)
        {
            TownWallEdges.lines = temp.ToArray<Line>();
        }
        // CastleWallEdges.lines = temp.ToArray<Line>();

        //   Debug.Log("Begin splineStreetSys with streets: " + splineStreetSys.lines.Length);
        //data.StoreProduct(castleWallEdgeSplineOut, CastleWallEdges);

        TownWallEdges.Clamp(tileLocation, tileSize);

        SplineSysBundle.Add(new SplineSysWrapper(
       nameof(TownWallEdges), TownWallEdges, "townWallSplineOut"));


    }
    
    private static CoordRect MakeCollisionRectWithOffset(Town.Geom.Vector2 currentOffset)
    {
        return new CoordRect(currentOffset.ToCoord(), new Coord(1000, 1000));
    }

    private static CoordRect MakeCollisionRectWithOffset(Den.Tools.Coord currentOffset)
    {
        return new CoordRect((currentOffset) * 1000f, new Coord(1000));
    }

    private static bool IsCoordLocality(Coord data_area_coords)
    {
        return data_area_coords == TownGlobalObject.GetIndexAtCoord(data_area_coords); ;
    }

    private static void RelaxOptimiseClampLineArray(SplineSys DistrictEdges)
    {
        var newlines = new ArraySegment<Line>(DistrictEdges.lines, 0, (DistrictEdges.lines.Length));

        DistrictEdges.lines = RelaxOptimiseClampLinesInArraySegmentToArray(newlines);
     
    }

    private static Line[] RelaxOptimiseClampLinesInArraySegmentToArray(ArraySegment<Line> newlines)
    {
        foreach (Line spline in newlines)
        {
            spline.Relax(0.5f, (int)1);
            spline.Optimize(1 * 0.5f);

            // World Tile Size / World Height.
        //    spline.Clamped(Vector3.zero, new Vector3(1000, 500, 1000));
        }
        return newlines.ToArray<Line>();
    }

    //private static void RelaxOptimiseClampLineArray(TransitionsList DistrictEdges)
    //{
    //    foreach (Transition ine in DistrictEdges)
    //    {
    //        spline.Relax(0.5f, (int)1);
    //        spline.Optimize(1 * 0.5f);

    //        // World Tile Size / World Height.
    //        spline.Clamped(Vector3.zero, new Vector3(1000, 500, 1000));
    //    }
    //}

    private static Vector3[] GetLineVertices(float startX, float endX, float startY, float endY, float thickness = 1f)
    {
        var p1 = new Vector3(startX, 0, startY);
        var p2 = new Vector3(endX, 0, endY);
        var dir = (p1 - p2).normalized;
        var norm = Vector3.Cross(dir, Vector3.up);
        var halfThickness = (norm * thickness) / 2;
        var p3 = p2 + halfThickness;
        var p4 = p1 + halfThickness + dir / 2;
        p1 = p1 - halfThickness + dir / 2;
        p2 = p2 - halfThickness;
        return new Vector3[]
        {
                p1,
                p2,
                p3,
                p4
        };
    }

    private static Vector3[] GetVertices(int width, int length, float offsetX, float offsetZ)
    {
        return new Vector3[]
        {
                new Vector3 (offsetX, 0, offsetZ),
                    new Vector3 (offsetX, 0, offsetZ + length),
                    new Vector3 (offsetX + width, 0, offsetZ + length),
                    new Vector3 (offsetX + width, 0, offsetZ)
        };
    }


    private static float ScaleAndShiftToWorldZ(float worldMultiplier, Town.Geom.Vector2 edgeA, Town.Geom.Vector2 currentOffset)
    {

        return (edgeA.y * worldMultiplier) + currentOffset.y;
    }

    private static float ScaleAndShiftToWorldX(float worldMultiplier, Town.Geom.Vector2 edgeA, Town.Geom.Vector2 currentOffset)
    {
        return (edgeA.x * worldMultiplier) + currentOffset.x;
    }

    /// <summary>
    /// Should move one side of a line to 0, or 500
    /// or elide a line completely that lies outside this range.
    /// </summary>
    /// <param name="start">one edge</param>
    /// <param name="end">the other edge</param>
    /// <param name="coordTestRect">must be a 0,0 -> 1000,1000 in size rect offset as you wish</param>
    /// <returns></returns>
    private static bool TwoVectorsToTileSpaceBothIsTruncated( ref   Vector3 start,
                                                              ref   Vector3 end,
                                                                Den.Tools.CoordRect coordTestRect) {

        //  if (!data.area.active.Contains(trn.pos)) continue; //skipping out-of-active area

        //// TODO: FIX THIS.  We cant tile merge until we do.
          return true;

       // Den.Tools.CoordRect coordTestRectTemp = new CoordRect(coordTestRect.offset, coordTestRect.size);


        bool processBothRecords = true;

        if (coordTestRect == null) {
            Debug.Log("default coordTestRect");
            coordTestRect = new CoordRect(Coord.zero, new Coord(1000));
        }
        Coord started = start.ToCoord(); // new Coord((int)start.x, (int)start.z);
        Coord ended = end.ToCoord(); // new Coord((int)end.x, (int)end.z);


        /// Fake holder for the tests.
        //  Den.Tools.CoordRect coordTestRect = new CoordRect(Coord.zero, new Coord(1000));  
        // should be 0,0 -> 1000,1000


        if (!coordTestRect.Contains(started) && !coordTestRect.Contains(ended))
        {
            //  Debug.LogFormat("{0}<- and {1}<- rejected from {2} and {3} with a {4} container", started, ended, start, end , coordTestRect.offset);
           /// processBothRecords = false;
            return false;
        }
        else
        if (!coordTestRect.Contains(started))
        {

            // started.ClampByRect(coordTestRect);


            //     Debug.LogFormat("{0} and {1}<- rejected from {2} and {3}<-", started, ended, start, end);
            //   ended.ClampByRect(coordTestRect);
            // rebuild the Vector3

            started = TrimToRect(coordTestRect, started);
            start = started.ToCeilVector3();
        }
        else
        if (!coordTestRect.Contains(ended))
        {
        //    Debug.LogFormat("{0}<- and {1} rejected from {2}<- and {3}", started, ended, start, end);

            ended = TrimToRect(coordTestRect, ended);
            end = ended.ToCeilVector3();
            

            //  ended.ClampByRect(coordTestRect);
            //    rebuild the Vector3
            //    end = ended.ToCeilVector3(); // new Vector3(ended.x, 499, ended.z);
        }
        else
        {
            // This should be a FULLY matching record
        }


        // started.ClampByRect(coordTestRect);
       // new Vector3(started.x, 499, started.z);

       // ended.ClampByRect(coordTestRect);
        // rebuild the Vector3
        // new Vector3(ended.x, 499, ended.z);

        //  by here we can ignore anything that doesn't match
      //  Debug.LogFormat("{0}<- and {1}<- {6} both {2}<- and {3}<- with a {4} container of size {5}", started, ended, start, end, coordTestRect.offset, coordTestRect.size, (processBothRecords) ? "accepted": "rejected");

        return processBothRecords;
     


    }

    // because for some reason mapMagic does some ugly end thing... lets try spreading it over....
    private static Coord TrimToRect(CoordRect coordTestRect, Coord locale)
    {
        int fudge = -50;

        if (locale.x < coordTestRect.offset.x)
        {
            locale.x = Mathf.Max(coordTestRect.offset.x - fudge, locale.x);
        }

        if (locale.x > coordTestRect.offset.x)
        {
            locale.x = Mathf.Min(coordTestRect.offset.x + coordTestRect.size.x + fudge, locale.x);
        }
        if (locale.z < coordTestRect.offset.z)
        {
            locale.z = Mathf.Max(coordTestRect.offset.z - fudge, locale.z);
        }
        if (locale.z > coordTestRect.offset.z)
        {
            locale.z = Mathf.Min(coordTestRect.offset.z + coordTestRect.size.z + fudge, locale.z);
        }
        return locale;
    }

    private static void ExtractRoadwayToRefSplineSys(
        List<List<Town.Geom.Vector2>> ListOfRoadways, 
        Coord PassedWorldOffest, 
        ref SplineSys splineSys)
    {
        //Spline[] splines = SplineMatrixOps.Isoline(matrixHeightIn, curLevel);

        List<Den.Tools.Splines.Line> splineList = new List<Line>();
        // Spline mySpline = new Spline();

     //   CoordRect newOffset = MakeCollisionRectWithOffset(PassedWorldOffest);

        Line mySpline = new Line();

        if (ListOfRoadways.Count < 1)
            Debug.LogWarning("No road in roads");

       // int timeThru = 0;

        foreach (var listOfRoadNodes in ListOfRoadways)
        {

         //   Debug.Log(++timeThru);

            //   Debug.Log("Begin street in streets");

            List<Vector3> mySplineNodes = new List<Vector3>();


           // float FUDGE_TO_FIT_MAP = 500f;


            if (listOfRoadNodes.Count < 1)
            {
                Debug.LogWarning("No nodes in itemslist");

                continue;
            }

            //    Debug.Log("Begin nodes in street");

            //  We need to know which items connect to which so we will do every other item. 
            // The list should always be divisible by two anyway

            for (int i = 0; i < listOfRoadNodes.Count -1; i = i + 2)
            {
              
                Vector3 nodeStart =
              new Vector3(
          (listOfRoadNodes[i].x * TownGlobalObjectService.WorldMultiplier) + PassedWorldOffest.x, 
        // Make it almost on the ceiling                                                                           
     499,
          (listOfRoadNodes[i].y * TownGlobalObjectService.WorldMultiplier) + PassedWorldOffest.z
          
          );
              


                if (i + 1 > listOfRoadNodes.Count -1)
                {
                    // ODD NUMBER!
                    Debug.Log("odd number of road nodes.");
                    continue;
                }

                Vector3 nodeEnd =
            new Vector3(
        (listOfRoadNodes[i+1].x * TownGlobalObjectService.WorldMultiplier) + PassedWorldOffest.x,
   // Make it almost on the ceiling                                                                           
   499,
        (listOfRoadNodes[i+1].y * TownGlobalObjectService.WorldMultiplier) + PassedWorldOffest.z);

                //    Debug.LogFormat("ORIG: {0},{1} to {2}{3} ", listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y);


                //     Debug.LogFormat("ORIG: {0},{1} to {2}{3} PROCESSING FROM from {4} to {5}",listOfRoadNodes[i].x, listOfRoadNodes[i].y, listOfRoadNodes[i + 1].x, listOfRoadNodes[i + 1].y, nodeStart, nodeEnd);

           



            //   if (TwoVectorsToTileSpaceBothIsTruncated(nodeStart, nodeEnd, newOffset))
            //   {
                   Debug.LogFormat("ACCEPTED from {0} to {1}", nodeStart, nodeEnd);
                    mySplineNodes.Add(nodeStart);
                    mySplineNodes.Add(nodeEnd);
            //   }


            }


   

            Vector3[] nodesMade = mySplineNodes.ToArray<Vector3>();

            //if (nodesMade.Count() > 0)
            //{
            //    Debug.Log("Begin make Road Spline with node [0]: " + nodesMade[0]);
            //}
                 
          


            for (int i = 0; i < nodesMade.Length - 2; i++)
            {
                Den.Tools.Splines.Line current = new Den.Tools.Splines.Line(nodesMade[i], nodesMade[i + 1]);

                splineList.Add(current);

            }



        }



        //   Debug.Log("Begin make array from SplineList");
        Line[] splines = splineList.ToArray<Line>();

        if (splines.Length < 1)
        {
            Debug.Log("Zero road Splines");

            mySpline = new Line(new Vector3(509f, .2f, 505f), new Vector3(509f, .2f, 505f));


            splines = new Line[] { mySpline };
        }
        else
        {
            //   Debug.Log(splines.Length + " itemslist nodes processed");
            //  Debug.Log(record.x + " , 0 , " + record.y);
        }

        //   Debug.Log("Begin make Road splineSys from splines");

        foreach (var spli in splines)
        {
            splineSys.AddLine(spli);

        }

        //foreach (Line spline in splineSys.lines)
        //{
        //    spline.Relax(0.5f, (int)1);
        //    spline.Optimize(1 * 0.5f);

        //    // World Tile Size / World Height.
        //  //  spline.Clamped(Vector3.zero, new Vector3(1000, 500, 1000));
        //}


        

        //  Debug.Log("Begin splineSys with itemslist: " + (splineSys.lines.Length - 1));
    //   data.StoreProduct(outlet, splineSys);


    }
}

public static class ConverterExtensions
{
    /// <summary>
    /// Warning! loses precision from float to int
    /// </summary>
    /// <param name="input">the vector3 to transform</param>
    /// <returns></returns>
    public static Coord ToCoord(this Vector3 input)
    {
        return new Coord((int)input.x, (int)input.z);  
    }

    public static Vector3 ToCeilVector3(this Coord input)
    {
        return new Vector3(input.x, 499.9f, input.z); 
    }

    public static void RemoveAnyBlanksInTransitionListArray(this TransitionsList list)
    {
        list.arr = list.arr.Truncated(list.count);    

      //  list.arr = list.arr.Where(x => x.pos != Vector3.zero).Select(x => x).ToArray();
       
    }



    /// <summary>
    /// For easy tile to world conversions
    /// </summary>
    /// <param name="input">tilespace numbers</param>
    /// <returns>vector3 of 1000 magnitude the input at height 500</returns>
    public static Vector3 ToTileSizeCeilVector3(this Coord input)
    {
        return new Vector3((int)input.x * 1000, 499.9f, (int)input.z * 1000);
    }

    /// <summary>
    /// For easy tile to world conversions
    /// </summary>
    /// <param name="input">tilespace numbers</param>
    /// <returns>vector3 of 1000 magnitude the input at height 500</returns>
    public static Vector3 ToTileSizeVector3(this Coord input)
    {
        return new Vector3((int)input.x * 1000, 0, (int)input.z * 1000);
    }

    public static Town.Geom.Vector2 ToGeomVector2(this Vector2 input)
    {
        return new Town.Geom.Vector2(input.x, input.y);
    }

    public static Coord ToCoord(this Town.Geom.Vector2 input)
    {
        return new Coord((int)input.x, (int)input.y);
    }

    public static Coord ToTileSizeCoord(this Town.Geom.Vector2 input)
    {
        return new Coord((int)input.x * 1000, (int)input.y * 1000);
    }

 
    public static Vector2 ToCeilVector3(this Town.Geom.Vector2 input)
    {
        return new Vector3(input.x, 499.9f, input.y);
    }


    public static Vector2 ToZeroVector3(this Town.Geom.Vector2 input)
    {
        return new Vector3(input.x, 0, input.y);
    }


    public static Town.Geom.Vector2 ToTileSizeTownGeomVector2(this Town.Geom.Vector2 input)
    {
        return new Town.Geom.Vector2((int)input.x * 1000, (int)input.y * 1000);
    }



}


using System.Collections;
using System.Collections.Generic;
using MapMagic.Core;
using MapMagic.Terrains;
using MapMagic.Products;
using UnityEngine;
using System.Linq;
using Den.Tools;
using MapMagic.Nodes.MatrixGenerators;
using System;

public class RunOnTileActionEvent : MonoBehaviour
{

    public Transform thingToTrack;
    private static Transform thingToTrackReference;


    static Queue<GameObject> activationList;//= new Queue<Transform>();
    static Queue<GameObject> deactivationList;//= new Queue<Transform>();

    public Transform RenderingMessage;
    public Transform DistrictMessage;

    private static GameObject DistrictMessageRefGameObject;


    public static TownGlobalObjectService tgos;

    private static Transform RenderingMessageRef;
    private static GameObject RenderingMessageRefGameObject;

    public static RunOnTileActionEvent RunOnAnyTilePrepActivityRef;

    public int SkimpFactor = 3;
    int execs = 0;

    // Start is called before the first frame update
    void Start()
    {
        thingToTrackReference = thingToTrack;

        RunOnAnyTilePrepActivityRef = this;

        //Queue<GameObject> 
        activationList = new Queue<GameObject>();
        //Queue<GameObject> 
        deactivationList = new Queue<GameObject>();

        RenderingMessageRef = RenderingMessage;
        RenderingMessageRefGameObject = RenderingMessageRef.gameObject;

        DistrictMessageRefGameObject = DistrictMessage.gameObject;

        TerrainTile.OnBeforeTileGenerate -= OnBeforeTileGenerated;
        TerrainTile.OnBeforeTileGenerate += OnBeforeTileGenerated;

        TerrainTile.OnTileApplied -= OnTileAppliedRenderMeshSplines;
        TerrainTile.OnTileApplied += OnTileAppliedRenderMeshSplines;

        // TODO ADD TAG BASED ROTATION

        //TerrainTile.OnTileApplied -= OnTileAppliedRotateTowardsTag;
        //TerrainTile.OnTileApplied += OnTileAppliedRotateTowardsTag;



        TerrainTile.OnAllComplete -= OnAllTilesGenerated;
        TerrainTile.OnAllComplete += OnAllTilesGenerated;

        tgos = Component.FindObjectOfType<TownGlobalObjectService>();

        TerrainTile.OnTileMoved -= OnTileMoved;
        TerrainTile.OnTileMoved += OnTileMoved;

    }

    //List<Transform> RotationTransformLists;

    //private void OnTileAppliedRotateTowardsTag(TerrainTile tile, TileData data, StopToken stop)
    //{

    //    // The target marker.
    //private Transform target;

    //// Angular speed in radians per sec.
    //// public float speed = 1.0f;

    //var objs = GameObject.FindGameObjectsWithTag("RotationMe");
    //int attempts = 0;
    //private readonly int maxAttempts = 10;

    //public int Attempts { get => attempts; set => attempts = value; }

    //}

    //private void FindFocus()
    //{
    //    GameObject center = GameObject.FindGameObjectWithTag("RotationFocus");



    //    if (center == null && attempts < maxAttempts)
    //    {
    //        Invoke("FindFocus", 2f);
    //        return;
    //    }
    //    if (maxAttempts == attempts)
    //    {
    //        Debug.Log("placing by rotation failed for " + transform.name);
    //        return;
    //    }

    //    target ??= center.transform;


    //    foreach (var item in objs)
    //    {

    //        // Determine which direction to rotate towards
    //        Vector3 targetDirection = target.position - item.transform.position;

    //        // The step size is equal to speed times frame time.
    //        // float singleStep = speed * Time.deltaTime;

    //        // Rotate the forward vector towards the target direction by one step
    //        // Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
    //        // Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 360f, 0.0f);

    //        // Draw a ray pointing at our target in
    //        //  Debug.DrawRay(transform.position, newDirection, Color.red);

    //        // Calculate a rotation a step closer to the target and applies rotation to this object
    //        // transform.rotation = Quaternion.LookRotation(newDirection);

    //        item.transform.rotation = Quaternion.LookRotation(targetDirection);

    //    }
    //}

    private void OnTileMoved(TerrainTile tile)
    {

        //for (int i = tile.transform.childCount-1; i > 0 ; i--)
        //{

        //    if (tile.transform.GetChild(i).name.StartsWith("__SPLINE__"))
        //        DestroyImmediate(tile.transform.GetChild(i).gameObject);

        //}

        for (int i = tile.transform.childCount - 1; i > 0; i--)
        {

            if (tile.transform.GetChild(i).name.StartsWith("__SPLINE__"))
                DestroyImmediate(tile.transform.GetChild(i).gameObject);

        }

        if (TownGlobalObject.splinesNodesDataForTile.ContainsKey(tile.coord))
        {

            if (!TownGlobalObject.splinesNodesDataForTile.TryRemove(tile.coord, out var ret))
            { Debug.LogError("remove spline failed"); };

        }

    }



    void Update()
    {
        ++execs;
        if (execs % SkimpFactor == 0)
        {
            execs = 0;

            if (doDeactivate)
            {
                doDeactivate = false;
                RenderingMessage.gameObject.SetActive(false);
                CancelInvoke("deactivateTimeout");
            }

            if (doActivate)
            {
                doActivate = false;
                RenderingMessage.gameObject.SetActive(true);

                // here we should set the timeout.

                Invoke("deactivateTimeout", 60f);

            }
        }
    }

    private void deactivateTimeout() {


        doDeactivate = false;
        RenderingMessage.gameObject.SetActive(false);

    }

    public static bool doActivate = false;
    public static bool doDeactivate = false;


    public static void OnAllTilesGenerated(MapMagicObject mapMagic)
    {
        doDeactivate = true;


        Coord newPositionAsCoord = new Coord((int)(thingToTrackReference.position.x * 0.001), (int)(thingToTrackReference.position.z * 0.001));
   
         SetPlaceName(newPositionAsCoord);


        if (TownGlobalObject.TownWaitingToRender)
        {
            TownGlobalObject.TownWaitingToRender = (TownGlobalObject.TownsWaitingToRender.Count == 0) ? false : true;

            foreach (var item in TownGlobalObject.TownsWaitingToRender.ToArray())
            {

                //Debug.LogFormat(
                //    "{1} chosen for position {0} against {2} and {3}",
                //    tgos.ObjectToTrackPositionInTileCoords,
                //    tgos.LocalTownCoords,
                //    item.name,
                //    TownGlobalObject.bundles[tgos.LocalTownCoords].name);

                ///   if (item.name == TownGlobalObject.bundles[tgos.LocalTownCoords].name  &&
                ///   
                if (!TownGlobalObject.renderedTowns.Contains<Coord>(item.options.coord)) //  tgos.LocalTownCoords))
                {



                    GameObject hold = item.Generate();
                    TownGlobalObject.renderedTowns.Add(item.options.coord);


                }
            }


            //for (int i = 0; i < TownGlobalObject.TownsWaitingToRender.Count; i++)
            //{
            //    TownGlobalObject.TownsWaitingToRender.Dequeue().Generate();

            //}


            //  Debug.Log("Generate");
        }



    }

    public static void OnBeforeTileGenerated(TerrainTile tile, TileData data, StopToken stop)
    {
        doActivate = true;

        //for (int i = tile.transform.childCount - 1; i > 0; i--)
        //{

        //    if (tile.transform.GetChild(i).name.StartsWith("__SPLINE__"))
        //        DestroyImmediate(tile.transform.GetChild(i).gameObject);

        //}

    }


    public static void SetPlaceName(Coord coord) {

      
        DistrictMessageRefGameObject.GetComponent<TMPro.TextMeshProUGUI>().text = ReturnTownName(coord);
       
 
     }

    public static string ReturnTownName(Coord coord) {

        Coord locality = TownGlobalObject.GetIndexAtCoord(coord);

      return  TownGlobalObject.townsData[locality].name;
    }

    public static void OnTileAppliedRenderMeshSplines(TerrainTile tile, TileData data, StopToken stop)
    {

        /*    
         
         IN THE NEXT SECTION WE HANDLE THE SPLINE MESH RENDERING
      
         
         */

        //TODO: Just make a parent holder for all splines on a tile and delete THAT


        //for (int i = tile.transform.childCount - 1; i > 0; i--)
        //{

        //    if (tile.transform.GetChild(i).name.StartsWith("__SPLINE__"))
        //        DestroyImmediate(tile.transform.GetChild(i).gameObject);

        //}

        // we only do this on Main pass and when we actually have made something to process.
        if (data.isDraft)// || !TownGlobalObject.splinesNodesDataForTile.ContainsKey(data.area.Coord))
            return;

        //  int numberOfSplineNodes = 0;


        if (!TownGlobalObject.splinesNodesDataForTile.ContainsKey(data.area.Coord))
        {
            // this is not a mesh spline tile
            return;
        }

        // By this point this should absolutely exist - unless there is no spline data!
        int    totalNumberOfListsOfSplineMeshSplines = TownGlobalObject.splinesNodesDataForTile[data.area.Coord].Count;

        // There is nothing in the list
        if (totalNumberOfListsOfSplineMeshSplines == 0)
        {
            return;
        }


        //public void Test(ConcurrentDictionary<string, int> dictionary)
        //{
        //    dictionary.AddOrUpdate("key", 1, (key, current) => current += 2);
        //}



        //  TownHolder.Instance.SplineMeshWallMasterSpawner.SetActive(false);

        List<GameObject> thingsToActivate = new List<GameObject>();

      

        Coord locality = TownGlobalObject.GetIndexAtCoord(data.area.Coord);

     

        // Coord offset = locality - data.area.Coord;

        Coord offset = data.area.Coord;

        // SplinePowerExtended

        var DynamicHolder = TownHolder.Instance.MapMagicObjectReference.transform.Find(string.Format("Tile {0},{1}", offset.x, offset.z));


        // For like the 4th time we check this  TODO: Make it part of the Town Instancing
        TownGlobalObject.townsData[locality].TownGameObject ??= new GameObject(TownGlobalObject.townsData[locality].name);


        //create or use the holder now it has the right name.
        var go = TownGlobalObject.townsData[locality].TownGameObject;
    
        

        // We walk over the nodes assuming pairs?

            for (int i = 0; i < totalNumberOfListsOfSplineMeshSplines; i++)
            {
            //SplineMesh.Spline();
            TypedSpline   newValue = TownGlobalObject.GetSplineList(data.area.Coord)[i];

            // No splines for us...
            if (newValue.nodes.Count ==0)
            {
                continue;
            }

          //  Transform child = null;
                        

                            // TODO make this an actual hash and shove it in a table
                           // string hash = string.Format("{0}_{1}_{4}_{5}|{2}_{3}", startvec.x, startvec.y, startvec.z, endvec.x, endvec.y, endvec.z);
                          //  string fullhash = string.Format("{0}_{1}|{4}_{5}|{2}_{3}", startvec.x, startvec.y, startvec.z, endvec.x, endvec.y, endvec.z);
                            string hash = string.Format("__SPLINE__{0}__{2}|{3}__{5}", newValue.nodes[0].Position.x, newValue.nodes[0].Position.y, newValue.nodes[0].Position.z, newValue.nodes[newValue.nodes.Count-1].Position.x, newValue.nodes[newValue.nodes.Count - 1].Position.y, newValue.nodes[newValue.nodes.Count - 1].Position.z);


                        // TODO replace this with a lookup  SOOOOON
                       

                        //foreach (Transform children in DynamicHolder.parent)
                        //{
                        //   // if(child.IsNull())
                        //    child ??= children.Find(hash);
                        //}

                        //// we already rendered this  remove the gmeobject then and do it again.?
                        //if (child !=null)
                        //    {
                        //        var code = child.GetComponent<SplineMesh.Spline>();
                        //        bool wipeit = false;
                        //        for (int nodeat = 0; nodeat < code.nodes.Count; nodeat++)
                        //        {

                        //            var thisnode = code.nodes[nodeat];
                        //            if (thisnode.Position.y == 0)
                        //            {
                        //                wipeit = true;
                        //            }
                        //        }

                        //        if (wipeit)
                        //        {
                        //            DestroyImmediate(child.gameObject);
                        //        }
                        //        else
                        //        {
                        //            continue;
                        //        } 

                        //    }

            var newSpline = ((objectRendered)newValue.chosenType) switch
            {
                objectRendered.wall => GameObject.Instantiate(TownHolder.Instance.SplineMeshWallMasterSpawner, DynamicHolder, true),
                objectRendered.fence => GameObject.Instantiate(TownHolder.Instance.SplineMeshFenceMasterSpawner, DynamicHolder, true),
                objectRendered.gatehouse => GameObject.Instantiate(TownHolder.Instance.SplineMeshGatehouseMasterSpawner, DynamicHolder, true),
                _ => GameObject.Instantiate(TownHolder.Instance.SplineMeshWallMasterSpawner, DynamicHolder, true),
            };

            newSpline.name = hash+ ((objectRendered)newValue.chosenType).ToString() ;

            newSpline.GetComponent<SplineMesh.Spline>().nodes = newValue.nodes;


            if (newValue.tryToFloor)
            {
                var scrp = newSpline.AddComponent<AlignNodesToTerrainOnEnable>();

              //  scrp.checkVal = 0.07f;
            }

            thingsToActivate.Add(newSpline);


            // mark it as finally processed. but only if we havent already.

        //  if (!TownGlobalObject.isSplinesMeshRenderedOnTile.ContainsKey(data.area.Coord))
         //       TownGlobalObject.isSplinesMeshRenderedOnTile.Add(data.area.Coord, true);


        }

            // Activate them as a group.
        for (int i = 0; i < thingsToActivate.Count; i++)
        {
            thingsToActivate[i].SetActive(true);
        }
     
    }
}
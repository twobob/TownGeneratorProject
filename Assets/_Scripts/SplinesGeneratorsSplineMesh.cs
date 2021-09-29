using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Den.Tools;
using Den.Tools.Splines;
using Den.Tools.Matrices;
using Den.Tools.GUI;
using MapMagic.Core;
using MapMagic.Products;
using MapMagic.Nodes;

namespace Twobob.Mm2
{
    

    [System.Serializable]
    [GeneratorMenu(
         menu = "Spline/Modifiers",
         name = "SplineMesh",
         iconName = "GeneratorIcons/Constant",
         colorType = typeof(SplineSys),
         disengageable = true,
         helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]

    public class SplineMeshControl : Generator, IInlet<SplineSys>, IOutlet<SplineSys>
    {

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(SplineMeshControl));
#endif

        [Val("Input", "Inlet")] public readonly Inlet<SplineSys> splineIn = new Inlet<SplineSys>();
        //  [Val("Spline", "Height")] public readonly Inlet<MatrixWorld> heightIn = new Inlet<MatrixWorld>();



        [Val("Output", "Outlet")] public readonly Outlet<SplineSys> output = new Outlet<SplineSys>();


        [Val("Type")] public objectRendered chosenType = objectRendered.wall;


        [Val("TryToFloor")] public bool TryToFloor = false;


        public IEnumerable<IInlet<object>> Inlets() { yield return splineIn; }// yield return heightIn; }


        public enum objectRendered
        {
            wall,fence,gatehouse
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Type Safety", "UNT0010:MonoBehavior instance creation is not recommended", Justification = "<Pending>")]
        public override void Generate(TileData data, StopToken stop)
        {
            Coord locality = TownGlobalObject.GetIndexAtCoord(data.area.Coord);

            if (TownGlobalObject.townsData[locality] == null)
                return;

            //SplineSys src = data.ReadInletProduct(splineIn);
            SplineSys src = data.ReadInletProduct(this);
            // MatrixWorld heights = data.ReadInletProduct(heightIn);
            if (src == null) return;

            if (!enabled)// || heights == null) 
            { data.StoreProduct(this, src); return; }


            if (stop != null && stop.stop) return;
            SplineSys dst = new SplineSys(src);

            // at this point cleanse. it should be clean. but clean anyway?
            dst.Clamp((Vector3)data.area.active.worldPos, (Vector3)data.area.active.worldSize);

        
            // WE only do main stage
            if (data.isDraft) 
            { data.StoreProduct(this, dst); return; }

            if (dst.lines.Length == 0)
            {
                // null. there are no segments to add.
                //  TownGlobalObject.GetSplineList(data.area.Coord);
                data.StoreProduct(this, dst);
                return;
            }
            // this is already completed processed by all threads /n/ times and marked as done in OnTileApplied()
            else if (TownGlobalObject.isSplinesMeshRenderedOnTile.ContainsKey(data.area.Coord))
                return;
                
            //{
            //    // we already processed the tile.
            //    data.StoreProduct(this, dst);
            //    return;
            //}


            // these might not be linked but probably are???
            foreach (var item in dst.lines)
            {

                var subdic = new Dictionary<Coord, List<Vector3>>();
                var sublist = new List<SplineMesh.Spline>();

                // track to ensure we have at a minimum two nodes...
                int nodesInThisSegmentGroup = 0;


                // use this rather than the monobehaviour
                TypedSpline typed = new TypedSpline();

             //   SplineMesh.Spline mysm = new SplineMesh.Spline();

                // these should be linked  - Hopefully this will pickup on a partially aborted WHILE

                for (int segmentindex = 0; segmentindex < item.segments.Length; segmentindex++)
                {
                    // They will be in pairs by now. we need to do the logic to string them back together, here.

                    Segment segmentToProcess = item.segments[segmentindex];

                    // nodesInThisSegmentGroup = AddNodeReturnIncrementedTotalCount(data, nodesInThisSegmentGroup, segmentToProcess, mysm);

                    nodesInThisSegmentGroup = AddNodeToListReturnIncrementedTotalCount(nodesInThisSegmentGroup, segmentToProcess, typed.nodes);

                    segmentindex++;


                }
                typed.chosenType = (int)chosenType;
                typed.tryToFloor = TryToFloor;

                //update a value
                if (TownGlobalObject.splinesNodesDataForTile.ContainsKey(data.area.Coord))
                {
                    TownGlobalObject.splinesNodesDataForTile[data.area.Coord].Add(typed);
                }
                else
                {// create a new one
                    TownGlobalObject.GetSplineList(data.area.Coord).Add(typed);
                }
                //TownGlobalObject.splinesNodesDataForTile.AddOrUpdate(data.area.Coord, TownGlobalObject.GetSplineList(data.area.Coord), (key, current) => { current.Add(typed);  return current; }   );  

              //  TownGlobalObject.GetSplineList(data.area.Coord).Add(typed);


                // here we would loop if the segmentindex has not been reached.
                // might have to watch for weird single node edge cases but oh well. Bridge. Come to it.
            }

            // We just store the original.

         //   data.StoreProduct(this, src);

            // We store TRUNCATED copy.

            data.StoreProduct(this, dst);
        }

        private static int AddNodeReturnIncrementedTotalCount(TileData data, int nodesInThisSegmentGroup, Segment segmentToProcessPos, SplineMesh.Spline mysm)
        {
            // if (data.area.active.Contains(segmentToProcessPos))
            // {

            mysm.AddNode(new SplineMesh.SplineNode(segmentToProcessPos.start.pos, segmentToProcessPos.start.pos));
            mysm.AddNode(new SplineMesh.SplineNode(segmentToProcessPos.end.pos, segmentToProcessPos.end.pos));
            nodesInThisSegmentGroup += 2;
            // }

            return nodesInThisSegmentGroup;
        }


        private static int AddNodeToListReturnIncrementedTotalCount(int nodesInThisSegmentGroup, Segment segmentToProcessPos, List<SplineMesh.SplineNode> mysm)
        {
            
            mysm.Add(new SplineMesh.SplineNode(segmentToProcessPos.start.pos, segmentToProcessPos.start.pos));
            mysm.Add(new SplineMesh.SplineNode(segmentToProcessPos.end.pos, segmentToProcessPos.end.pos));
            nodesInThisSegmentGroup += 2;
           
            return nodesInThisSegmentGroup;
        }

    }


  
}
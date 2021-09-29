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
using System.Linq;

namespace MapMagic.Nodes.SplinesGenerators
{

    [System.Serializable]
    [GeneratorMenu(
    menu = "Spline/Initial",
    name = "FromObjects",
    iconName = "GeneratorIcons/Constant",
    colorType = typeof(SplineSys),
    disengageable = true,
    helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/spline")]
    public class AutoSplinesFromObjects : Generator, IInlet<TransitionsList>, IOutlet<SplineSys>
    {

        [Val("Positions", "Positions")] public readonly Inlet<TransitionsList> input = new Inlet<TransitionsList>();

        [Val("Output", "Outlet")] public readonly Outlet<SplineSys> output = new Outlet<SplineSys>();


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(AutoSplinesFromObjects));
#endif

        public void RemoveAnyBlanksInTransitionListArrayHelper(ref TransitionsList list)
        {
            list.arr = list.arr.Truncated(list.count);

        }
         
        public override void Generate(TileData data, StopToken stop)
        {
            

            if (!enabled) return;
            // if (data.isDraft) return;

            TransitionsList src = data.ReadInletProduct<TransitionsList>(this);

            TransitionsList copy = new TransitionsList(src);
            RemoveAnyBlanksInTransitionListArrayHelper(ref copy);

            // nodes for spline
            List<Vector3> markers = new List<Vector3>(copy.count);

            // setup the clamp mask
            var tileLocation = data.area.Coord.ToVector3(1000); // x 0 z
            var tileLocationV2 = tileLocation.V2(); // x z
            var tileSize = new Vector3(1000, 500, 1000);  // TODO: Set this to the mapmagic tile size... not assume 1k

            // now magically create perfect size slices for this tile.  Thanks Denis.
            // dst.Clamp(tileLocation, tileSize);

            //data - whatever data
            foreach (var item in copy.arr)
            {

                Vector2 offsetted = (item.pos.V2() + tileLocationV2);


                var offsettedstore = new Vector3(offsetted.x, item.pos.y, offsetted.y);

                if (!markers.Contains(offsettedstore))

                    try
                    {
                        markers.Add(offsettedstore);
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat(" Edge case {0} with location {1},{2},{3} and a list of Length {4}", e.Message, offsettedstore.x, offsettedstore.y, offsettedstore.z, markers.Count);
                        // ignore this weird edge case.
                    }

            }


            // add the first one again, as a node. for a loop. and to ensure we have two.. which we will now test for..
            markers.Add(markers[0]);

            // there was one object ONLY...
            if (markers[0] == markers[1])
            {
                // this will just give is an N/A rather than bombing.
                Debug.LogErrorFormat(" Please add at least TWO objects");
                return;
            }

            // make some holders
            SplineSys spline = new SplineSys();
            Line line = new Line();
            line.SetNodes(markers.ToArray());
            spline.AddLine(line);


            // now magically create perfect size slices for this tile.  Thanks Denis.
            spline.Clamp(tileLocation, tileSize);


            //save it.
            data.StoreProduct(this, spline);
        }
    }
}


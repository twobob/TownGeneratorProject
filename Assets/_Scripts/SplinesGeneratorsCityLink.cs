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
        menu = "Spline/Standard",
        name = "CityLink",
        iconName = "GeneratorIcons/Constant",
        colorType = typeof(SplineSys),
        disengageable = true,
        helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]
    public class CityLink : Generator, IOutlet<SplineSys>
    {
        [Val("Input", "Inlet")] public readonly Outlet<SplineSys> input = new Outlet<SplineSys>();
        [Val("Output", "Outlet")] public readonly Outlet<SplineSys> output = new Outlet<SplineSys>();


#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(CityLink));
#endif


        public override void Generate(TileData data, StopToken stop)
        {
            if (!enabled) return;
            // if (data.isDraft) return;

            // nodes for spline
            List<Vector3> markers = new List<Vector3>(); //TownGlobalObjectService.TownRequests.Count + TownInitService.__totalCities + 1);
            //data - whatever data
            foreach (var subtown in TownGlobalObject.townsData)
            {
                Town.Geom.Vector2 offsetted = (subtown.Value.Center + subtown.Value.townOffset);

                var offsettedstore = new Vector3(offsetted.x, 499f, offsetted.y);

                if (!markers.Contains(offsettedstore))

                        markers.Add(offsettedstore);
                   

            }
           

            // add the first one again, as a node. for a loop.
            markers.Add(markers[0]);


            foreach (var subtown in TownGlobalObject.townsData.Reverse())
            {          

                foreach (var road in subtown.Value.Roads)
                {
                    Town.Geom.Vector2 offsettedroad = new Town.Geom.Vector2(
                        road[road.Count - 1].x * TownGlobalObjectService.WorldMultiplier + subtown.Value.townOffset.x,
                         road[road.Count - 1].y * TownGlobalObjectService.WorldMultiplier + subtown.Value.townOffset.y
                        );

                    var store = new Vector3(offsettedroad.x, 499f, offsettedroad.y);

                    if (!markers.Contains(store))

                    markers.Add(store);

                }
            }

           

            // make some holders
            SplineSys spline = new SplineSys();
            Line line = new Line();
            line.SetNodes(markers.ToArray());
            spline.AddLine(line);

            // setup the clamp mask
           var tileLocation = data.area.Coord.ToVector3(1000);
           var tileSize = new Vector3(1000, 500, 1000);

            // now magically create perfect size slices for this tile.  Thanks Denis.
            spline.Clamp(tileLocation, tileSize);


            //save it.
            data.StoreProduct(this, spline);
        }
    }
}




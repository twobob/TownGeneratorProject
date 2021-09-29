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
menu = "Spline/Modifiers",
name = "WigglerMini",
iconName = "GeneratorIcons/Constant",
colorType = typeof(SplineSys),
disengageable = true,
helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]
    public class WigglerMini : Generator, IInlet<SplineSys>, IOutlet<SplineSys>
    {
        [Val("Input", "Inlet")] public readonly Inlet<SplineSys> input = new Inlet<SplineSys>();

        [Val("Output", "Outlet")] public readonly Outlet<SplineSys> output = new Outlet<SplineSys>();


        /// <summary>
        /// clampInput
        /// </summary>
        [Val("ClampIN?")] public bool clampIn = true;

        [Val("Wiggly")] public float wiggliness = 1f;

        [Val("Divisions")] public int divisions = 4;

        /// <summary>
        /// Make the random "repeatable"
        /// </summary>
        [Val("Repeatable?")] public bool useNoise = true;

        /// <summary>
        /// use this to get unique patterns without changing anything else.
        /// </summary>
        [Val("ImaginaryPart")] public float FractalStep = 1f;



        /// <summary>
        /// scale the wiggly factor
        /// </summary>
        [Val("AutoScale?")] public bool AutoScale = true;


        /// <summary>
        /// clampInput
        /// </summary>
        [Val("ClampOUT?")] public bool clampOUT = true;

        /// <summary>
        /// Not entirely sure...
        /// </summary>
        //  [Val("NodeType?")] public Node.TangentType nodeType = Node.TangentType.auto;

        /// [Val("Bendy?")] public bool doBendy = false;

        //  [Val("Bendiness")] public float bendiness = 1f;

        //   [Val("Relax?")] public bool doRelax = false;

        //  [Val("RelaxIterations")] public int ri = 4;

        //  [Val("RelaxBlur")] public float blur = 1f;


        private Vector2 Full_I_Value = Vector2.zero;

        private float scalar = 1;

        private float averageLength = 1000f;

        private float ActualLength = 0f;

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(WigglerMini));
#endif 

        public override void Generate(TileData data, StopToken stop)
        {
            if (divisions < 1)
            {
                divisions = 1;
            }



            SplineSys src = data.ReadInletProduct(this);

            if (src==null)
            {
                return;
            }

            SplineSys dst = new SplineSys(src);

            if (src == null) return;

            if (!enabled)
            {
                data.StoreProduct(this, src);
                return;
            }

            // setup the clamp mask
            var tileLocation = data.area.Coord.ToVector3(1000);
            var tileSize = new Vector3(1000, 500, 1000);

            // now magically create perfect size slices for this tile.  Thanks Denis.

            if (clampIn) dst.Clamp(tileLocation, tileSize);

            // avoid non offsets for our imaginary pair.
            if (FractalStep == 0)
                FractalStep = 1;

            // setup the imaginary offset
            Full_I_Value = new Vector2(FractalStep, FractalStep);

           
            /// if (data.isDraft) return;
            dst.Subdivide(divisions);


            
            foreach (var item in dst.lines)
            {

                ActualLength = item.segments.Select(x => x.StartEndLength).Sum();

                if (AutoScale)
                {
                    scalar = averageLength / ActualLength;
                }


                for (int i = 1; i < item.NodesCount - 1; i++)
                {

                    var cur = item.GetNodePos(i);
                    var nv = Vector3.zero;
                    var newFull_I_Value = new Vector3(Full_I_Value.x, 0, Full_I_Value.y);
                    var loc = new Vector3(cur.x, 0, cur.y);
                    
                    if (useNoise)
                    {
                         nv = ReturnWigglyVector3UsingPerlinNoise(wiggliness, cur);
                        // sanity check.

                       // nv = new Vector3(cur.x + ReturnPerlinNoiseValueAtSpot(wiggliness, cur), cur.y, cur.z + ReturnPerlinNoiseValueAtSpot(wiggliness, cur + newFull_I_Value));

                       // var holder = new Vector3(cur.x + ReturnWiggly(wiggliness), cur.y, cur.z + ReturnWiggly(wiggliness));
                    }
                    else
                    {
                        nv = new Vector3(cur.x + ReturnWiggly(wiggliness), cur.y, cur.z + ReturnWiggly(wiggliness));
                      //  var holder = new Vector3(cur.x + ReturnPerlinNoiseValueAtSpot(wiggliness, cur), cur.y, cur.z + ReturnPerlinNoiseValueAtSpot(wiggliness, cur + newFull_I_Value));

                    }
                    item.SetNodePos(i, nv);
                }
            }



            //if (doRelax)
            //{
            //    dst.Relax(blur, ri);
            //}

            // bend nodes

            //if (doBendy)
            //{
            //    for (int i = 0; i < dst.lines.Length; i++)
            //    {
            //        for (int j = 0; j < dst.lines[i].segments.Length; j++)
            //        {
            //            // Skip the very first two.
            //            if (j > 0)
            //            {

            //                Node start = dst.lines[i].segments[j].start;
            //                Vector2 startpos = new Vector2(start.pos.x, start.pos.z);


            //                if (useNoise)
            //                {
            //                    Vector3 place = ReturnWigglyVector3UsingPerlinNoise(bendiness, startpos.V3());

            //                    start.dir =
            //                            (FlipALocationCoin(startpos)) ?
            //                          start.dir + place :
            //                          start.dir - place;

            //                }
            //                else
            //                {
            //                    Vector3 place = ReturnWigglyVector3(bendiness);

            //                    start.dir =
            //                         (FlipALocationCoin(startpos)) ?
            //                       start.dir + place :
            //                       start.dir - place;
            //                }


            //                start.type = nodeType;
            //            }
            //            // Skip the lasties
            //            if (j < dst.lines[i].segments.Length )
            //            {

            //                Node end = dst.lines[i].segments[j].end;
            //                Vector2 endpos = new Vector2(end.pos.x, end.pos.z);

            //                if (useNoise)
            //                {
                               
            //                    Vector3 place = ReturnWigglyVector3UsingPerlinNoise(bendiness, endpos.V3());

            //                    end.dir =
            //                            (FlipALocationCoin(endpos)) ?
            //                          end.dir + place :
            //                          end.dir - place;
            //                }
            //                else
            //                {

            //                    Vector3 place =
            //                    ReturnWigglyVector3(bendiness);
            //                   end.dir =  (FlipALocationCoin(endpos)) ?
            //                   end.dir + place :
            //                   end.dir - place;
            //                }

            //                end.type = nodeType;
            //            }
            //        }
            //    }
            //}

            //if (useNoise)
            //{
            //    for (int i = 0; i < dst.lines.Length; i++)
            //    {
            //        for (int j = 0; j < dst.lines[i].segments.Length; j++)
            //        {
            //            // Skip the very first.
            //            if (j > 0)
            //            {

            //                Node start = dst.lines[i].segments[j].start;
            //                Vector2 startpos = new Vector2(start.pos.x, start.pos.z);
            //                Vector3 mag = ReturnWigglyVector3UsingPerlinNoise(bendiness, startpos.V3());

            //                start.dir =
            //                     (FlipALocationCoin(startpos)) ?
            //                   start.dir + mag :
            //                   start.dir - mag ;

            //                start.type = nodeType;
            //            }
            //            // Skip the last
            //            if (j < dst.lines[i].segments.Length - 1)
            //            {

            //                Node end = dst.lines[i].segments[j].end;
            //                Vector2 endpos = new Vector2(end.pos.x, end.pos.z);
            //                Vector3 mag = ReturnWigglyVector3UsingPerlinNoise(bendiness, endpos.V3());

            //                end.dir =
            //                    (FlipALocationCoin(endpos)) ?
            //                   end.dir + mag :
            //                   end.dir - mag;

            //                end.type = nodeType;
            //            }
            //        }
            //    }
            //}


            // setup the clamp mask
          //  var tileLocation = data.area.Coord.ToVector3(1000);
           // var tileSize = new Vector3(1000, 500, 1000);

            // now magically create perfect size slices for this tile.  Thanks Denis.


            if(clampOUT) dst.Clamp(tileLocation, tileSize);

            data.StoreProduct(this, dst);

        }

        // persistence?
        private bool FlipALocationCoin(Vector2 location)
        {
            
            return ReturnPerlinNoiseValueAtSpot(1, location) < 0.5f;
        }

        private bool FlipACoin()
        {
            // to make it easy to replace with 
            //    return (UnityEngine.Random.value < 0.5f);

            return RandomGen.FlipACoin();

        }


        private Vector3 ReturnWigglyVector3UsingPerlinNoise(float factor, Vector3 location)
        {

          //  var loc = new Vector3(location.x, 0, location.y);
            var newFull_I_Value = new Vector3(Full_I_Value.x, 0, Full_I_Value.y);

         //   new Vector3(cur.x + ReturnPerlinNoiseValueAtSpot(wiggliness, loc), cur.y, cur.z + ReturnPerlinNoiseValueAtSpot(wiggliness, loc + newFull_I_Value));



            var ret =

             new Vector3(
                ReturnPerlinNoiseValueAtSpot(factor, location),
                0,
                 ReturnPerlinNoiseValueAtSpot(factor, location + newFull_I_Value)
                 );

            var temp = ret + location;


            return temp ;



        }



        private float ReturnPerlinNoiseValueAtSpot(float factor, Vector2 location)
        {

            var ret = ((Mathf.PerlinNoise(location.x, location.y) - 0.5f) *2) * factor * scalar;


            //// do a true / false coin flip.
            //if (FlipALocationCoin(location))

            //    return -ret;
            //else
                return ret;
        }


        private float ReturnWiggly(float factor)
        {
            // to  simplify changing the random here is an example.
            //   return (((UnityEngine.Random.value - 0.5f) * 2) * factor);
            return RandomGen.Next(10, -10) * 0.1f * factor * scalar;
        }

        private Vector3 ReturnWigglyVector3(float factor)
        {

            return new Vector3(RandomGen.Next(10, -10) * 0.1f * factor * scalar, 0, RandomGen.Next(10, -10) * factor * scalar);
        }


        //private Vector3 ReturnWigglyVector3UsingFractalNoise(float factor, Vector2 location, Noise randomnoise)
        //{

        //    return new Vector3(
        //        ReturnFractalNoiseValueAtSpot(factor, location, randomnoise),
        //        0,
        //         ReturnFractalNoiseValueAtSpot(factor, location + Full_I_Value, randomnoise)
        //         );
        //}




        //private Vector3 ReturnWigglyVector3UsingRandomNoise(float factor, Vector2 location, Noise randomnoise)
        //{


        //    return new Vector3(
        //        ReturnRandomNoiseValueAtSpot(factor, location, randomnoise),
        //        0,
        //         ReturnRandomNoiseValueAtSpot(factor, location + Full_I_Value, randomnoise)
        //         );
        //}


        //private float ReturnRandomNoiseValueAtSpot(float factor, Vector2 location, Noise randomnoise)
        //{


        //    // do a true / false coin flip.
        //    if (FlipACoin())
        //        return randomnoise.Perlin(location.x, location.y) * factor;
        //    else
        //        return -randomnoise.Perlin(location.x, location.y) * factor;

        //}


        //private float ReturnFractalNoiseValueAtSpot(float factor, Vector2 location)
        //{

        //    var FractalType = 2;
        //    var iterations = 1;
        //    var detail = 0.5f;
        //    var turbulence = 0f;

        //    var ret = (randomnoise.Fractal(location.x, location.y, 1, iterations, detail, turbulence, FractalType)
        //        * 2 - 1) * 1 * factor;

        //    // do a true / false coin flip.
        //    if (FlipACoin())

        //        return -ret;
        //    else
        //        return ret;
        //}


    }
}

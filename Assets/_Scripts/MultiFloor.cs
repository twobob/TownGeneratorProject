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


namespace MapMagic.Nodes.MatrixGenerators
{

    public class MultiFloor
    {

        [System.Serializable]
        [GeneratorMenu(menu = "Spline/Standard", name = "MultiFloor", iconName = null, disengageable = true, helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]
        public class MultiFloor200 : Generator, IMultiInlet, IMultiOutlet // IOutlet<SplineSys>
        {
            [Val("Spline", "Height")]
            public readonly Inlet<MatrixWorld> heightIn = new Inlet<MatrixWorld>();
            [Val("Inlet1Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline1In = new Inlet<SplineSys>();
            [Val("Inlet2Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline2In = new Inlet<SplineSys>();
            [Val("Inlet3Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline3In = new Inlet<SplineSys>();
            [Val("Inlet4Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline4In = new Inlet<SplineSys>();
            [Val("Inlet5Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline5In = new Inlet<SplineSys>();
            [Val("Inlet6Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline6In = new Inlet<SplineSys>();
            [Val("Inlet7Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline7In = new Inlet<SplineSys>();
            [Val("Inlet8Spline", "Inlet")]
            public readonly Inlet<SplineSys> spline8In = new Inlet<SplineSys>();

            [Val("Out1Spline", "Out1")]
            public readonly Outlet<SplineSys> spline1Out = new Outlet<SplineSys>();
            [Val("Out2Spline", "Out2")]
            public readonly Outlet<SplineSys> spline2Out = new Outlet<SplineSys>();
            [Val("Out3Spline", "Out3")]
            public readonly Outlet<SplineSys> spline3Out = new Outlet<SplineSys>();
            [Val("Out4Spline", "Out4")]
            public readonly Outlet<SplineSys> spline4Out = new Outlet<SplineSys>();
            [Val("Out5Spline", "Out1")]
            public readonly Outlet<SplineSys> spline5Out = new Outlet<SplineSys>();
            [Val("Out6Spline", "Out2")]
            public readonly Outlet<SplineSys> spline6Out = new Outlet<SplineSys>();
            [Val("Out7Spline", "Out3")]
            public readonly Outlet<SplineSys> spline7Out = new Outlet<SplineSys>();
            [Val("Out8Spline", "Out4")]
            public readonly Outlet<SplineSys> spline8Out = new Outlet<SplineSys>();


            public IEnumerable<IInlet<object>> Inlets() {
                yield return heightIn;
                yield return spline1In; yield return spline2In; yield return spline3In; yield return spline4In;
                yield return spline5In; yield return spline6In; yield return spline7In; yield return spline8In;
            }
            public IEnumerable<IOutlet<object>> Outlets() {
                yield return spline1Out; yield return spline2Out; yield return spline3Out; yield return spline4Out;
                yield return spline5Out; yield return spline6Out; yield return spline7Out; yield return spline8Out;
            }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(MultiFloor200));
#endif


            public override void Generate(TileData data, StopToken stop)
            {
                ProcessToFloor(spline1In, spline1Out, data, stop);
                ProcessToFloor(spline2In, spline2Out, data, stop);
                ProcessToFloor(spline3In, spline3Out, data, stop);
                ProcessToFloor(spline4In, spline4Out, data, stop);
                ProcessToFloor(spline5In, spline5Out, data, stop);
                ProcessToFloor(spline6In, spline6Out, data, stop);
                ProcessToFloor(spline7In, spline7Out, data, stop);
                ProcessToFloor(spline8In, spline8Out, data, stop);
            }

            public void ProcessToFloor(Inlet<SplineSys> splineIn, Outlet<SplineSys> splineOut, TileData data, StopToken stop) {

                SplineSys src = data.ReadInletProduct(splineIn);
                MatrixWorld heights = data.ReadInletProduct(heightIn);
                if (src == null) return;
                if (!enabled || heights == null) { data.StoreProduct(splineOut, src); return; }

                if (stop != null && stop.stop) return;
                SplineSys dst = new SplineSys(src);

                FloorSplines(dst, heights);
                dst.Update();

                if (stop != null && stop.stop) return;
                data.StoreProduct(splineOut, dst);

                DebugGizmos.Clear("Spline");
                foreach (Segment segment in dst.lines[0].segments)
                    DebugGizmos.DrawLine("Spline", segment.start.pos, segment.end.pos, Color.white, additive: true);

            }



            public static void FloorSplines(SplineSys dst, MatrixWorld heights)
            {
                foreach (Line line in dst.lines)
                {
                    for (int s = 0; s < line.segments.Length; s++)
                        line.segments[s].start.pos.y = FloorPoint(line.segments[s].start.pos, heights);

                    line.segments[line.segments.Length - 1].end.pos.y = FloorPoint(line.segments[line.segments.Length - 1].end.pos, heights);
                }
            }

            public static float FloorPoint(Vector3 pos, MatrixWorld heights)
            {
                if (pos.x <= heights.worldPos.x) pos.x = heights.worldPos.x + 0.001f;
                if (pos.x >= heights.worldPos.x + heights.worldSize.x) pos.x = heights.worldPos.x + heights.worldSize.x - 0.001f;

                if (pos.z <= heights.worldPos.z) pos.z = heights.worldPos.z + 0.001f;
                if (pos.z >= heights.worldPos.z + heights.worldSize.z) pos.z = heights.worldPos.z + heights.worldSize.z - 0.001f;

                float h = heights.GetWorldInterpolatedValue(pos.x, pos.z);
                return h * heights.worldSize.y;
            }
        }

    }
}
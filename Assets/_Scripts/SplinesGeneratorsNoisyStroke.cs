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

namespace MapMagic.Nodes.SplinesGenerators
{

    [System.Serializable]
    [GeneratorMenu(menu = "Spline/Standard", name = "NoisyStroke", iconName = "GeneratorIcons/Constant", disengageable = true,
        colorType = typeof(SplineSys),
        helpLink = "https://gitlab.com/denispahunov/mapmagic/wikis/map_generators/constant")]
    public class NoisyStroke : Generator, IInlet<SplineSys>, IOutlet<MatrixWorld>
    {

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void EnlistInMenu() => MapMagic.Nodes.GUI.CreateRightClick.generatorTypes.Add(typeof(NoisyStroke));
#endif

        [Val("Width")] public float width = 10;
        [Val("Hardness")] public float hardness = 0.0f;

        [Val("NoiseFalloff")] public bool noiseFallof = false;
        [Val("NoiseAmount")] public float noiseAmount = 1f;
        [Val("NoiseSize")] public float noiseSize = 20;
        // [Val("NoiseRadius")] public float radius = 1000f;

        public override void Generate(TileData data, StopToken stop)
        {
            SplineSys splineSys = data.ReadInletProduct(this);
            if (splineSys == null || !enabled) return;

            //stroking
            if (stop != null && stop.stop) return;
            MatrixWorld strokeMatrix = new MatrixWorld(data.area.full.rect, data.area.full.worldPos, data.area.full.worldSize, data.globals.height);

            Vector2D center = data.area.active.rect.Center.vector2d;

            SplineMatrixOps.Stroke(splineSys, strokeMatrix, white: true, antialiased: true);

            //spreading
            if (stop != null && stop.stop) return;
            MatrixWorld spreadMatrix = Spread(strokeMatrix, width);

            MatrixWorld noiseWorld = new MatrixWorld(spreadMatrix.rect, spreadMatrix.worldPos, spreadMatrix.worldSize);
            noiseWorld.Fill(0);

            CoordRect intersection = CoordRect.Intersected(noiseWorld.rect, spreadMatrix.rect);


            Coord min = intersection.Min; Coord max = intersection.Max;

            Coord coord = new Coord(); //temporary coord to call GetFallof

            // float falloff = 0.6f;


         //   if (noiseFallof)

            {
                for (int x = min.x; x < max.x; x++)
                    for (int z = min.z; z < max.z; z++)
                    {
                        coord.x = x;
                        coord.z = z;

                        int pos = (z - noiseWorld.rect.offset.z) * noiseWorld.rect.size.x + x - noiseWorld.rect.offset.x;  //coord.GetPos
                        if (spreadMatrix.arr[pos] < 0.00001f) continue;

                        float falloff = 1;

                        if (noiseFallof)
                        {
                            float maxNoise = 1 - spreadMatrix.arr[pos]; if (spreadMatrix.arr[pos] < 0.5f) maxNoise = spreadMatrix.arr[pos];
                            falloff += (data.random.Fractal(x, z, noiseSize) * 2 - 1) * maxNoise * noiseAmount;
                        }

                        noiseWorld.arr[pos] =
                           spreadMatrix.arr[pos]
                            * falloff;
                    }
            }

            //hardness
            if (hardness > 0.0001f)
            {
                float h = 1f / (1f - hardness);
                if (h > 9999) h = 9999; //infinity if hardness is 1

                noiseWorld.Multiply(h);
            }

            noiseWorld.Clamp01();

            if (stop != null && stop.stop) return;
            // data.StoreProduct(this, spreadMatrix);
            data.StoreProduct(this, noiseWorld);
        }



        public static MatrixWorld Spread(MatrixWorld matrix, float range)
        {
            MatrixWorld spreadMatrix;
            float pixelRange = range / matrix.PixelSize.x;

            if (pixelRange < 1) //if less than a pixel making line less noticable
            {
                spreadMatrix = matrix;
                spreadMatrix.Multiply(pixelRange);
            }

            else //spreading the usual way
            {
                spreadMatrix = new MatrixWorld(matrix);
                MatrixOps.SpreadLinear(matrix, spreadMatrix, subtract: 1f / pixelRange, diagonals: true, quarters: true);

            }

            return spreadMatrix;
        }


        public static void SpreadOrMultiply(MatrixWorld src, MatrixWorld dst, float range)
        {
            float pixelRange = range / src.PixelSize.x;

            if (pixelRange < 1) //if less than a pixel making line less noticable
                dst.Multiply(pixelRange);
            else
                MatrixOps.SpreadLinear(src, dst, subtract: 1f / pixelRange);
        }
    }
}


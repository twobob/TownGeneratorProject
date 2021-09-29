using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlignNodesToTerrainOnEnable : MonoBehaviour
{
    public int attempts = 0;
    public float checkVal = 0.005f;
    private int MaxAttempts = 20;
    SplineMesh.Spline spline; 
    void OnEnable()
    {
        spline = GetComponent<SplineMesh.Spline>();
        TryToFloor();
        //  splineFormer.InvalidateMesh();
    }

    private void TryToFloor()
    {
        // list, right length
        List<float> testArr = new List<float>();

        // Fill it with 500's

        for (int i = 0; i < spline.nodes.Count; i++)
        {
            testArr.Add( 500f);
        }

      

        int testcount = 0;

        for (int i = 0; i < spline.nodes.Count; i++)
        {
            var node = spline.nodes[i];
            testArr[i] = GetTerrainPos(node.Position.x, node.Position.z).y;
           
            node.Position = node.Direction = new Vector3(node.Position.x, testArr[i], node.Position.z);
            
             testcount += 1;
           
        }

        if (!spline.enabled)
            spline.enabled = true;


        if (testArr.Contains(0))
        {

            // Last pop get nuclear
            if (attempts > MaxAttempts - 4)
            {

                // check every single node.
                for (int i = 0; i < spline.nodes.Count; i++)
                {
                    var node = spline.nodes[i];

                    // move x closer
                    if (Mathf.Abs(node.Position.x % 1) <= (checkVal))
                    {
                        node.Position = new Vector3(Mathf.Round(node.Position.x), node.Position.y, node.Position.z);
                    }
                    // Move z closer
                    if (Mathf.Abs(node.Position.z % 1) <= (checkVal))
                    {
                        node.Position = new Vector3(node.Position.x, node.Position.y, Mathf.Round(node.Position.z));
                    }
                }

                checkVal *= 8;

            }

            if (attempts < MaxAttempts)
            {
                attempts += 1;
                Invoke(nameof(TryToFloor), 1);
                return;
            }
            else
            {
              
                Debug.LogFormat(gameObject, "Failed to floor splines {0} on countout", gameObject.name);
                return;
            }
        }

       
       
    }

    static Vector3 GetTerrainPos(float x, float y)
    {
        //Create object to store raycast data

        //Create origin for raycast that is above the terrain. I chose 500.
        Vector3 origin = new Vector3(x, 500f, y);

        //Send the raycast.
        // Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 501f);

        // TODO OPTION MASK SELECTION
        LayerMask mask = LayerMask.GetMask("Default");

        Ray ray = new Ray(origin, Vector3.down);


        Physics.Raycast(ray, out RaycastHit hit, 501f, mask);


        //  Debug.Log("Terrain location found at " + hit.point);
        return hit.point;
    }


}

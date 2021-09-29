using System.IO;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using MapMagic.Core;
using MapMagic.Terrains;

namespace Twobob
{
    public class MeshFromImage : MonoBehaviour
    {
        // For saving the mesh------------------------ 
        public KeyCode saveKey = KeyCode.F12;
        public string saveName = "SavedMesh";

        // Concerning mesher--------------------------
        public GameObject mesher; //require

        public List<Vector3> vertices;
        public List<int> triangles;

        public Vector3 point0;
        public Vector3 point1;
        public Vector3 point2;
        public Vector3 point3;

        public int loop;
        public float size;

        public Mesh meshFilterMesh;
        public Mesh meshColliderMesh;

        // Sprite work
        public Color[] pixels;

        public Texture2D newTexture;
        public Texture2D oldTexture;  //require

        private Sprite mySprite;
        private SpriteRenderer spriteRenderer;

        public int pathCount;

        public GameObject displayerComponent; //require

        public PolygonCollider2D polygonColliderAdded; //require

        private Vector3 oldPosition = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private System.Diagnostics.Stopwatch zeit = new System.Diagnostics.Stopwatch();





        public bool FindTextureByPosition(float x, float z)
        {

            bool stop = true;

            // did we already run? and did we not move in xz.

            if (oldPosition.x == x && oldPosition.z == z)
            {
                //we should have already assigned the texture. Just return
                Debug.Log("early return ");
                return stop;
            }


            // Let's find the Texture Holder and then return the texture as a reference into oldTexture

            TerrainTile tileFound =  MapMagicObjectRef.tiles.FindByWorldPosition(x, z);
     
            // We just did that to see if the tile exists really.

            if (null == tileFound)
            {
                Debug.Log("no data yet ");
                return stop;
            }
            else
            {
               
                //    DirectTexturesHolder holder = tileFound.GetComponentInChildren<DirectTexturesHolder>();

                Debug.Log("Finding for "+ x + " " + z );

                oldTexture = DirectTexturesHolder.FindTexture(MapMagicObjectRef, "Road", x, z);

                Debug.Log(oldTexture.isReadable);
                stop = false;
            }

            System.TimeSpan ts = zeit.Elapsed;
            zeit.Stop();
            Debug.Log(ts.TotalMilliseconds);

            zeit.Reset();

            oldPosition = new Vector3(x, z);

            return stop;
        }

        public MapMagicObject MapMagicObjectRef;

        private Transform player;

        void Update()
        {
            // Save if F12 press

            if (Input.GetKeyDown(saveKey) && !WeAreProcessing)
            {

                Go();

                SaveAsset();

            }
        }

        void Start() {

            player = GameObject.FindWithTag("Player").transform;

         


                }


       private  bool WeAreProcessing = false;
      

        void Go()
        {

            WeAreProcessing = true;
            zeit.Start();
         //   if (FindTextureByPosition(player.position.x, transform.position.z))
         //       return;

            // Mesher
            vertices = new List<Vector3>();
            triangles = new List<int>();
            meshFilterMesh = mesher.GetComponent<MeshFilter>().mesh;
            meshColliderMesh = mesher.GetComponent<MeshCollider>().sharedMesh;
            size = 10; // lenght of the mesh in Z direction
            loop = 0;

            // Sprite
            //      pixels = oldTexture.GetPixels();

            Texture2D myTexture = DirectTexturesHolder.FindTexture(MapMagicObjectRef, "Road", player.position.x, player.position.z);


         //   Debug.Log(myTexture.format);
         //   Debug.Log(myTexture.width);
         //   Debug.Log(myTexture.height);

            pixels = myTexture.GetPixels();

            newTexture = new Texture2D(myTexture.width, myTexture.height, TextureFormat.RGBA32, false);


            if ((TryGetComponent(out SpriteRenderer spriteRenderer)))
            {
              
            }
            else
            {
                gameObject.AddComponent<SpriteRenderer>();
            }

           // spriteRenderer = (gameObject.GetComponentByType< SpriteRenderer >()== null) ?  : SpriteRenderer = gameObject.GetComponentByType<SpriteRenderer>();

            ConvertSpriteAndCreateCollider(pixels);
            BrowseColliderToCreateMesh(polygonColliderAdded);

        }

      

        public void ConvertSpriteAndCreateCollider(Color[] pixels)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                // delete all black pixel (black is the circuit, white is the walls)
                pixels[i] = ((pixels[i].r == 0 && pixels[i].g == 0 && pixels[i].b == 0 && pixels[i].a == 0)) ? Color.clear : Color.white;


               
              
            }

          //  Debug.Log(pixels.Length +" pixels");

            // Set a new texture with this pixel list
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            // Create a sprite from this texture
            mySprite = Sprite.Create(newTexture, new Rect(0, 0, newTexture.width, newTexture.height), new Vector2(10.0f, 10.0f), 10.0f, 0, SpriteMeshType.Tight, new Vector4(0, 0, 0, 0), false);

            // Add it to our displayerComponent
            displayerComponent.GetComponent<SpriteRenderer>().sprite = mySprite;

            // Add the polygon collider to our displayer Component and get his path count
            polygonColliderAdded = displayerComponent.AddComponent<PolygonCollider2D>();

        }

        // Method to browse the collider and launch makemesh
        public void BrowseColliderToCreateMesh(PolygonCollider2D polygonColliderAdded)
        {
            //browse all path from collider
            pathCount = polygonColliderAdded.pathCount;
            for (int i = 0; i < pathCount; i++)
            {
                Vector2[] path = polygonColliderAdded.GetPath(i);

                // browse all path point
                for (int j = 1; j < path.Length; j++)
                {
                    if (j != (path.Length - 1)) // if we aren't at the last point
                    {
                        point0 = new Vector3(path[j - 1].x, path[j - 1].y, 0);
                        point1 = new Vector3(path[j - 1].x, path[j - 1].y, size);
                        point2 = new Vector3(path[j].x, path[j].y, size);
                        point3 = new Vector3(path[j].x, path[j].y, 0);
                        MakeMesh(point0, point1, point2, point3);

                    }
                    else if (j == (path.Length - 1))// if we are at the last point, we need to close the loop with the first point
                    {
                        point0 = new Vector3(path[j - 1].x, path[j - 1].y, 0);
                        point1 = new Vector3(path[j - 1].x, path[j - 1].y, size);
                        point2 = new Vector3(path[j].x, path[j].y, size);
                        point3 = new Vector3(path[j].x, path[j].y, 0);
                        MakeMesh(point0, point1, point2, point3);
                        point0 = new Vector3(path[j].x, path[j].y, 0);
                        point1 = new Vector3(path[j].x, path[j].y, size);
                        point2 = new Vector3(path[0].x, path[0].y, size); // First point
                        point3 = new Vector3(path[0].x, path[0].y, 0); // First point
                        MakeMesh(point0, point1, point2, point3);
                    }
                }
            }
        }


        //Method to generate 2 triangles mesh from the 4 points 0 1 2 3 and add it to the collider
        public void MakeMesh(Vector3 point0, Vector3 point1, Vector3 point2, Vector3 point3)
        {

            // Vertice add
            vertices.Add(point0);
            vertices.Add(point1);
            vertices.Add(point2);
            vertices.Add(point3);

            //Triangle order
            triangles.Add(0 + loop * 4);
            triangles.Add(2 + loop * 4);
            triangles.Add(1 + loop * 4);
            triangles.Add(0 + loop * 4);
            triangles.Add(3 + loop * 4);
            triangles.Add(2 + loop * 4);
            loop = loop + 1;

            // create mesh 
            meshFilterMesh.vertices = vertices.ToArray();
            meshFilterMesh.triangles = triangles.ToArray();

            // add this mesh to the MeshCollider
            mesher.GetComponent<MeshCollider>().sharedMesh = meshFilterMesh;
        }

        // Save if F12 press
        public void SaveAsset()
        {

            long offset = System.DateTimeOffset.Now.ToUnixTimeSeconds();

            var mf = mesher.GetComponent<MeshFilter>();
            if (mf)
            {
                var savePath = "Assets/" + saveName + offset + ".asset";
             
                Debug.Log("Saved Mesh to:" + savePath);
                AssetDatabase.CreateAsset(mf.mesh, savePath);
            }
            WeAreProcessing = false;

        }

    } // https://stackoverflow.com/a/54086859
}

using UnityEngine;
using UnityEngine.Networking;

    /// <summary>
    /// Demo component which spawns a set of network objects when the server is started.
    /// </summary>
    public class ObjectSpawner : MonoBehaviour
    {
        /// <summary>
        /// The object, position, and rotation to spawn.
        /// </summary>
        [System.Serializable]
        private class SpawnableObject
        {
            [SerializeField] private GameObject m_Object;
            [SerializeField] private Vector3 m_Position;
            [SerializeField] private Quaternion m_Rotation;
            [SerializeField]
            private float m_SpawnScale =1f;
            [SerializeField]
            private int m_SpawnAmount = 1;


            public GameObject Object { get { return m_Object; } }
            public Vector3 Position { get { return m_Position; } }
            public Quaternion Rotation { get { return m_Rotation; } }
            public float SpawnScale { get { return m_SpawnScale; } }
            public int SpawnAmount { get { return m_SpawnAmount; } }
        }

        // Internal variables
        [SerializeField] private SpawnableObject[] m_SpawnObjects;
         [SerializeField] private Vector3 m_randomOffset;
         public Vector3 RandomOffset { get { return m_randomOffset; } }

         [SerializeField]
         public float repeatTime = 0.2f;


        public void StartMakingThings() {

         //   print("we started making things");

            InvokeRepeating("SpawnThing", .1f, repeatTime);
        
        }

        public void DisableInvokes() {

            CancelInvoke("SpawnThing");
        }

        public static int TotalObjectsSpawned = 0;

        private void SpawnThing()
        {
         
		 Debug.Log("Spwned");

            for (int i = 0; i < m_SpawnObjects.Length; ++i)
            {

                for (int j = 0; j < m_SpawnObjects[i].SpawnAmount; ++j)
                {
                    TotalObjectsSpawned++;
                  //  if (Random.Range(0f, 1f) > 0.4f)
                  //  {



                        Vector3 offset = new Vector3((int)UnityEngine.Random.Range(-RandomOffset.x, RandomOffset.x),
                                                 (int)UnityEngine.Random.Range(-RandomOffset.y, RandomOffset.y),
                                                 (int)UnityEngine.Random.Range(-RandomOffset.z, RandomOffset.z));


                        var obj = GameObject.Instantiate(m_SpawnObjects[i].Object, transform.position + m_SpawnObjects[i].Position + offset, m_SpawnObjects[i].Rotation) as GameObject;

                        //obj.transform.localScale = new Vector3(m_SpawnObjects[i].SpawnScale, m_SpawnObjects[i].SpawnScale, m_SpawnObjects[i].SpawnScale);
                        //if (m_SpawnObjects[i].Object.name == "Cube")
                        //{
                        //    print(m_SpawnObjects[i].SpawnScale);
                        //}
                 //   }
                }
            }
        }
    }


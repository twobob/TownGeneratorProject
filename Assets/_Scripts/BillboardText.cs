using UnityEngine;

[ExecuteInEditMode]
public class BillboardText : MonoBehaviour
{
    private GameObject thingToLookAt;
    private GameObject mainRef;

    private void OnEnabled()
    {
      //  mainRef = Camera.main;
        thingToLookAt = GameObject.FindGameObjectWithTag("Player");
    }

    private void Awake() {
     //   mainRef = Camera.main;
        thingToLookAt = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {

      //  if (mainRef != Camera.main)
        //    mainRef = Camera.main;
        thingToLookAt = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {

        //thingToLookAt = mainRef;
        //if (thingToLookAt == null)
        //    Debug.Log("doh");

        Vector3 v = new Vector3(0, thingToLookAt.transform.position.y - transform.position.y, 0);
      //  v.x = v.z = 0.0f;
        Vector3 targetPos = thingToLookAt.transform.position - v;
        transform.LookAt(targetPos);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 180, transform.localEulerAngles.z);


        //if ((int)Time.timeSinceLevelLoad % 10 == 0)
        //{

        //    Debug.Log(thingToLookAt.transform.position);

        //}
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWithFixedY : MonoBehaviour
{

    public Transform thing;

    public float fixedY = 800f;
//
 //   private string storedName;
 //
    private Vector3 place;

    public bool rotateWithPlayer = false;

    private void OnEnable()
    {
        place = thing.position;
      //  storedName = transform.name;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        place = thing.position;

        transform.position = new Vector3(place.x, fixedY, place.z);

        if (!rotateWithPlayer)
            return;

        //if (storedName != "TownCamera")
       //     return;

        transform.eulerAngles = new Vector3( 90, thing.rotation.eulerAngles.y, 0);
    }
}

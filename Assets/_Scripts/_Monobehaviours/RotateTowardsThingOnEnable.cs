using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// To use this script, attach it to the GameObject that you would like to rotate towards another game object.
// After attaching it, go to the inspector and drag the GameObject you would like to rotate towards into the target field.
// Move the target around in the scene view to see the GameObject continuously rotate towards it.
public class RotateTowardsThingOnEnable : MonoBehaviour
{
    // The target marker.
    private Transform target;

    // Angular speed in radians per sec.
    // public float speed = 1.0f;
    public bool random = false;

    int attempts =0;
    private readonly int maxAttempts = 10;

    public int Attempts { get => attempts; set => attempts = value; }

    private void FindFocus()
    {

        GameObject center = GameObject.Find("TownCenter(Clone)");
      //  GameObject center = GameObject.FindGameObjectWithTag("RotationFocus");

        if (center == null && attempts < maxAttempts)
        {
            Invoke("FindFocus", 2f);
            return;
        }
        if (maxAttempts == attempts)
        {
            Debug.Log("placing by rotation failed for " + transform.name);
            return;
        }

        target ??= center.transform;

       

        // Determine which direction to rotate towards
        Vector3 targetDirection = target.position - transform.position;
        Vector3 newDirection = targetDirection;
        // The step size is equal to speed times frame time.
        // float singleStep = speed * Time.deltaTime;

        if (targetDirection == Vector3.zero)
        {
            // random value
            targetDirection = new Vector3(0.2f, 0.0f, 0.3f); //Vector3.right;

            //Debug.Log("placing by rotation forced focus " + transform.name);


            //Invoke("FindFocus", 2f);
            //return;
        }else 

        if (targetDirection != Vector3.zero && random)
        {


            // Rotate the forward vector towards the target direction by one step
            // Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
             newDirection = Vector3.RotateTowards( transform.forward, targetDirection, (float) RandomGen.Next(360,0), 1.0f).normalized;

           


            // Draw a ray pointing at our target in
             // Debug.DrawRay(transform.position, newDirection, Color.red);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            // transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else if (random)
        {
            newDirection = new Vector3( 0,
               Mathf.Clamp01((1f / ((float)RandomGen.Next(0, 100) + 0.01f)))
               ,0);
        }


        transform.rotation = Quaternion.LookRotation(newDirection);


        // Only rotate Y

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); 
    }

    void OnEnable()
    {

        Invoke("FindFocus", .5f);


    }
}




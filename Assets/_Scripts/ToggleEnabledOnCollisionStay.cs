using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleEnabledOnCollisionStay : MonoBehaviour
{
   // public Transform TMpro;

   // private GameObject tmProgo;

    private BillboardText refScript;


    void Start()
    {
        GetBillboardRefAndInvokePeriodicDisable();

    }

    private void GetBillboardRefAndInvokePeriodicDisable()
    {
        refScript = GetComponent<BillboardText>();
        Invoke("periodicDisable", 1f + (RandomGen.Next(0,199) * 0.01f));
    }

    void OnEnable()
    {
        GetBillboardRefAndInvokePeriodicDisable();

    }

    private void periodicDisable() {


        //tmProgo =
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false); 
        refScript.enabled = false;
       // tmProgo.SetActive(false);
      
    }

       void Awake()
    {
        GetBillboardRefAndInvokePeriodicDisable();
        refScript.enabled = false;
      
    }
    

    // Start is called before the first frame update
    void OnTriggerStay()
    {
      //  tmProgo.SetActive(true);
    }

    void OnTriggerEnter()
    {
        refScript.enabled = true;
        transform.GetChild(0).GetChild(0).gameObject.SetActive(true);  
    }

    void OnTriggerExit() {
        GetBillboardRefAndInvokePeriodicDisable();
    }



    //void OnTriggerEnter()
    //{
    //    //foreach (ContactPoint contact in collision.contacts)
    //    //{
    //    //    Debug.DrawRay(contact.point, contact.normal, Color.white);
    //    //}
    //    tmProgo.SetActive(true);
    //}
}

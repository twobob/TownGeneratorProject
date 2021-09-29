using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAndWriteSigns : MonoBehaviour
{

    public GameObject Sign;
 
    

    /// <summary>
    /// Pass the 
    /// </summary>
    /// <param name="what">string the put in the text</param>
    /// <param name="where">Vector3 to put the sign</param>
    /// <param name="rot">pass at a mimnimum Quaternion.identity</param>
    public void WriteAndMove(string what, Vector3 where, Quaternion how, GameObject who)
    {
        // Instantiate at position (0, 0, 0) and zero rotation.
        var go = Instantiate(Sign, where, how, who.transform);
       // go.transform.localScale = new Vector3(1, 1, 1);
        go.transform.localScale = new Vector3(.4f, .4f, .4f);
        TMPro.TextMeshProUGUI ui = go.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        // go.transform.position = who.transform.position;
        // go.transform.rotation = who.transform.rotation;
        // go.transform.localPosition = Vector3.zero;
        try
        {
            ui.text = what;
        }
        catch 
        {

            return;
        }
     
    }


   
    void OnEnable()
    {
        SignCommands.Instance = this;
   
    }
}

public static class SignCommands
{

    public static SpawnAndWriteSigns Instance;
   
    public static void WriteAndMove(string what, Vector3 where, Quaternion how, GameObject who = null)
    {
        if (null != who && what != string.Empty && where != null && how != null)
        {
            try
            {
              //  Debug.LogFormat(who, "setting up {0} at {1} with a {2} rot.", what, where, how);
                Instance.WriteAndMove(what, where, how, who);
            }
            catch (System.Exception ex)
            {

                Debug.LogFormat(who, "setting up {0} at {1} with a {2} rot. failed", what, where, how, ex.Message);
            }
           
        }
       
      
    }
}
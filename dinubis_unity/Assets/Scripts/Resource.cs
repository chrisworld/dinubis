using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour { //MonoBehaviour


    private OSC myOsc;
    private Image healthslide;

    [SyncVar(hook = "CmdTakeDamage")]
    public float health = 100f;

    //[SyncVar]
    //public float juergen = 1f;

    [Header("Unity")]
    public Image healthBar;



    // Use this for initialization
	void Start () {
        myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();
		//healthslide = healthBar.GetComponentInChildren<HealthBar> ();
        //healthslide = GameObject.FindGameObjectsWithTag("HealthBar");
	}
	
  

    [Command] 
    public void CmdTakeDamage (float amount)
    {
        if (health == 1)
        {
           End();

        }

        // if (health == 0)
        // {
        //    End_Kill();

        // }

        else {
        health -= amount;
        healthBar.fillAmount = health / 100f;
       // juergen = health / 100f;
        Debug.Log("health: "+health);
        OSCDig();
        }
    }





    //[ClientRPC]
/*
[SyncVar] only sync from Server --> Client. They never sync from Client --> Server. However they 
            *can* be changed locally on the client and no errors will be thrown.

[Command] send information from Client --> Server. It can only be used if the client owns the object 
        and has authority.
[ClientRPC] are just like command's but send information from Server --> all Client's
[TargetRPC] is just like ClientRPC, but only sends to 1 client (not all clients)

Your hook looks correct.

If you want the client to change the value of the SyncVar then send a Command to the server and 
then on the server change the value. This new updated value will then be trickled back to all clients
and hooks will be called. 
*/

/*
if(isClient){


}

*/


  //OSC Message 
  private void OSCDig(){
  OscMessage msg = new OscMessage ();
  msg.address = "/resource_dig";
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  Debug.Log("Send OSC message /resource_dig");
  }







  private void OSCEnd(){
  OscMessage msg = new OscMessage ();
  msg.address = "/abbau_end";
  msg.values.Add (2);
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  Debug.Log("Send OSC message /resource_end");
  }

    // Update is called once per frame
    void Update () {
     //   healthBar.fillAmount = juergen;
	}

    void End ()
    {
        Debug.Log("The End.(Looting completed.)");
        OSCEnd();
        // #if UNITY_EDITOR
        //   UnityEditor.EditorApplication.isPlaying = false;
        // #else
        //   Application.Quit();
        // #endif
    }
}

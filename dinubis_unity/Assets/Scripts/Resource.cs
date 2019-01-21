using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Networking;

public class Resource : NetworkBehaviour { //MonoBehaviour


    private OSC myOsc;
    private Image healthslide;

    //[SyncVar]
    public float health = 100;

    [SyncVar]
    public float juergen = 1f;

    [Header("Unity")]
    public Image healthBar;



    // Use this for initialization
	void Start () {
        myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();
		//healthslide = healthBar.GetComponentInChildren<HealthBar> ();
        //healthslide = GameObject.FindGameObjectsWithTag("HealthBar");
	}
	
    //[SyncEvent]
    public void TakeDamage (float amount)
    {
        if (health <= 0)
        {
           End();
        }

        else {
        health -= amount;
        juergen = health / 100f;
        Debug.Log("health: "+health);
        OSCDig();
        }
    }

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
  msg.address = "/end";
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  Debug.Log("Send OSC message /resource_end");
  }


    // Update is called once per frame
    void Update () {
        healthBar.fillAmount = juergen;
	}


    void End ()
    {
        Debug.Log("The End.(Looting completed.)");
        OSCEnd();
        EditorApplication.isPlaying = false;
    }
}

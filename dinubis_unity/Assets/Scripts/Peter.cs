using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peter : MonoBehaviour {

  void OnTriggerEnter(Collider col)
  {
    if(col.gameObject.CompareTag("Player")){
      OSCCollidePeter();
    }
  }

  private void OSCCollidePeter()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    //msg.address = "/peter";
    msg.address = "/hey";
    myOsc.Send (msg);
    Debug.Log("Collision with Peter -> OSC");
  }
}

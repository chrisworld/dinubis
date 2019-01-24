using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uhrturm : MonoBehaviour {

  void OnTriggerEnter(Collider col)
  {
    if(col.gameObject.CompareTag("Player")){
      OSCCollideUhrturm();
    }
  }

  private void OSCCollideUhrturm()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    //msg.address = "/uhrturm";
    msg.address = "/hey";
    myOsc.Send (msg);
    Debug.Log("Collision with Uhrturm -> OSC");
  }
}

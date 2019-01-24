using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oper : MonoBehaviour {

  void OnTriggerEnter(Collider col)
  {
    if(col.gameObject.CompareTag("Player")){
      OSCCollideOper();
    }
  }

  private void OSCCollideOper()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    //msg.address = "/oper";
    msg.address = "/hey";
    myOsc.Send (msg);
    Debug.Log("Collision with oper -> OSC");
  }
}

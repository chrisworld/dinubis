using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : MonoBehaviour {

  void OnTriggerEnter(Collider col)
  {
    if(col.gameObject.CompareTag("Player")){
      OSCCollideAnvil();
    }
  }

  private void OSCCollideAnvil()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    //msg.address = "/anvil";
    msg.address = "/hey";
    myOsc.Send (msg);
    Debug.Log("Collision with Anvil -> OSC");
  }
}

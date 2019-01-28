using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Nubi : NetworkBehaviour {

  void Start()
  {
    if (isLocalPlayer) {
      FindObjectOfType<SoundManager>().ActivateSounds();
    }
  }

  void OnControllerColliderHit(ControllerColliderHit hit)
  {
    if(hit.gameObject.CompareTag("Player")){
      OSCCollideNubi();
    }
  }

  private void OSCCollideNubi()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/hey";
    myOsc.Send (msg);
    Debug.Log("Collision with nubi -> OSC");
  }
}

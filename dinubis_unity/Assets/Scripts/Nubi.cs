using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Nubi : NetworkBehaviour {

  //private AudioListener al;

  void Start()
  {
    /*
    al = gameObject.GetComponentInChildren<AudioListener>();
    if (isLocalPlayer) {
      al.enabled = true;
      FindObjectOfType<SoundManager>().ActivateSounds();
    }
    */
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

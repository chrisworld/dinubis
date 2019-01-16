using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResiSpawner : NetworkBehaviour {

  public Transform spawn_center;
  public GameObject resi_prefab;
  public int n_resi = 1;

  // Use this for initialization
  public override void OnStartServer () // This is invoked for NetworkBehaviour objects when they become active on the server.
  {
    OSCSendNumResi();
    for (int index = 0; index < n_resi; index = index + 1) 
    {
      Vector3 spawnPosition = new Vector3 (spawn_center.position.x + Random.Range(-20f, 20f), 0.5f, spawn_center.position.z + Random.Range(-20f, 20f));
      //Vector3 spawnPosition = new Vector3 (104f, 0.5f, 43f);
      Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);

      GameObject resi = (GameObject)Instantiate (resi_prefab, spawnPosition, spawnRotation);
      resi.GetComponent<Resident>().id = index;
      resi.GetComponent<Resident>().freq = index * 20 + 50;
      NetworkServer.Spawn (resi);
    }
  }

  private void OSCSendNumResi()
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/num_resi";
    msg.values.Add (n_resi);
    myOsc.Send (msg);
    Debug.Log("Send resi num: " + n_resi);
  }
}

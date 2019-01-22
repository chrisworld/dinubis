using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResiSpawner : NetworkBehaviour {

  public Transform spawn_center;
  public GameObject resi_prefab;
  public int n_resi = 1;
  public int base_freq = 60;

  private int active_resis = 0;

  private GameObject[] resis;

  // Use this for initialization
  public override void OnStartServer () // This is invoked for NetworkBehaviour objects when they become active on the server.
  {
    OSCSendNumResi(n_resi);
    for (int index = 0; index < n_resi; index = index + 1) 
    {
      Vector3 spawnPosition = new Vector3 (spawn_center.position.x + Random.Range(-20f, 20f), 0.5f, spawn_center.position.z + Random.Range(-20f, 20f));
      //Vector3 spawnPosition = new Vector3 (104f, 0.5f, 43f);
      Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);

      GameObject resi = (GameObject)Instantiate (resi_prefab, spawnPosition, spawnRotation);
      resi.GetComponent<Resident>().id = index;
      resi.GetComponent<Resident>().freq = index * base_freq + base_freq;
      resi.GetComponent<Resident>().spawner_pos = gameObject.transform.position;

      NetworkServer.Spawn (resi);
    }
  }

  void Update()
  {
    CountActiveResis();
  }

  // Init resis
  private void CountActiveResis()
  {
    int cur_active_resis = 0;
    resis = GameObject.FindGameObjectsWithTag("Resident");
    foreach (GameObject resi in resis)
    {
      if (resi.GetComponent<Resident>().in_hearing_dist){
        cur_active_resis += 1;
      }
    }
    if (cur_active_resis != active_resis){
      active_resis = cur_active_resis;
      OSCSendNumResi(active_resis);
    }
  }

  private void OSCSendNumResi(int num_act_resis)
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/num_resi";
    msg.values.Add (num_act_resis);
    myOsc.Send (msg);
    Debug.Log("Send active resi num: " + num_act_resis);
  }
}

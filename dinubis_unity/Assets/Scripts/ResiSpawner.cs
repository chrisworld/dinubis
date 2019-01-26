using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ResiSpawner : NetworkBehaviour {

  public Transform[] spawn_positions;
  public GameObject resi_prefab;
  public float spawn_range = 20f; 
  public int n_resi = 1;
  public int[] base_freqs = {50, 60, 70};

  private int active_resis = 0;

  private GameObject[] resis;

  private int resi_index;

  // Use this for initialization
  public override void OnStartServer () // This is invoked for NetworkBehaviour objects when they become active on the server.
  {
    //OSCSendNumResi(n_resi * spawn_positions.Length);
    resi_index = 0;
    foreach (Transform spawn_pos in spawn_positions){
      int resi_base_freq = base_freqs[Random.Range(0, base_freqs.Length)];
      for (int index = 0; index < n_resi; index = index + 1) 
      {
        Vector3 spawnPosition = new Vector3 (spawn_pos.position.x + Random.Range(-spawn_range, spawn_range), 0.5f, spawn_pos.position.z + Random.Range(-spawn_range, spawn_range));
        //Vector3 spawnPosition = new Vector3 (104f, 0.5f, 43f);
        Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);
        GameObject resi = (GameObject)Instantiate (resi_prefab, spawnPosition, spawnRotation);
        resi.GetComponent<Resident>().id = resi_index;
        resi.GetComponent<Resident>().freq = index * resi_base_freq + resi_base_freq;
        resi.GetComponent<Resident>().spawner_pos = spawn_pos.position;
        resi_index += 1;
        NetworkServer.Spawn (resi);
      }
    }
  }

  void Start()
  {
    OSCSendNumResi(n_resi * spawn_positions.Length);
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
      OSCSendNumActiveResi(active_resis);
    }
  }

  private void OSCSendNumResi(int num_resis)
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/num_resi";
    msg.values.Add (num_resis);
    myOsc.Send (msg);
    Debug.Log("Send resi num: " + num_resis);
  }

  private void OSCSendNumActiveResi(int num_act_resis)
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/num_active";
    msg.values.Add (num_act_resis);
    myOsc.Send (msg);
    Debug.Log("Send active resi num: " + num_act_resis);
  }
}

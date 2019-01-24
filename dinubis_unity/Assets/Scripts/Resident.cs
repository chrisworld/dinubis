using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Resident : NetworkBehaviour {

  public float follow_dist;
  public float follow_speed;
  public float walking_range;
  public float walking_speed;
  public float hearing_dist;

  public float rot_phase_shift = 10;

  [HideInInspector]
  public Vector3 spawner_pos;
  [HideInInspector]
  public bool follow_nubi;
  [HideInInspector]
  public int id;
  [HideInInspector]
  public bool in_hearing_dist;

  public float freq;

  NavMeshAgent agent;

  private GameObject[] nubis;
  private OSC myOsc;
  private float follow_dist_sqr;
  private float hearing_dist_sqr;

  // Use this for initialization
  void Start () {
    // NavMesh Agent setup
    agent = gameObject.GetComponent<NavMeshAgent>();
    // OSC init
    myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();
    OSCSendSpawnResi();
    follow_dist_sqr = follow_dist * follow_dist;
    hearing_dist_sqr = hearing_dist * hearing_dist;
  }
  
  // Update is called once per frame
  void Update () 
  {
    CalculateDistances();
  }

  // calculate Distances
  private void CalculateDistances()
  {
    // calculate distances
    //Vector3 resident_pos = gameObject.GetComponent<Transform>().position;
    nubis = GameObject.FindGameObjectsWithTag("Player");

    //List<float> distance_list = new List<float>();
    Dictionary<GameObject, float> nubi_dict = new Dictionary<GameObject, float>();

    // run all nubis in game
    foreach (GameObject nubi in nubis)
    {
      float distance = (gameObject.transform.position - nubi.transform.position).sqrMagnitude;
      nubi_dict.Add(nubi, distance);
      
      // find local player and set synth params
      if (nubi.GetComponent<NetworkIdentity>().isLocalPlayer){
        // nubi in hearing distance
        if (distance < hearing_dist_sqr) in_hearing_dist = true;
        else in_hearing_dist = false;

        float norm_dist = 1 - ( distance / hearing_dist_sqr );
        // calculate Rotation to player
        float height = nubi.GetComponent<Transform>().position.y / 10;
        float norm_rot =  Mathf.Abs((gameObject.transform.rotation.eulerAngles.y - nubi.transform.rotation.eulerAngles.y) / 180);
        OSCSendUpdateResi(freq + rot_phase_shift * norm_rot, Mathf.Clamp(norm_dist, 0, 1), Mathf.Clamp(height, 0, 1));
      }
    }



    // follow closest nubi via nav agent
    if (nubi_dict.Count != 0){
      float min_dist = nubi_dict.Values.Min();

      // track the nubi down
      if (min_dist < follow_dist_sqr)
      {
        GameObject closest_nubi = FindClosestNubi(nubi_dict);
        agent.speed = follow_speed;
        CmdAgentSetDestination(closest_nubi.transform.position);
        //agent.SetDestination(closest_nubi.transform.position);
      }
      else
      {
        if (!agent.pathPending){
          if (agent.remainingDistance <= agent.stoppingDistance){
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f){
              agent.speed = walking_speed;
              CmdAgentSetDestination(RandomNavmeshLocation(walking_range));
              //agent.SetDestination(RandomNavmeshLocation(walking_range));
            }
          }
        }
      }
    }
  }

  [Command]
  public void CmdAgentSetDestination(Vector3 argPosition)
  {
    RpcAgentSetDestination(argPosition);    
  }
 
  [ClientRpc]
  public void RpcAgentSetDestination(Vector3 argPosition)
  {
    agent.SetDestination(argPosition);
  }

  // Find the closest nubi and returns him
  private GameObject FindClosestNubi(Dictionary<GameObject, float> nubi_dict)
  {
    //var ordered = nubi_dict.OrderBy(x => x.Value);
    float min_value = nubi_dict.Values.Min();
    var closest_nubis = nubi_dict.Where(nubi => nubi.Value.Equals(min_value)).Select(nubi => nubi.Key);
    foreach (GameObject closest_nubi in closest_nubis){
      return closest_nubi;
    }
    return null;
  }

  // copied from: https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
  // credits to Selzier
  public Vector3 RandomNavmeshLocation(float radius) 
  {
    Vector3 randomDirection = Random.insideUnitSphere * radius;

    // random position around the spawner
    randomDirection += spawner_pos;
    //randomDirection += transform.position;

    NavMeshHit hit;
    Vector3 finalPosition = Vector3.zero;
    if (NavMesh.SamplePosition(randomDirection, out hit, radius,  NavMesh.AllAreas)) {
       finalPosition = hit.position;            
    }
    return finalPosition;
  }

  // Collision Trigger
  void OnTriggerEnter(Collider col){
    if(col.gameObject.CompareTag("Player")){
      freq = Mathf.Clamp(freq + Random.Range(-20, 20), 20, 1000);
      OSCCollisionResi();
    }
  }

  // OSC Messages-----
  // OSC Coll Resi
  private void OSCCollisionResi(){
    OscMessage msg = new OscMessage ();
    msg.address = "/resiColl";
    msg.values.Add (freq);
    myOsc.Send (msg);
    Debug.Log("Send OSC message /resiColl new freq: " + freq);
  }

  // OSC spawn Resi
  private void OSCSendSpawnResi(){
    OscMessage msg = new OscMessage ();
    msg.address = "/spawn_resi";
    msg.values.Add (id);
    msg.values.Add (freq);
    msg.values.Add (0);
    myOsc.Send (msg);
    Debug.Log("Send message /spawn_resi with id: " + id + " freq: " + freq);
  }

  // OSC update Resi
  private void OSCSendUpdateResi(float f, float mag, float height){
    OscMessage msg = new OscMessage ();
    msg.address = "/update_resi";
    msg.values.Add (id);
    msg.values.Add (f);
    msg.values.Add (mag);
    msg.values.Add (height);
    myOsc.Send (msg);
    //Debug.Log("Send message /update_resi with id: " + id + " freq: " + freq + " mag: " + mag + " height: " + height);
  }

  // OSC Send num Resi
  private void OSCSendNumResi(int n_resi)
  {
    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/num_resi";
    msg.values.Add (n_resi);
    myOsc.Send (msg);
    Debug.Log("Send resi num: " + n_resi);
  }
}

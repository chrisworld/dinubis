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

  [HideInInspector]
  public bool follow_nubi;
  [HideInInspector]
  public int id;
  public int freq;

  NavMeshAgent agent;

  private GameObject[] nubis;
  private OSC myOsc;
  private float follow_dist_sqr;

  // Use this for initialization
  void Start () {
    // NavMesh Agent setup
    agent = gameObject.GetComponent<NavMeshAgent>();
    // OSC init
    myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();
    OSCSendSpawnResi();
    follow_dist_sqr = follow_dist * follow_dist;
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
    Vector3 resident_pos = gameObject.GetComponent<Transform>().position;
    nubis = GameObject.FindGameObjectsWithTag("Player");

    //List<float> distance_list = new List<float>();
    Dictionary<GameObject, float> nubi_dict = new Dictionary<GameObject, float>();

    // run all nubis in game
    foreach (GameObject nubi in nubis)
    {
      float distance = (resident_pos - nubi.transform.position).sqrMagnitude;
      nubi_dict.Add(nubi, distance);
      
      // find local player and set synth params
      if (nubi.GetComponent<NetworkIdentity>().isLocalPlayer){
        float norm_dist = 1 - ( distance / follow_dist_sqr );
        //Debug.Log("Localplayer nubi with distance: " + distance + " norm dist: " + Mathf.Clamp(norm_dist, 0, 1));
        OSCSendUpdateResi(freq, Mathf.Clamp(norm_dist, 0, 1));
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
        agent.SetDestination(closest_nubi.transform.position);
      }
      else
      {
        if (!agent.pathPending){
          if (agent.remainingDistance <= agent.stoppingDistance){
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f){
              agent.speed = walking_speed;
              agent.SetDestination(RandomNavmeshLocation(walking_range));
            }
          }
        }
      }
    }
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
    randomDirection += transform.position;
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
      Debug.Log("Collision with Resi");
      OSCCollisionResi();
      Destroy(gameObject);
      OSCCollisionResi();
    }
  }

  // OSC Messages-----
  // OSC Coll Resi
  private void OSCCollisionResi(){
    OscMessage msg = new OscMessage ();
    msg.address = "/resiColl";
    //msg.values.Add (transform.position.x);
    myOsc.Send (msg);
    Debug.Log("Send OSC message /resiColl");
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
  private void OSCSendUpdateResi(int f, float mag){
    OscMessage msg = new OscMessage ();
    msg.address = "/update_resi";
    msg.values.Add (id);
    msg.values.Add (f);
    msg.values.Add (mag);
    myOsc.Send (msg);
    //Debug.Log("Send message /update_resi with id: " + id + " freq: " + freq);
  }
}

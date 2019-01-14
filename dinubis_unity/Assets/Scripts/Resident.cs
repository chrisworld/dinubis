using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Resident : MonoBehaviour {

  public float follow_dist;
  public float follow_speed;
  public float walking_range;
  public float walking_speed;

  [HideInInspector]
  public bool follow_nubi;
  NavMeshAgent agent;

  private GameObject[] nubis;
  private OSC myOsc;

  // Use this for initialization
  void Start () {
    // NavMesh Agent setup
    agent = gameObject.GetComponent<NavMeshAgent>();

    myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/soundObject";
    msg.values.Add (transform.position.x);
    msg.values.Add (transform.position.y);
    msg.values.Add (transform.position.z);
    myOsc.Send (msg);
    //Debug.Log("Send message /soundObject");

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
    foreach (GameObject nubi in nubis)
    {
      // TODO: calculate distances
      float distance = (resident_pos - nubi.transform.position).sqrMagnitude;
      nubi_dict.Add(nubi, distance);
    }

    // follow closest nubi
    if (nubi_dict.Count != 0){
      float min_dist = nubi_dict.Values.Min();

      // track the nubi down
      if (min_dist < follow_dist * follow_dist)
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

  // Collision
  void OnTriggerEnter(Collider col){
    if(col.gameObject.CompareTag("Player")){
      Debug.Log("Collision with Resi");
      OSCCollisionResi();
      Destroy(gameObject);
      OSCCollisionResi();
    }
  }

  private void OSCCollisionResi(){
    OscMessage msg = new OscMessage ();
    msg.address = "/resiColl";
    //msg.values.Add (transform.position.x);
    myOsc.Send (msg);
    Debug.Log("Send OSC message /resiColl");
  }
}

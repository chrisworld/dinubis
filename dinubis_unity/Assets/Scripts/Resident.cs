using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Resident : MonoBehaviour {

  public float follow_dist;
  [HideInInspector]
  public bool follow_nubi;
  NavMeshAgent agent;

  private GameObject[] nubis;

  // Use this for initialization
  void Start () {
    agent = gameObject.GetComponent<NavMeshAgent>();
    agent.SetDestination(gameObject.GetComponent<Transform>().position);

    OSC myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();;
    OscMessage msg = new OscMessage ();
    msg.address = "/soundObject";
    msg.values.Add (transform.position.x);
    msg.values.Add (transform.position.y);
    msg.values.Add (transform.position.z);
    myOsc.Send (msg);
    Debug.Log("Send message /soundObject");
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
    Dictionary<GameObject, float> distance_dict = new Dictionary<GameObject, float>();
    foreach (GameObject nubi in nubis)
    {
      // TODO: calculate distances
      float distance = (resident_pos - nubi.transform.position).sqrMagnitude;
      distance_dict.Add(nubi, distance);
    }

    // follow closest nubi
    if (distance_dict.Count != 0){
      //float max = distance_dict.Values.Max();
      float min = distance_dict.Values.Min();
      //Debug.Log("max/min distance: " + max + " / " + min);
      if (min < follow_dist * follow_dist){
        //agent.SetDestination(white_sheep.position);
      }
    }
  }

  // Collision
  void OnCollisionEnter(Collision collision){
    /*
    if (collision.gameObject.name == "black_sheep"){
      Debug.Log("Wolf eats black sheep");
      shepherd.GetComponent<ShepherdAgent>().BlackSheepDead();
    }
    */
  }
}

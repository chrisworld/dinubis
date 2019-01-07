using System.Collections;
using System.Collections.Generic;
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
  }
  
  // Update is called once per frame
  void Update () {
    // calculate distances
    Vector3 resident_pos = gameObject.GetComponent<Transform>().position;
    nubis = GameObject.FindGameObjectsWithTag("Player");
    foreach (GameObject nubi in nubis)
    {
      // TODO: calculate distances
      //resident_dist = Vector3.Distance(resident_pos, nubi.position);
    }

    // calculate distances to each player
    //float resident_dist = Vector3.Distance(wolf_pos, shepherd.position);

    //Debug.Log("distance: " + shepherd_dist + white_sheep_dist);
    // follow closest object
    /*
    if (white_sheep_dist < follow_dist){
      agent.SetDestination(white_sheep.position);
      follow_shepherd = false;
      follow_white_sheep = true;
    }
    */
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
  // public
  public float walkSpeed = 1;
  public float runSpeed = 3;
  public float turnSmoothTime = 0.2f;
  public float speedSmoothTime = 0.1f;
  public float gravity = -12;
  public float jumpHeight = 0.5f;



  public float attackDamage = 1f;
  private float maxDistance = 15f;
  private float maxDistanceResi = 50f;
  private Resource resource;
  private GameObject[] resources;
  private GameObject[] resis;
  private OSC myOsc;




  //controls how much the player can turn while in mid air
  public float inAirControl = 1;          
  public bool move_activated = true;

  // private
  private bool walk = false;
  private bool run = false;

  private float turnSmoothVelocity;
  private float speedSmoothVelocity;
  private float currentSpeed;
  private float velocityY;

  private Animator anim;
  private Transform cameraT;
  private CharacterController controller;


  // start
  void Start()
  {
    anim = GetComponentInChildren<Animator>();
    cameraT = Camera.main.transform;
    controller = GetComponent<CharacterController>();
    myOsc = GameObject.Find ("OSCManager").GetComponent<OSC> ();


  }

  // Move Character
  public void Move(Vector2 input, bool run_active)
  {
    if (!isLocalPlayer) {
      return;
    };

    // control movement
    Vector2 inputDirection = input.normalized;
    float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
    transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));

    if (controller.enabled)
    {
      float targetSpeed = (run_active ? runSpeed : walkSpeed) * inputDirection.magnitude;
      currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

      velocityY += Time.deltaTime * gravity;
      Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;
      controller.Move(velocity * Time.deltaTime);
      currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
      if (controller.isGrounded){
        velocityY = 0;
      }
      //this is how we tell the animation controller which state we're in
      float animationSpeedPercent = (run ? currentSpeed / runSpeed : currentSpeed / walkSpeed * 0.5f);
      
      // animation
      anim.SetFloat("speed", animationSpeedPercent, GetModifiedSmoothTime(speedSmoothTime), Time.deltaTime);

      // walk and run sound
      if (animationSpeedPercent > 0.1 && animationSpeedPercent < 0.6 && !walk){
        walk = true;
        run = false;
        OSCPlayerWalk();
      }
      else if (animationSpeedPercent > 0.6 && !run){
        run = true;
        walk = false;
        OSCPlayerRun();
      }
      else if (animationSpeedPercent < 0.1 && (walk || run)){
        walk = false;
        run = false;
        OSCPlayerStop();
      }
    }
  }


  
  public void Dig()  //in InputHandler
  {
    if (!isLocalPlayer) {
      return;
    };

    // Find objects
    resources = GameObject.FindGameObjectsWithTag("Resource");
    resis = GameObject.FindGameObjectsWithTag("Resident");

    foreach (GameObject resi in resis) //
    { 
      Vector3 player_pos = gameObject.GetComponent<Transform>().position;
      Vector3 resi_pos = resi.GetComponent<Transform>().position;
      float distance_resi = (player_pos - resi_pos).sqrMagnitude;
    
      if (distance_resi > maxDistanceResi) {

        foreach (GameObject resource in resources)
        {

        //float resource_pos = Vector3.Distance(resource.position, transform.position);
    
        //resource = GameObject.FindGameObjectsWithTag("Resource");
        Vector3 resource_pos = resource.GetComponent<Transform>().position;
        float distance_resource = (player_pos - resource_pos).sqrMagnitude;

          if (distance_resource < maxDistance) {
            
            //health_bar = transform.Find("Resource").gameObject.GetComponent<Image>();
            //health_bar = resource.transform.GetChild(1).GetComponent<HealthBar>();
            //health_bar = GameObject.FindGameObjectsWithTag("HealthBar");
            //health_bar = resource.transform.Find("HealthBar").gameObject;
            //resource.TakeDamage(attackDamage);
            resource.GetComponent<Resource>().TakeDamage(attackDamage);
          } 
        }
      }
    }
  }

  //OSC Messages

  //Player Walk
  private void OSCPlayerWalk(){
  OscMessage msg = new OscMessage ();
  msg.address = "/player_move";
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  //Debug.Log("Send OSC message /player_walk");
  }

  //Player Run
  private void OSCPlayerRun(){
  OscMessage msg = new OscMessage ();
  msg.address = "/player_run";
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  //Debug.Log("Send OSC message /player_run");
  }

  //Player Stop
  private void OSCPlayerStop(){
  OscMessage msg = new OscMessage ();
  msg.address = "/player_stop";
  //msg.values.Add (transform.position.x);
  myOsc.Send (msg);
  //Debug.Log("Send OSC message /player_stop");
  }



  //--


  // play the flute
  public void GetAttacked()
  {
    //anim.SetTrigger("getAttacked");
    //sound_player.hamlin_hurt.Play();
  }

  //should work even when movement disabled
  public void Jump()
  {
    if (!isLocalPlayer) {
      return;
    };
    //if (controller.isGrounded)
    //{
      float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
      velocityY = jumpVelocity;
      //anim.SetTrigger("jump");
    //}
  }

  //not convinced this is really that useful - may delete later
  float GetModifiedSmoothTime(float smoothTime)
  {
    if (controller.isGrounded) return smoothTime;
    else if (inAirControl == 0) return float.MaxValue;
    else return (smoothTime / inAirControl);
  }
}
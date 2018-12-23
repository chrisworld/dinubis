using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  // public
  public float walkSpeed = 1;
  public float runSpeed = 3;
  public float turnSmoothTime = 0.2f;
  public float speedSmoothTime = 0.1f;
  public float gravity = -12;
  public float jumpHeight = 0.5f;

  //controls how much the player can turn while in mid air
  public float inAirControl = 1;          

  public bool move_activated = true;

  // private
  private bool walk = false;
  private bool run = false;

  float turnSmoothVelocity;
  float speedSmoothVelocity;
  float currentSpeed;
  float velocityY;

  Animator anim;
  Transform cameraT;
  CharacterController controller;

  // start
  void Start()
  {
    cameraT = Camera.main.transform;
    controller = GetComponent<CharacterController>();
  }

  // update
  void Update()
  {

  }

  // Move hamlin
  public void Move(Vector2 input, bool run_active)
  {
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
      //anim.SetFloat("speedPercent", animationSpeedPercent, GetModifiedSmoothTime(speedSmoothTime), Time.deltaTime);
      
      // walk and run sound
      if (animationSpeedPercent > 0.1 && animationSpeedPercent < 0.6 && !walk){
        walk = true;
        run = false;
      }
      else if (animationSpeedPercent > 0.6 && !run){
        run = true;
        walk = false;
      }
      else if (animationSpeedPercent < 0.1 && (walk || run)){
        walk = false;
        run = false;
      }
    }
  }

  // play the flute
  public void getAttacked()
  {
    //anim.SetTrigger("getAttacked");
    //sound_player.hamlin_hurt.Play();
  }

  //should work even when movement disabled
  public void Jump()
  {
    Debug.Log("Jump");
    if (controller.isGrounded)
    {
      float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
      velocityY = jumpVelocity;
      //anim.SetTrigger("jump");
    }
  }

  //not convinced this is really that useful - may delete later
  float GetModifiedSmoothTime(float smoothTime)
  {
    if (controller.isGrounded) return smoothTime;
    else if (inAirControl == 0) return float.MaxValue;
    else return (smoothTime / inAirControl);
  }

}

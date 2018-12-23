using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// handle inputs
public class InputHandler : MonoBehaviour
{
  private PlayerController player;

  void Start()
  {
    if(player == null){
      player = gameObject.GetComponent<PlayerController>();
    }
  }

  void Update()
  {
    handleInput();
  }

  public void handleInput()
  {
    // input Readers variables
    Vector2 move_direction = moveInputReader();
    // Move
    if(player.move_activated)
    {
      player.Move(move_direction, checkValidRunKey());
    }
    // Jump
    if (checkValidJumpKey())
    {
      player.Jump();
    }
  }

  // move input reader
  public Vector2 moveInputReader()
  {
    return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
  }

  // check valid jump keys
  public bool checkValidJumpKey()
  {
    KeyCode[] valid_keys = {
      KeyCode.Space,
      KeyCode.Keypad0,
      KeyCode.RightControl
    };
    // check valid key
    foreach (KeyCode key in valid_keys){
      if (Input.GetKeyDown(key)) return true;
    }
    return false;
  }

  // check valid run keys
  public bool checkValidRunKey()
  {
    KeyCode[] valid_keys = {
      KeyCode.LeftShift,
      KeyCode.Keypad1,
      KeyCode.RightShift
    };
    // check valid key
    foreach (KeyCode key in valid_keys)
    {
      if (Input.GetKey(key))
      {
        return true;
      }
    }
    return false;
  }
}

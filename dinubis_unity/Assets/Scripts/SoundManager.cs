using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

  public AudioSource[] sounds;

  public void ActivateSounds()
  {
    // set Sound Sources active
    foreach (AudioSource s in sounds){
      s.Play();
    }
  }
}

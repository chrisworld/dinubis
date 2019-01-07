using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
 	void Update() 
 	{
		if (Input.GetKey("escape")){			
			#if UNITY_EDITOR
	      UnityEditor.EditorApplication.isPlaying = false;
	    #else
	      Application.Quit();
	    #endif
		}
  }
}

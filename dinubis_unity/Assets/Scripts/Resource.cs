using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Resource : MonoBehaviour {

    public float health = 100;

    [Header("Unity")]
    public Image healthBar;


    // Use this for initialization
	void Start () {
		
	}
	

    public void TakeDamage (float amount)
    {
        health -= amount;

        healthBar.fillAmount = health / 100f;

        if (health <= 0)
        {
           // End();
        }

    }


    // Update is called once per frame
    void Update () {
		
	}
}

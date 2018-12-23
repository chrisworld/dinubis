using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThirdPersonCamera : MonoBehaviour {

    public float mouseSensitivity = 5;
    public Transform player;
    public float rotationSmoothTime = 0.12f;

    //NOTE: if you change the camera angle or player size you may need to adjust these
    //min and max limit camera rotation so it doesn't look weird e.g. rotating underneath floor
    public float pitchMin = 10f;
    public float pitchMax = 60f;
    public float distanceFromPlayer = 5f;

    float yaw;
    float pitch;

    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void Start()
    {
        //Set Cursor to not be visible
        Cursor.visible = false;
    }

    void LateUpdate () {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = player.position - transform.forward * distanceFromPlayer;
	}
}

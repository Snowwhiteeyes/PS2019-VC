﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public Transform respawnLocation;
    public Text debugText;
    public GameObject light;
    [Header("Movement Settings")]
    public float desktopAcceleration = 1;
    public float mobileAcceleration = 1;
    public float lightTurnMultiplier = 1;
    public float friction = 1;
    [Header("Hotkey Settings")]
    public KeyCode respawnKey;
    [Header("Debugging")]
    public Vector3 velocity;
    public Gyroscope myGyro;
    public Vector3 input;
    public Quaternion defaultLightRotation;

    void Start()
    {
        myGyro = Input.gyro;
        myGyro.enabled = true; //enable Gyro
        defaultLightRotation = light.transform.rotation;
    }

    void Update()
    {
        UserInputs();
        NewMovement();
        //checkErrors();
        //Light();

        if (transform.position.y < -10f)
        {
            Respawn();
        }
    }

    void Light()
    {
        Vector3 rotated_input = new Vector3(-input.z, 0, input.x);
        light.transform.rotation = defaultLightRotation;
        light.transform.Rotate((light.transform.InverseTransformDirection(rotated_input) / mobileAcceleration) * lightTurnMultiplier);

    }

    void NewMovement()
    {
        rb.velocity += input;
    }

    void Movement()
    {
        velocity = transform.InverseTransformDirection(rb.velocity); //In case rb.velocity changed due to collisions, apply changes to velocity aswell
        velocity += input; //Add Acceleration to current Velocity
        velocity = Vector3.MoveTowards(velocity, Vector3.zero, friction); //Apply Friction
        rb.velocity = transform.TransformDirection(velocity);
    }

    void checkErrors()
    {
        if (friction >= desktopAcceleration)
            Debug.LogError("Acceleration ("+ desktopAcceleration + ") should be larger than friction ("+ friction + ") !");
    }

    void UserInputs()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            input = new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal")); //Get Desktop User Input
            input *= desktopAcceleration;
        }
        else
        {
            input = new Vector3(Input.acceleration.y, 0, -Input.acceleration.x); //Get Mobile User Input
            input *= mobileAcceleration;
        }

        if (Input.GetKeyDown(respawnKey))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = respawnLocation.position; //reset position
        rb.velocity = Vector3.zero; //reset velocity
        CheckpointScript tmp = GetComponent<CheckpointScript>();
        tmp.reachedCheckpoints.Clear(); //reset reached checkpoints
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}

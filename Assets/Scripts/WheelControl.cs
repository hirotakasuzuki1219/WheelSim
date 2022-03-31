using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WheelControl : MonoBehaviour
{
    public ContactForce CF;
    public float rotaional_velocity;
    public float steering_velocity;
    //public float translational_velocity;
    private float verticalInput;
    private float horizontalInput;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(new Vector3(0, -verticalInput * rotaional_velocity, 0) * Time.deltaTime);
        transform.Rotate(new Vector3(0, horizontalInput * steering_velocity, 0)*Time.deltaTime, Space.World);
        //transform.Translate(Vector3.right * horizontalInput *Time.deltaTime *translational_velocity);
        //transform.position += new Vector3(1, 0, 0) * horizontalInput * Time.deltaTime * translational_velocity;
        //CF.rb.velocity = Vector3.zero;
        //transform.Translate(new Vector3(0, 0, 0));
    }
}

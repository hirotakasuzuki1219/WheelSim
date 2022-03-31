using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreForce : MonoBehaviour
{
    public ContactForce CF;
    public Rigidbody rb_core;

    // Start is called before the first frame update
    void Start()
    {
        rb_core = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb_core.AddRelativeForce(CF.Horizontal_Force, CF.Vertical_Force, CF.Side_Force, ForceMode.Force);
        //transform.Translate(new Vector3(0, 0, 0));
    }
}

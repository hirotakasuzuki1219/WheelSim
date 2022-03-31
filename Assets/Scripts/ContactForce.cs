using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class ContactForce : MonoBehaviour
{
    public CoreForce CoreF;

    //入力パラメータ
    public float Radius;
    public float Stiff;
    public float Damp;
    public float Damp_side;
    public float Fric;
    public float Fric_side;

    public Rigidbody rb;

    //計算値
    public float Vertical_Force;
    public float Horizontal_Force;
    public float Side_Force;
    private float Sinkage;
    private float Slip_ratio_x;
    private Vector3 _estimatedAngularVelocity = Vector3.zero;
    private Vector3 _translationPrevious = Vector3.zero;
    private Quaternion _rotationPrevious = Quaternion.identity;
    public float rotational_velo;
    private Vector3 velo;
    private int Slip_state;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Sinkage = -rb.transform.position.y + Radius;
        if (Sinkage > 0)
        {

            //角速度計算
            Quaternion deltaRotation = Quaternion.Inverse(_rotationPrevious) * transform.rotation;
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            float angularSpeed = (angle * Mathf.Deg2Rad) / Time.deltaTime;
            _estimatedAngularVelocity = axis * angularSpeed;
            _rotationPrevious = transform.rotation;//ローカル座標系
            rotational_velo = (-Radius * _estimatedAngularVelocity.y);
            //併進速度計算
            velo.x = (float)(rb.velocity.x * Math.Cos(CoreF.rb_core.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) - rb.velocity.z * Math.Sin(CoreF.rb_core.transform.rotation.eulerAngles.y * Mathf.Deg2Rad));//ローカル．負になってしまう．
            velo.z = (float)(rb.velocity.x * Math.Sin(CoreF.rb_core.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) + rb.velocity.z * Math.Cos(CoreF.rb_core.transform.rotation.eulerAngles.y * Mathf.Deg2Rad));
            _translationPrevious = rb.transform.position;

            Vertical_Force = Stiff * Sinkage - Damp * rb.velocity.y;
            //Side_Force =  - Vertical_Force * Fric_side * velo.z - accel_z * Damp_side;
            Side_Force = (float)(-Vertical_Force * Fric_side * Math.Sign(velo.z) * (1 - System.Math.Exp(- Math.Abs(velo.z) / 1)));

            if (rotational_velo > 0)
            {
                //滑り率評価
                if (velo.x > rotational_velo) 
                {
                    Slip_ratio_x = (float)(-1.0 + (rotational_velo / velo.x));
                }
                else if (velo.x <= rotational_velo)
                {
                    Slip_ratio_x = (float)(1.0 - (velo.x / rotational_velo));
                    if (rotational_velo == 0)
                    {
                        Slip_ratio_x = 0;
                    }
                }
                //けん引力評価
                if (rotational_velo == 0 && Slip_ratio_x == 0)
                {
                    Horizontal_Force = 0;
                    Slip_state = 1;
                }
                else if (Slip_ratio_x >= 0)
                {
                    Horizontal_Force = (float)(Vertical_Force * Fric * (1 - System.Math.Exp(-Slip_ratio_x / 0.01)) - Stiff * Sinkage * Sinkage / 2);
                    Slip_state = 2;
                }
                else if (Slip_ratio_x < 0)
                {
                    Horizontal_Force = (float)(Vertical_Force * Fric * Slip_ratio_x);
                    Slip_state = 3;
                }
            }
            else if (rotational_velo == 0)
            {
                if (velo.x > 0)
                {
                    Slip_ratio_x = -1;
                    Horizontal_Force = (float)(Vertical_Force * Fric * Slip_ratio_x);
                    Slip_state = 4;
                    //Horizontal_Force = (float)(- Vertical_Force * Fric * Math.Sign(velo.x));
                }
                else if (velo.x == 0)
                {
                    Slip_ratio_x = 0;
                    Horizontal_Force = 0;
                    Slip_state = 5;
                }
                else if (velo.x < 0)
                {
                    Slip_ratio_x = -1;
                    Horizontal_Force = (float)(-Vertical_Force * Fric * Slip_ratio_x);
                    Slip_state = 6;
                    //Horizontal_Force = (float)(- Vertical_Force * Fric * Math.Sign(velo.x));
                }
            }
            else if (rotational_velo < 0)
            {
                velo.x = -velo.x;
                rotational_velo = -rotational_velo;
                //滑り率評価
                if (velo.x > rotational_velo)
                {
                    Slip_ratio_x = (float)(-1.0 + (rotational_velo / velo.x));
                }
                else if (velo.x <= rotational_velo)
                {
                    Slip_ratio_x = (float)(1.0 - (velo.x / rotational_velo));
                }
                //けん引力評価
                if (Slip_ratio_x >= 0)
                {
                    Horizontal_Force = (float)(-(Vertical_Force * Fric * (1 - System.Math.Exp(-Slip_ratio_x / 0.01)) - Stiff * Sinkage * Sinkage / 2));
                    Slip_state = 7;
                }
                else if (Slip_ratio_x < 0)
                {
                    Horizontal_Force = (float)(-Vertical_Force * Fric * Slip_ratio_x);
                    Slip_state = 8;
                }
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using System;

public class Plane : MonoBehaviour
{
    public Rigidbody Obj;
    public int zrotForce = 1;
    public int MaxRot = 90;
    public int MinRot = -90;
    public int rotupForce = 1;
    public float speed;
    public float speedincrease;
    public float speeddecrease;
    public int Maxspeed;
    public int Minspeed;
    public int takeoffspeed;
    public int lift;
    public int minlift;
    public bool hit = false;

    /// <summary>
    /// ëÄçÏèÛë‘
    /// </summary>
    public enum Ctrl 
    {
        NONE = 0,
        SPEED_UP = 1,
        SPEED_DOWN = 2,
        ROLL = 3,
        PITCH = 4,
    }

    void Start()
    {
        InvokeRepeating("Speed", 1, 1);
    }

    public void Speed()
    {
        Speed(Ctrl.NONE);
    }

    public void Speed(Ctrl ctrl, float val = 0f)
    {
        if (Input.GetKey(KeyCode.Space) || ctrl == Ctrl.SPEED_UP)
        {
            Debug.Log("speed up : " + ctrl.ToString());
            Mathf.Repeat(1, Time.time);
            speed = speed + speedincrease;
        }
        if (Input.GetKey(KeyCode.LeftAlt) || ctrl == Ctrl.SPEED_DOWN)
        {
            Debug.Log("speed down : " + ctrl.ToString());
            Mathf.Repeat(1, Time.time);
            speed = speed - speeddecrease;
        }

        var spd = Obj.velocity.magnitude;
        var rigidbody = Obj.GetComponent<Rigidbody>();

        rigidbody.AddRelativeForce(0, 0, -speed);

        if (ctrl == Ctrl.ROLL)
        {
            var H = val * zrotForce;
            rigidbody.AddRelativeTorque(0, 0, H * (spd / 100));
        }
        if (ctrl == Ctrl.PITCH)
        {
            var V = val * zrotForce;
            rigidbody.AddRelativeTorque(V * (spd / 100), 0, 0);
        }
        
    }

    void Update()
    {
        var spd = Obj.velocity.magnitude;
        var rigidbody = Obj.GetComponent<Rigidbody>();

        rigidbody.AddRelativeForce(0, 0, -speed);

        var H = (Input.GetAxis("Horizontal")) * zrotForce;
        if (H != 0)
        {
            Debug.Log("Horizontal:" + H.ToString());
            rigidbody.AddRelativeTorque(0, 0, H * (spd / 100));
        }

        var V = (Input.GetAxis("Vertical")) * rotupForce;
        if (V != 0)
        {
            Debug.Log("Vertical:" + V.ToString());
            rigidbody.AddRelativeTorque(V * (spd / 100), 0, 0);
        }

        if (Maxspeed <= speed)
        {
            speed = Maxspeed;
        }

        if (Minspeed >= speed)
        {
            speed = Minspeed;
        }

        if (speed < takeoffspeed)
        {
            rigidbody.AddForce(0, minlift, 0);

        }
        if (speed > takeoffspeed)
        {
            rigidbody.AddForce(0, lift, 0);
        }

        if (rigidbody.rotation.z > MaxRot)
        {
            var rot = rigidbody.rotation;
            rigidbody.rotation = new Quaternion(rot.x, rot.y, MaxRot, rot.w);
        }

        if (rigidbody.rotation.z < MinRot)
        {
            var rot = rigidbody.rotation;
            rigidbody.rotation = new Quaternion(rot.x, rot.y, MinRot, rot.w);
        }
    }
}
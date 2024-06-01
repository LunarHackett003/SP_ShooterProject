using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KartTestController : MonoBehaviour
{
    [System.Serializable]
    public class Wheel
    {
        public WheelCollider wheel;
        public float torque;
        public float steerBounds;
        public float brakePower;
        public float steerTime;
        [HideInInspector] public float wheelSteerDampVelocity;
        [HideInInspector] public float wheelTorqueDampVelocity;
    }
    Vector2 moveInput;
    public Wheel[] wheels;
    public Rigidbody rb;
    private void FixedUpdate()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            Wheel w = wheels[i];
            //If the wheel collider is null, skip this Wheel object
            if (!w.wheel)
                continue;
            if(w.torque > 0)
                w.wheel.motorTorque = moveInput.y * w.torque;
            if (w.steerBounds > 0)
                w.wheel.steerAngle = Mathf.SmoothDamp(w.wheel.steerAngle, w.steerBounds * moveInput.x, ref w.wheelSteerDampVelocity, w.steerTime);
        }
    }

    void OnMove(InputValue val)
    {
        moveInput = val.Get<Vector2>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CarMove : MonoBehaviour
{
    [SerializeField] private List<WheelInfo> wheels = new List<WheelInfo>();

    [SerializeField] private float speed = 50f;
    [SerializeField] private float breakForce = 1500f;

    [SerializeField] private float maxAngle = 45f;

    [SerializeField] private float maxRMTWheels = 90;

    [Header("Фары")]
    [SerializeField] private GameObject[] haloLight = new GameObject[4];

    public void ActivatorLight()
    {
        if (haloLight.Length < 1) return;

        if (!haloLight[0].activeInHierarchy)
            for (int i = 0; i < haloLight.Length; i++)
                haloLight[i].SetActive(true);
    }

    public void DeactivatorLight()
    {
        if (haloLight.Length < 1) return;

        if (haloLight[0].activeInHierarchy)
            for (int i = 0; i < haloLight.Length; i++)
                haloLight[i].SetActive(false);
    }

    private Vector3 wPos;
    private Quaternion wRotate;

    public void Move(float gusValue, float rotateValue)
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            if (wheels[i].motor)
            {
                if (wheels[i].leftWheel.rpm > 25) {
                    if (wheels[i].leftWheel.rpm < maxRMTWheels) {
                        wheels[i].leftWheel.brakeTorque = 0;
                        wheels[i].rightWheel.brakeTorque = 0;
                        wheels[i].leftWheel.motorTorque = speed * gusValue;
                        wheels[i].rightWheel.motorTorque = speed * gusValue;
                    }
                    else {
                        wheels[i].leftWheel.brakeTorque = 0;
                        wheels[i].rightWheel.brakeTorque = 0;
                        wheels[i].leftWheel.motorTorque = 0;
                        wheels[i].rightWheel.motorTorque = 0;
                    }
                }
                else {
                    wheels[i].leftWheel.brakeTorque = 0;
                    wheels[i].rightWheel.brakeTorque = 0;
                    wheels[i].leftWheel.motorTorque = speed * gusValue * 3;
                    wheels[i].rightWheel.motorTorque = speed * gusValue * 3;
                }
            }

            if (wheels[i].steering)
            {
                    wheels[i].leftWheel.steerAngle = Mathf.Lerp(wheels[i].leftWheel.steerAngle, rotateValue, 45);
                    wheels[i].rightWheel.steerAngle = Mathf.Lerp(wheels[i].rightWheel.steerAngle, rotateValue, 45);
            }

            if (wheels[i].visualLW != null)
            {
                wheels[i].leftWheel.GetWorldPose(out wPos, out wRotate);
                wheels[i].visualLW.position = wPos;
                wheels[i].visualLW.rotation = wRotate;
            }

            if (wheels[i].visualRW != null)
            {
                wheels[i].rightWheel.GetWorldPose(out wPos, out wRotate);
                wheels[i].visualRW.position = wPos;
                wheels[i].visualRW.rotation = wRotate;
            }
        }
    }

    public void StopCar()
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            if (wheels[i].motor)
            {
                wheels[i].leftWheel.brakeTorque = breakForce;
                wheels[i].rightWheel.brakeTorque = breakForce;

                wheels[i].leftWheel.motorTorque = 0;
                wheels[i].rightWheel.motorTorque = 0;
            }
        }
    }
}

[System.Serializable]
public class WheelInfo {
    public WheelCollider leftWheel;
    public Transform visualLW;
    public WheelCollider rightWheel;
    public Transform visualRW;
    public bool motor;
    public bool steering;
}
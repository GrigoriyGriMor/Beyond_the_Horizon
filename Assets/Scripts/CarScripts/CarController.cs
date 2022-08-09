using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public GameObject leftWheelVisuals;
    public WheelCollider rightWheel;
    public GameObject rightWheelVisuals;
    public bool motor;
    public bool steering;
    public void ApplyLocalPositionToVisuals()
    {
        //left wheel
        if (leftWheelVisuals == null)
        {
            return;
        }
        Vector3 position;
        Quaternion rotation;
        leftWheel.GetWorldPose(out position, out rotation);

        leftWheelVisuals.transform.position = position;
        leftWheelVisuals.transform.rotation = rotation;

        //right wheel
        if (rightWheelVisuals == null)
        {
            return;
        }
        rightWheel.GetWorldPose(out position, out rotation);
        rightWheelVisuals.transform.position = position;
        rightWheelVisuals.transform.rotation = rotation;
    }
}
public class CarController : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float maxBrakeTorque;

    [HideInInspector]
    public bool brakeCar;
    //[HideInInspector]
    public float motor;
    //[HideInInspector]
    public float steering;
    private float brakeTorque;

    public void FixedUpdate()
    {
       float motor = maxMotorTorque * this. motor;
       float steering = maxSteeringAngle * this.steering;
        brakeTorque = maxBrakeTorque;

        if (!brakeCar)
        {
            brakeTorque = 0;
        }

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;

                axleInfo.rightWheel.brakeTorque = brakeTorque;
                axleInfo.leftWheel.brakeTorque = brakeTorque;

            }
            axleInfo.ApplyLocalPositionToVisuals();
        }
    }
}

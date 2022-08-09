using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointMission : MonoBehaviour
{
    [SerializeField]
    private Transform targetPointMission;
  
    public void SetTarget(Transform target)
    {
        targetPointMission = target;
    }

}

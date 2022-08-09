using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMController : CharacterBase
{
    [SerializeField] private Transform[] spineBoneArray = new Transform[1];

    [SerializeField] private float minVerticalAngle = -45;
    [SerializeField] private float maxVerticalAngle = 60;

}

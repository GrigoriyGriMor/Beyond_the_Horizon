using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balrond3PersonMovements
{
    public class Balrond3pCameraFollow : MonoBehaviour
    {

        [Header("Target to follow")]
        public Transform target;
        [Header("Target's height")]
        public float setTargetHeight;
        [Header("Distance")]
        public float maxDistance = 2.0f;
        public float minDistance = 1.0f;
        [Header("Zoom speed")]
        public float smooth = 10.0f;


        // Start is called before the first frame update
        void Start()
        {
            transform.position = target.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = target.position;
            transform.position += new Vector3(0, setTargetHeight, 0);
        }
    }
}

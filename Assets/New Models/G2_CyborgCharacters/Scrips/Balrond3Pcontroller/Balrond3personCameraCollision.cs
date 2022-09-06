using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Balrond3PersonMovements
{

    public class Balrond3personCameraCollision : MonoBehaviour
    {
        private Vector3 dollyDir;
        private Vector3 dollyDirAdjusted;
        private Balrond3pCameraFollow follow;
        private Balrond3pMainCamera cam;

        void Awake()
        {
            follow = transform.parent.parent.GetComponent<Balrond3pCameraFollow>();
            cam = transform.parent.GetComponent<Balrond3pMainCamera>();
            dollyDir = transform.parent.localPosition;
        }
        // Update is called once per frame
        void FixedUpdate()
        {
            Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * follow.maxDistance);
            RaycastHit hit;

            if (Physics.Linecast(transform.parent.localPosition, desiredCameraPos, out hit))
            {
                if (transform.localPosition.z <= follow.minDistance && !hit.transform.name.Equals(follow.target.transform.gameObject.name) && !hit.transform.gameObject.name.Equals(transform.gameObject.name) && !hit.transform.gameObject.name.Equals(cam.transform.gameObject.name))
                {

                    transform.localPosition += new Vector3(0, 0, follow.smooth * Time.deltaTime);
                }
            }
            else
            {
                if (-follow.maxDistance < transform.localPosition.z)
                {
                    transform.localPosition -= new Vector3(0, 0, follow.smooth * Time.deltaTime);
                }
            }
        }
    }
}
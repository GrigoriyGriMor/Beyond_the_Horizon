using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRagdoll : MonoBehaviour
{
    [SerializeField] private Rigidbody[] AllRigibody;

    private Animator _animator;

    private void Start()
    {
        for (int i = 0; i < AllRigibody.Length; i++)
            AllRigibody[i].isKinematic = true;

        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_animator.enabled)
            {
                for (int i = 0; i < AllRigibody.Length; i++)
                    AllRigibody[i].isKinematic = false;

                _animator.enabled = false;
            }

            else
            {
                for (int i = 0; i < AllRigibody.Length; i++)
                    AllRigibody[i].isKinematic = true;

                _animator.enabled = true;
            }
        }
    }
}

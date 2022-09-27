using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRagdoll : MonoBehaviour
{
    public Rigidbody[] AllRigibody;

    private Animator _animator;

    private void Start()
    {
        for (int i = 0; i < AllRigibody.Length; i++)
            AllRigibody[i].isKinematic = true;

        _animator = GetComponent<Animator>();
    }
}

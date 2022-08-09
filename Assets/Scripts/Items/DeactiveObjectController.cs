using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveObjectController : MonoBehaviour {
    [SerializeField] private float destroyTime = 2;

    [Header("FORCE")]
    [SerializeField] private float forwardForce = 50.0f;

    [SerializeField] private bool needRoatate = false;
    [SerializeField] private float rotateForce = 5.0f;

    private Rigidbody _rb;

    [SerializeField] private bool loot = false;

    private void Awake() {
        if (!loot) _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable() {
        if (loot) {
            StartCoroutine(TimerDestroy());
            return;
        }
        else {
            if (_rb != null) _rb.isKinematic = false;
            StartCoroutine(Startmove());
        }
    }

    private IEnumerator Startmove() {
        yield return new WaitForFixedUpdate();

        if (_rb != null) {
            _rb.AddForce(transform.forward * forwardForce);
            if (needRoatate) _rb.AddTorque(new Vector3(0, rotateForce, rotateForce));
        }
        StartCoroutine(TimerDestroy());
    }

    [SerializeField] private bool destroyOrDeactive = false;
    private IEnumerator TimerDestroy() {
        yield return new WaitForSeconds(destroyTime);

        if (_rb != null) _rb.isKinematic = true;
        if (!destroyOrDeactive) {
            gameObject.SetActive(false);
            transform.position = Vector3.zero;
        }
        else Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (loot) return;

        if (other.gameObject.layer == 2) return;

        if (_rb != null) _rb.isKinematic = true;
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision) {
        if (loot) return;

        if (_rb != null) _rb.isKinematic = true;
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}

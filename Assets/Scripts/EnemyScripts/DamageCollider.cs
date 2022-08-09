
/// скрипт висит на коллайдере который наносит урон противнику
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{

    [SerializeField]
    private float damageAttack = 10;

    [SerializeField]
    private EnemyControllerNear enemyController;

    private RaycastHit hit;

    private void Awake()
    {
        enemyController = GetComponentInParent<EnemyControllerNear>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || enemyController == null) return;

        PlayerController playerController = other.GetComponent<PlayerController>();

        InDamageModule inDamageModule = other.GetComponent<InDamageModule>();

        if ((playerController && inDamageModule) && enemyController.stateEnemy == StateEnemy.attack)
        {
            inDamageModule.InDamage(damageAttack, hit, transform);
        }
    }

}

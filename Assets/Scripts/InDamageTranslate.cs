using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDamageTranslate : MonoBehaviour
{
    [SerializeField] private InDamageModule DamageModule;
    [SerializeField] private float DamageFloating;

    public void StartInDamageModule(float damage, float Impulse, RaycastHit hit, Transform objectDamage = null, Transform muzzlePoint = null)
    {
        float _damage = damage * DamageFloating;

        DamageModule.InDamage(_damage, hit, Impulse, objectDamage, muzzlePoint);
    }
}

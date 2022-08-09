using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModule : CharacterBase
{
    [SerializeField] private string[] panchTriggerName = new string[3];
    [SerializeField] private int bodyUpLayer = 2;
    private Coroutine punchAttack;

    public void InAttack(WeaponController weapon)
    {
        if (weapon == null)
        {
            if (punchAttack == null)
                punchAttack = StartCoroutine(PunchAttack());
        }
        else
        {
            weapon.Fire();
        }
    }

    private IEnumerator PunchAttack()
    {
        playerAnim.SetTrigger(panchTriggerName[Random.Range(0, panchTriggerName.Length)]);

        float time = 0;
        while (time < 1)
        {
            time += 0.02f;
            yield return new WaitForFixedUpdate();
        }

        punchAttack = null;
    }

}

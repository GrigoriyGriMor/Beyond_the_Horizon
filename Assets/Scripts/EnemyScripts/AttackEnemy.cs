using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class AttackEnemy : MonoBehaviour
{

    [Header("Мин расстояние для прицеливания")]
    public float distAiming = 10;

    public string aiming;
    public string reload;
    public string oneShoot;

    public EnemyAnimator enemyAnimator;
    //public Animator animator;

    [SerializeField]
    private Attack[] attackTriggerName = new Attack[4];

    [SerializeField] private int bodyUpLayer = 1;

    private Coroutine punchAttack;

    private Coroutine timerReload;

    [SerializeField]
    private WeaponController usedWeapon;

    private Rig rigObject;               // Rig обьекта

    [SerializeField]
    private float currentRigObjectWeight;

    [SerializeField]
    private float timerAttack;

    public WeaponController GetWeaponRef() {
        return usedWeapon;
    }

    void Start()
    {
        rigObject = GetComponentInChildren<Rig>();

        usedWeapon = GetComponentInChildren<WeaponController>();

        if (usedWeapon)
        {
            usedWeapon.reload.AddListener(() =>
            {
                WeaponReloadInfo r = usedWeapon.UseReloadGun();

                Reload(r);
            });
        }

    }

    /// <summary>
    /// Атака
    /// </summary>
    public float UpdateAttack(float distToEnemy)
    {
        if (usedWeapon)
        {
            if (distToEnemy > distAiming)
            {
                enemyAnimator.SetStateAnimator(aiming, true);
            }
            else
            {
                enemyAnimator.SetStateAnimator(aiming, false);
            }
        }

        InAttack(usedWeapon);

        return timerAttack;

    }

    /// <summary>
    /// Атака
    /// </summary>
    /// <param name="weapon"></param>
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

            enemyAnimator.SetStateAnimator(oneShoot);                                  //   anim oneShoot
            if (enemyAnimator.animator.GetLayerWeight(bodyUpLayer) == 0) enemyAnimator.SetAnimatorLayerWeight(bodyUpLayer, 1);                          // вкл слой anim body
        }
    }

    /// <summary>
    /// Перезарядка анимация
    /// </summary>
    public void Reload(WeaponReloadInfo info)
    {
        currentRigObjectWeight = rigObject.weight;    //      Изменяем 

        rigObject.weight = 0;                        //          вес рига 

        enemyAnimator.SetStateAnimator(reload);

       timerReload = StartCoroutine(SetRig(info.reloadTime));
    }

    private IEnumerator PunchAttack()
    {
        Attack randomAttackAnim = attackTriggerName[Random.Range(0, attackTriggerName.Length)];

        timerAttack = randomAttackAnim.timeAnim;


        enemyAnimator.SetStateAnimator(randomAttackAnim.attack);

        yield return new WaitForSeconds(timerAttack);

        punchAttack = null;
    }


    /// <summary>
    /// Изменяем вес рига после перезарядки
    /// </summary>
    /// <param name="reloadTime"></param>
    /// <returns></returns>
    private IEnumerator SetRig(float reloadTime)
    {
        yield return new WaitForSeconds(reloadTime);

        rigObject.weight = currentRigObjectWeight;
    }


    [System.Serializable]
    public class Attack
    {
        public string attack;
        public float timeAnim;
    }
}

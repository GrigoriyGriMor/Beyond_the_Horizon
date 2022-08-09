using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldAndHealSkillController : SkillBase
{
    [SerializeField] private ParticleSystem healPartical;
    [SerializeField] private ParticleSystem shieldParticle;

    [SerializeField] private float skillCastTime = 2;

    [SerializeField] private int healPoint = 15;

    Coroutine coroutine;

    public override void UseSkill() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
            
        coroutine = StartCoroutine(UseSkillCor());
    }

    private IEnumerator UseSkillCor() {
        playerAnim.SetTrigger("UseSkill_1");
        _rb.velocity = Vector3.zero;
        _player.gameIsPlayed = false;

        _player.SetNewBoneWeight(0);

        while (Mathf.Abs(1 - playerAnim.GetLayerWeight(3)) > 0.1f) {
            playerAnim.SetLayerWeight(3, Mathf.LerpUnclamped(playerAnim.GetLayerWeight(3), 1, 10 * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        healPartical.gameObject.SetActive(true);
        shieldParticle.gameObject.SetActive(true);

        InDamageModule module = _player.GetComponent<InDamageModule>();
        float castTime = 0;

        while (castTime < skillCastTime) {
            module.InHealing(healPoint * Time.fixedDeltaTime);
            _rb.velocity = Vector3.zero;
            playerAnim.transform.localEulerAngles = Vector3.zero;
            castTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        playerAnim.SetTrigger("EndCastSkill");

        healPartical.gameObject.SetActive(false);
        shieldParticle.gameObject.SetActive(false);

        if (_player.GetMode() == SupportClass.PlayerStateMode.Combat)
            _player.SetNewBoneWeight(1);

        while (Mathf.Abs(playerAnim.GetLayerWeight(3) - 0) > 0.1f) {
            playerAnim.SetLayerWeight(3, Mathf.LerpUnclamped(playerAnim.GetLayerWeight(3), 0, 10 * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        if (_player.GetComponent<InDamageModule>().GetHeal() > 0) _player.gameIsPlayed = true;
        coroutine = null;
    }

    public void BreakSkill() {
        StopCoroutine(coroutine);
        playerAnim.SetTrigger("EndCastSkill");

        healPartical.gameObject.SetActive(false);
        shieldParticle.gameObject.SetActive(false);

        if (_player.GetMode() == SupportClass.PlayerStateMode.Combat)
            _player.SetNewBoneWeight(1);

        playerAnim.SetLayerWeight(3, 0);

        if (_player.GetComponent<InDamageModule>().GetHeal() > 0) _player.gameIsPlayed = true;
        coroutine = null;
    }
}

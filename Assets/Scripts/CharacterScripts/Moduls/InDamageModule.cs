using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.ProBuilder.Shapes;

public class InDamageModule : CharacterBase {
    [Header("Heal Point")]
    [SerializeField] private float currentHeal = 100.0f;
    [SerializeField] private float maxHeal = 100.0f;
    [SerializeField] private Text healText;

    [Header("Shield Point")]
    [SerializeField] private float currentShield = 20.0f;
    [SerializeField] private float maxShield = 20.0f;
    [SerializeField] private Text shieldText;

    [Header("Visualiling")]
    [SerializeField] private ParticleSystem inDamageShieldParticle;
    [SerializeField] private ParticleSystem inDamageParticle;
    [SerializeField] private Slider healPointBar;
    [SerializeField] private Slider shieldPointBar;

    [SerializeField] private ParticleSystem shieldDestroyParticle;

    public UnityEvent deach;

    /// Обьект который нанес Урон
    private Transform objectDamage;

    [SerializeField] private Volume damageVignette;
    private Vignette bloom;
    private float maxVegnetteValue = 0.5f;

    [Header("Скорость востановления хп")]
    [SerializeField] private float healRegen = 15.0f;
    [SerializeField] private float healRegenTime = 5.0f;
    [SerializeField] private ParticleSystem healParticle;
    [SerializeField] private Rigidbody[] AllRagdoll;

    private float impulse;
    private Transform shotPosition;

    private Animator _curentAminator;
    private RaycastHit hit;

    private void Start() {
        if (damageVignette) {
            damageVignette.profile.TryGet(out bloom);
            bloom.intensity.value = 0;
        }

        currentHeal = maxHeal;
        //healPointBar.maxValue = maxHeal;
        //healPointBar.value = currentHeal;
        if (healText) healText.text = currentHeal.ToString();

        currentShield = maxShield;
        //shieldPointBar.maxValue = maxShield;
        //shieldPointBar.value = currentShield;
        if (shieldText) shieldText.text = currentShield.ToString();

        StartCoroutine(WaitStart());
    }

    bool gameIsPlayer = false;
    private IEnumerator WaitStart() {
        yield return new WaitForSeconds(0.5f);

        gameIsPlayer = true;
        StartCoroutine(HealRegeniration());
    }

    float lastShield;
    public void InDamage(float damage, RaycastHit hit, float impulse = 0, Transform objectDamage = null, Transform shotPosition = null) {

        if (impulse != 0)
            this.impulse = impulse;

        if (shotPosition != null)
            this.shotPosition = shotPosition;

        this.hit = hit;

        Debug.Log(0);

        if (currentHeal <= 0) return;
        if (objectDamage) this.objectDamage = objectDamage;     // Solo

        lastShield = currentShield;

        if (currentShield <= 0)
            currentHeal -= damage;
        else {
            currentShield -= damage * 0.75f;
            currentHeal -= damage * 0.25f;
        }

        if (damageVignette) {
            float pecent = (healPointBar.value * 100) / maxHeal;
            bloom.intensity.value = maxVegnetteValue - (maxVegnetteValue * (pecent / 100));
        }

        if (healPointBar != null) healPointBar.value = currentHeal;
        if (healText) healText.text = Mathf.CeilToInt(Mathf.Clamp(currentHeal, 0, maxHeal)).ToString();

        if (shieldPointBar != null) shieldPointBar.value = currentShield;
        if (shieldText) shieldText.text = Mathf.CeilToInt(Mathf.Clamp(currentShield, 0, maxShield)).ToString();

        if (currentHeal <= 0)
            Deach();


        if (hit.collider != null) {
            if (lastShield > 0) {
                if (inDamageShieldParticle != null) {
                    inDamageShieldParticle.transform.position = hit.point;
                    inDamageShieldParticle.Play();
                }
            }
            else {
                if (inDamageParticle != null) {
                    inDamageParticle.transform.position = hit.point;
                    inDamageParticle.Play();
                }
            }
        }

        if ((lastShield > 0 && currentShield <= 0) && shieldDestroyParticle != null)
            shieldDestroyParticle.Play();
    }

    public void InHealing(float healPoint, bool shield = false) {
        if (currentHeal <= 0) return;

        if (shield) {
            currentShield = Mathf.Clamp(currentShield + healPoint, 0, maxShield);

            if (shieldPointBar != null) shieldPointBar.value = currentShield;
            if (shieldText) shieldText.text = Mathf.CeilToInt(Mathf.Clamp(currentShield, 0, maxShield)).ToString();
        }
        else {
            currentHeal = Mathf.Clamp(currentHeal + healPoint, 0, maxHeal);

            if (damageVignette) {
                float pecent = (healPointBar.value * 100) / maxHeal;
                bloom.intensity.value = maxVegnetteValue - (maxVegnetteValue * (pecent / 100));
            }

            if (healPointBar != null) healPointBar.value = currentHeal;
            if (healText) healText.text = Mathf.CeilToInt(Mathf.Clamp(currentHeal, 0, maxHeal)).ToString();
        }
    }

    private IEnumerator HealRegeniration() {
        yield return new WaitForSeconds(healRegenTime);

        if (currentHeal > 0 && currentHeal != maxHeal) {
            currentHeal = Mathf.Clamp((currentHeal + healRegen), 0, maxHeal);
            if (healParticle) healParticle.Play();

            if (healPointBar != null) healPointBar.value = currentHeal;
            if (healText) healText.text = Mathf.CeilToInt(Mathf.Clamp(currentHeal, 0, maxHeal)).ToString();

            if (damageVignette) {
                float pecent = (healPointBar.value * 100) / maxHeal;
                bloom.intensity.value = maxVegnetteValue - (maxVegnetteValue * (pecent / 100));
            }
        }

        StartCoroutine(HealRegeniration());
    }

    public void InDamageAfterFall(float damage) {
        if (currentHeal <= 0) return;

        currentHeal -= damage;

        if (damageVignette) {
            float pecent = (healPointBar.value * 100) / maxHeal;
            bloom.intensity.value = maxVegnetteValue - (maxVegnetteValue * (pecent / 100));
        }

        if (healPointBar != null) healPointBar.value = currentHeal;
        if (healText) healText.text = Mathf.CeilToInt(Mathf.Clamp(currentHeal, 0, maxHeal)).ToString();

        if (currentHeal <= 0)
            Deach();
    }


    private void Deach() {
        if (damageVignette) bloom.intensity.value = 0.75f;

        if (GetComponent<Animator>())
            GetComponent<Animator>().enabled = false;

        if (AllRagdoll != null)
        {
            for (int i = 0; i < AllRagdoll.Length; i++)
                AllRagdoll[i].isKinematic = false;
        }

        AddForce(hit);
        deach.Invoke();

        if (playerAnim) {
            playerAnim.SetTrigger("Die");
        }
    }

    private void AddForce(RaycastHit hit)
    {
        Collider col = hit.collider;
        var dir = (hit.point - shotPosition.transform.position);

        Rigidbody _rigidbody = hit.collider.GetComponent<Rigidbody>();
        _rigidbody.AddForce(dir * impulse, ForceMode.Impulse);
    }

    public float GetHeal() {
        return currentHeal;
    }

    public void SetHeal(float newHP) {
        if (!gameIsPlayer || currentHeal <= 0) return;

        currentHeal = newHP;
        if (healPointBar != null) healPointBar.value = currentHeal;
        if (healText) healText.text = Mathf.CeilToInt(Mathf.Clamp(currentHeal, 0, maxHeal)).ToString();

        if (damageVignette) {
            float pecent = (healPointBar.value * 100) / maxHeal;
            bloom.intensity.value = maxVegnetteValue - (maxVegnetteValue * (pecent / 100));
        }

        if (currentHeal <= 0)
            Deach();
    }

    public void SetShield(float newShield) {
        if (!gameIsPlayer || currentHeal <= 0) return;

        currentShield = newShield;
        if (shieldPointBar != null) shieldPointBar.value = currentShield;
        if (shieldText) shieldText.text = Mathf.CeilToInt(Mathf.Clamp(currentShield, 0, maxShield)).ToString();
    }

    public float GetShield() {
        return currentShield;
    }

    public void ReloadParam() {
        if (damageVignette)
            bloom.intensity.value = 0;

        currentHeal = maxHeal;
        currentShield = maxShield;

        SetHeal(maxHeal);
        SetShield(maxShield);
    }

    /// Возвращает Обьект который нанес Урон
    public Transform GetTarget() {
        return objectDamage;
    }
}

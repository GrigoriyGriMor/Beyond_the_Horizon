using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillBase : MonoBehaviour
{
    public SkillParams skillParam;
    [HideInInspector] public SkillsModule module;

    [HideInInspector] public SupportClass.gameState state;
    [HideInInspector] public GameObject visual;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Animator playerAnim;

    [HideInInspector] public Transform spineBone;

    [HideInInspector] public PlayerController _player;

    [HideInInspector] public WeaponModule weaponModule;

    private bool moduleActivate = false;

    public SkillParams Init(SupportClass.gameState _state, GameObject _visual, Rigidbody rb, Animator _playerAnim, SkillsModule _module, Transform _spineBone = null, Camera _mainCamera = null, PlayerController player = null) {
        state = _state;
        visual = _visual;
        mainCamera = _mainCamera;
        _rb = rb;
        playerAnim = _playerAnim;
        module = _module;
        spineBone = _spineBone;
        _player = player;

        moduleActivate = true;

        return skillParam;
    }

    public virtual void UseSkill() { 
    
    }
}

[System.Serializable]
public class SkillParams {
    public Sprite skillImage;
    public float skillCoolDown = 5;
}
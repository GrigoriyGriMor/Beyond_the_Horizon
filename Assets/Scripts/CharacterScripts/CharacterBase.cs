using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    //LoadSetting
    [HideInInspector] public SupportClass.gameState state;
    [HideInInspector] public GameObject visual;
    [HideInInspector] public Camera mainCamera;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Animator playerAnim;

    [HideInInspector] public Transform spineBone;

    [HideInInspector] public PlayerController _player;

    [HideInInspector] public WeaponModule weaponModule;

    private bool moduleActivate = false;

    public void Init(SupportClass.gameState _state, GameObject _visual, Rigidbody rb, Animator _playerAnim, Transform _spineBone = null, Camera _mainCamera = null, PlayerController player = null)
    {
        state = _state;
        visual = _visual;
        mainCamera = _mainCamera;
        _rb = rb;
        playerAnim = _playerAnim;
        spineBone = _spineBone;
        _player = player;

        moduleActivate = true;
    }

    public void Init(SupportClass.gameState _state) {
        state = _state;

        moduleActivate = true;
    }

    public void Init(SupportClass.gameState _state, PlayerController player, WeaponModule _weaponModule) {
        state = _state;
        _player = player;

        weaponModule = _weaponModule;
    }
}

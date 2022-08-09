using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Coursor_Activator : MonoBehaviour
{
    [Header("Отключить активацию по OnEnable")]
    [SerializeField] private bool handSetting = false;

    public void ActivateCoursor() {
        if (CoursorController.Instance) CoursorController.Instance.UI_Object_On();
    }

    public void DeactiveCoursor() {
        if (CoursorController.Instance) CoursorController.Instance.UI_Object_Off();
    }

    private void OnEnable() {
        if (!handSetting && CoursorController.Instance) CoursorController.Instance.UI_Object_On();
    }

    private void OnDisable() {
        if (!handSetting && CoursorController.Instance) CoursorController.Instance.UI_Object_Off();
    }

}

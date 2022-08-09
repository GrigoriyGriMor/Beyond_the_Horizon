using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoursorController : MonoBehaviour
{
    private static CoursorController instance;
    public static CoursorController Instance => instance;

    [SerializeField] private InputPlayerManager inputManager;

    private uint activeUIOjbectCount = 0;

    private void Awake() {
        instance = this;

        if (inputManager == null) inputManager = GetComponent<InputPlayerManager>();
    }


    public void UI_Object_On() {
        activeUIOjbectCount += 1;

        if (activeUIOjbectCount > 0 && Cursor.lockState == CursorLockMode.Locked) {
            SetCursorState(true);
            inputManager.ClientControl(false);
        }
    }

    public void UI_Object_Off() {
        if (activeUIOjbectCount > 0) 
            activeUIOjbectCount -= 1;

        if (activeUIOjbectCount == 0 && Cursor.lockState != CursorLockMode.Locked) {
            SetCursorState(false);
            inputManager.ClientControl(true);
        }
    }

    public void SetCursorState(bool newState) {
        Cursor.lockState = !newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void SetCursorState() {
        Cursor.lockState = (Cursor.lockState != CursorLockMode.Locked) ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

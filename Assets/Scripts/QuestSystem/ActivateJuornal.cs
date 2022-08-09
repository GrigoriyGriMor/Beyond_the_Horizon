using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateJuornal : MonoBehaviour
{
    [SerializeField]
    private Transform journalUI;

    [SerializeField]
    private Transform missions;

    [Header("Control Buttons")]
    public KeyCode juornalButton = KeyCode.J;
    public KeyCode escButton = KeyCode.Escape;

    private Toggle toggle;

    private void Start()
    {
        journalUI.gameObject.SetActive(true);
        missions.TryGetComponent<Toggle>(out toggle);
        journalUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(juornalButton))
        {
            journalUI.gameObject.SetActive(!journalUI.gameObject.activeInHierarchy);
            toggle.isOn = true;
        }

        if (Input.GetKeyDown(escButton))
        {
            journalUI.gameObject.SetActive(false);
        }
    }
}

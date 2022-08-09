using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickStart : MonoBehaviour
{
    [SerializeField]
    private Button buttonNext;
    [SerializeField]
    private GameObject buttonStart;
    [SerializeField]
    private ToggleGroup toggleGroup;
    [HideInInspector]
    public List<Toggle> toggles;
    private int currentIndexToggle = 0;

    private void OnEnable()
    {
        for (int i = 0; i < toggleGroup.transform.childCount; i++)
        {
            Toggle tempToggle = toggleGroup.transform.GetChild(i).GetComponent<Toggle>();
            if (tempToggle)
            {
                toggles.Add(tempToggle);
            }
        }
        currentIndexToggle = 0;
        buttonStart.SetActive(false);
        buttonNext.gameObject.SetActive(true);
        toggles[currentIndexToggle].isOn = true;
        ChangeToggle();
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("FirstGame")) {
            menu.SetActive(true);
            PlayerPrefs.SetInt("FirstGame", 1);
        }
        else { 
            menu.SetActive(false);
            if (CoursorController.Instance) CoursorController.Instance.UI_Object_Off();
        }

        if (buttonNext) buttonNext.onClick.AddListener(() => ButtonNextClick());
        toggles[toggles.Count - 1].onValueChanged.AddListener(delegate { ChangeToggle(); });
    }

    [SerializeField] private GameObject menu;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.F10)) { 
            if (!menu.activeInHierarchy) 
                menu.SetActive(true);
            else
                menu.SetActive(false);
        }
    }

    private void ChangeToggle()
    {
        if (toggles.Count > 0)
        {
            if (toggles[toggles.Count - 1].isOn)
            {
                if (buttonStart)
                {
                    buttonStart.SetActive(true);
                    buttonNext.gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                buttonStart.SetActive(false);
                buttonNext.gameObject.SetActive(true);
            }
        }
    }

    private void ButtonNextClick()
    {
        //if (toggles.Count > 0)
        //{
        //    if (toggles[toggles.Count - 1].isOn)
        //    {
        //        if (buttonStart)
        //        {
        //            buttonStart.SetActive(true);
        //            buttonNext.gameObject.SetActive(false);
        //            return;
        //        }
        //    }
        //}

        for (int i = 0; i < toggles.Count; i++)
        {
            if (toggles[i] == toggleGroup.GetFirstActiveToggle()) currentIndexToggle = i;
        }
        currentIndexToggle++;
        if (currentIndexToggle > toggles.Count - 1) currentIndexToggle = toggles.Count - 1;
        toggles[currentIndexToggle].isOn = true;
    }
}

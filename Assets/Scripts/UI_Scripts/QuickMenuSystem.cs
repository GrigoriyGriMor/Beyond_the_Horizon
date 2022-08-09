using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickMenuSystem : MonoBehaviour
{
    [Header("Quick Menu")]
    [SerializeField] private GameObject quickMenu;
    
    [Header("Toogles")]
    [SerializeField] private Toggle map;
    [SerializeField] private Toggle mission;
    [SerializeField] private Toggle inventory;
    [SerializeField] private Toggle store;
    [SerializeField] private Toggle islandStore;

    [Header("exitMenu")]
    [SerializeField] private GameObject SettingMenu;
    [SerializeField] private Button ExitGame;

    private void Start() {
        map.isOn = true;

        ExitGame.onClick.AddListener(() => ExitThisGame());
    }

    //tab
    public void UseQuickMenu() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
        }
        else {
            quickMenu.SetActive(false);
        }
    }

    public void UseMap() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
            map.isOn = true;
        }
        else {
            if (map.isOn)
                quickMenu.SetActive(false);
            else
                map.isOn = true;
        }
    }

    public void UseMission() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
            mission.isOn = true;
        }
        else {
            if (mission.isOn)
                quickMenu.SetActive(false);
            else
                mission.isOn = true;
        }
    }

    public void UseInventory() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
            inventory.isOn = true;
        }
        else {
            if (inventory.isOn)
                quickMenu.SetActive(false);
            else
                inventory.isOn = true;
        }
    }

    public void UseStore() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
            store.isOn = true;
        }
        else {
            if (store.isOn)
                quickMenu.SetActive(false);
            else
                store.isOn = true;
        }
    }
    public void UseIslandStore() {
        if (!quickMenu.activeInHierarchy) {
            quickMenu.SetActive(true);
            islandStore.isOn = true;
        }
        else {
            if (islandStore.isOn)
                quickMenu.SetActive(false);
            else
                islandStore.isOn = true;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (quickMenu.activeInHierarchy)
                quickMenu.SetActive(false);
            else {
                if (ExitGame.gameObject.activeInHierarchy) {
                    SettingMenu.SetActive(false);
                }
                else {
                    SettingMenu.SetActive(true);
                }
            }
        }
    }

    public void ExitThisGame() {
        Application.Quit();
    }
}

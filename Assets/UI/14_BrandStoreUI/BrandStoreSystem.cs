using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrandStoreSystem : MonoBehaviour {
    [SerializeField] private Button B_OpenMainWindows;
    [SerializeField] private Button B_OpenClientGoodsWindows;

    [SerializeField] private GameObject mainWindows;
    [SerializeField] private GameObject clientGoodsWindows;

    [SerializeField] private KeyCode key_1 = KeyCode.LeftControl;
    [SerializeField] private KeyCode key_2 = KeyCode.M;

    private PlayerSessionData player_data = new PlayerSessionData();

    private void Awake() {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        B_OpenMainWindows.onClick.AddListener(() => {
            clientGoodsWindows.SetActive(false);
            mainWindows.SetActive(true); 
        });

        B_OpenClientGoodsWindows.onClick.AddListener(() => {
            clientGoodsWindows.SetActive(true);
            mainWindows.SetActive(false);
        });
    }

    public void Init(int _playerID, string playerSession) {
        player_data.player_id = _playerID;
        player_data.player_session = playerSession;

        mainWindows.GetComponent<MainWindows>().Init(player_data);
        clientGoodsWindows.GetComponent<ClientGoodsWindows>().Init(player_data);
    }

    private void Update() {
        if ((Input.GetKey(key_1) && Input.GetKeyDown(key_2)) && !mainWindows.activeInHierarchy) {
            clientGoodsWindows.SetActive(false);
            mainWindows.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && (mainWindows.activeInHierarchy || clientGoodsWindows.activeInHierarchy)) {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void OpenWindowsWithSelectbleParam(BillboardInGameInfo info) {
        clientGoodsWindows.SetActive(false);
        mainWindows.SetActive(true);

        mainWindows.GetComponent<MainWindows>().SetFillterParam(info.brand_name, (info.good_id != "") ? int.Parse(info.good_id) : -1);
    }
}

[System.Serializable]
public class PlayerSessionData {
    public int player_id;
    public string player_session;
}
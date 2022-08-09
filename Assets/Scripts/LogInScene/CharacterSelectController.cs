using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectController : MonoBehaviour
{
    [SerializeField] private GameObjectData ScrObjGameObjectData;

    [Header("Buttons")]
    [SerializeField] private Button b_EnterGame;
    [SerializeField] private Button b_GoToLogIn;

    [Header("Character Visual")]
    [SerializeField] private CharacterVisualForMenu[] visuals = new CharacterVisualForMenu[4];

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    [HideInInspector]
    public AuthWindowsController mainController;

    [Header("Подключение")]
    [SerializeField] private GameObject ClientController;

    #region main function
    private void Awake() {
        errorText.text = "";
        errorPanel.SetActive(false);
        b_EnterGame.onClick.AddListener(() => EnterGame());
        b_GoToLogIn.onClick.AddListener(() => GoToLogInWindows());
    }

    private void EnterGame() {
        if (LoadScreenController.Instance) LoadScreenController.Instance.LoadScreenActive();

        for (int i = 0; i < visuals.Length; i++) {
            if (visuals[i].toggle.isOn) {
                GameObject client = Instantiate(ClientController, Vector3.zero, Quaternion.identity);
                client.GetComponent<ClientDataInput>().Init(mainController.GetResponseData(), charactersData.data[i], charactersData.server_id);
                DontDestroyOnLoad(client);

                SceneManager.LoadScene(1);
            }
        }
        //выбираем нужный тоггле и заходим на сервер создаем префаб игрока
    }

    private void ClearThisWindows() {
        for (int i = 0; i < visuals.Length; i++) {
            visuals[i].AddNewCharacter.onClick.RemoveAllListeners();
            visuals[i].AddNewCharacter.gameObject.SetActive(false);

            visuals[i].characterName.text = "";
            visuals[i].HealPoint.text = "";
            visuals[i].ShieldPoint.text = "";
            visuals[i].level.text = "";

            visuals[i].mainPanel.SetActive(true);

            if (visuals[i].visual_pos.childCount > 0)
                Destroy(visuals[i].visual_pos.GetChild(0).gameObject);
        }
    }

    private void GoToLogInWindows() {
        ClearThisWindows();
        mainController.OpenWindow(SupportClass.windows.logIn);
    }

    public void CreateCharacterNEW() {
        ClearThisWindows();
        mainController.OpenWindow(SupportClass.windows.createCharacter);
    }
    #endregion

    private PlayerCharacterInServerData charactersData;
    #region initialization class
    public void Init(PlayerCharacterInServerData playerCharacters) {
        if (playerCharacters.data.Length <= 0)
            b_EnterGame.interactable = false;
        else
            b_EnterGame.interactable = true;

        charactersData = playerCharacters;

        //проходимся по всем элементам в UI и подставляем данные в поля включая визуал, который будет отображаться
        if (playerCharacters.data.Length <= visuals.Length) {
            for (int i = 0; i < visuals.Length; i++) {
                if (i < playerCharacters.data.Length) {
                    visuals[i].mainPanel.gameObject.SetActive(true);
                    visuals[i].AddNewCharacter.gameObject.SetActive(false);
                    visuals[i].panel.SetActive(true);
                    visuals[i].toggle.interactable = true;

                    visuals[i].characterName.text = playerCharacters.data[i].character_name;
                    visuals[i].HealPoint.text = playerCharacters.data[i].character_heal.ToString();
                    visuals[i].ShieldPoint.text = playerCharacters.data[i].character_shield.ToString();
                    visuals[i].level.text = playerCharacters.data[i].character_level.ToString();

                    //тут должна быть так же подгрузка данных о визуале персонажа
                    Instantiate(ScrObjGameObjectData.playerObjects[playerCharacters.data[i].body_type].playerPrefabForUI, 
                        visuals[i].visual_pos.position, visuals[i].visual_pos.rotation, visuals[i].visual_pos);
                }
                else {
                    visuals[i].mainPanel.gameObject.SetActive(false);
                    visuals[i].toggle.interactable = false;
                    visuals[i].panel.SetActive(false);

                    visuals[i].AddNewCharacter.gameObject.SetActive(true);
                    visuals[i].AddNewCharacter.onClick.AddListener(() => CreateCharacterNEW());
                }
            }
        }
        else {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Персонажей на сервере больше, чем доступно на клиенте, КРИТИЧЕСКАЯ ОШИБКА");
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Ошибка клиента, просьба сообщить об ошибке по почте support@cloudsgoods.com";
        }
    }
    #endregion
}

[System.Serializable]
public class CharacterVisualForMenu {
    [SerializeField] public string fieldName;

    [Header("Camera panel GO")]
    public GameObject panel;
    public Transform visual_pos;

    [Header("main panel")]
    public Toggle toggle;
    public GameObject mainPanel;

    public Text characterName;
    public Text level;
    public Text HealPoint;
    public Text ShieldPoint;

    [Header("add panel")]
    public Button AddNewCharacter;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class CharacterCreateController : MonoBehaviour
{
    [Header("Основные Кнопки")]
    [SerializeField] private Button b_GoToCharacterSelect;
    [SerializeField] private Button b_CreateCharacter;

    [Header("Индекс модели расы и пола")]
    [SerializeField] private int human_man = 0;
    [SerializeField] private int human_woman = 1;
    [SerializeField] private int robot_man = 2;
    [SerializeField] private int robot_woman = 3;
    [SerializeField] private int default_model = 4;

    [SerializeField] private Text CharacterName;

    [Header("VisualForCamera Pos")]
    [SerializeField] private Transform visual_pos;
    [SerializeField] private GameObjectData ScrObj_CharacterData;

    [Header("Слоты выбора")]
    [SerializeField] private ToggleGroup headSlotsBlock; 
    [SerializeField] private GameObject headSelectPanel;

    [SerializeField] private ToggleGroup hairSlotsBlock;
    [SerializeField] private GameObject hairSelectPanel;

    [HideInInspector]
    public AuthWindowsController mainController;

    [Header("Params")]
    [SerializeField] private Text healCountText;
    [SerializeField] private Text shieldCountText;
    [SerializeField] private Text speedCountText;

    private int selectbleRaceType = 0;
    private int selectbleGenderType = 0;
    private int selectbleHeadType = 0;
    private int selectbleHairType = 0;
    private int selectbleBodyType = 0;

   [SerializeField] private CharacterVisualController currentModel;

    private EnterGameData connectData;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    private void Start() {
        b_GoToCharacterSelect.onClick.AddListener(() => StartCoroutine(GoToCharacterSelect()));
        b_CreateCharacter.onClick.AddListener(() => StartCoroutine(CreateNewCharacter()));
    }

    private void OnEnable() {
        connectData = mainController.GetResponseData();
        LoadVisual();
    }

    private IEnumerator GoToCharacterSelect() {
        b_CreateCharacter.interactable = true;

        using (UnityWebRequest www = UnityWebRequest.Get(WebData.ServerCharInfoPath + $"?id={connectData.id}&x-session={connectData.session}&server_id={mainController.PlayerCharacters.server_id}")) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "CharacterCreateController");
                Debug.LogError("ServerSelectController " + www.error);
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + www.error;
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "CharacterCreateController ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + "www.downloadHandler.text.Length < 1";
                www.Dispose();
                yield break;
            }

            Debug.LogError(www.downloadHandler.text);
            PlayerCharacterInServerData data = JsonUtility.FromJson<PlayerCharacterInServerData>(www.downloadHandler.text);
            data.server_id = mainController.PlayerCharacters.server_id;

            if (!data.success) {
                Debug.LogError(" SOME ERROR " + data.message + " |error code: " + data.code);
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(data.message + " |error code: " + data.code, "RegisterController ");
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + data.message + " |error code: " + data.code;
                www.Dispose();
                yield break;
            }

            // отправляем запрос с данными на сервер и ждем подтверждения
            mainController.SetServerCharacterInfo(data);

            yield return new WaitForFixedUpdate();
            ClearWindows();
            yield return new WaitForFixedUpdate();

            mainController.OpenWindow(SupportClass.windows.selectCharacter);
            www.Dispose();
        }
    }

    private IEnumerator CreateNewCharacter() {
        if (selectbleBodyType == default_model) {
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Игра еще не готова, подождите еще немного ;)";
            yield break;
        } 

        b_CreateCharacter.interactable = false;

        if (CharacterName.text.Length <= 0) {
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Вы не ввели имя персонажа";
            b_CreateCharacter.interactable = true;
            yield break;
        }

        if (CharacterName.text.Length > 10) {
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Имя персонажа слишком длинное, должно быть менее 10 символов";
            b_CreateCharacter.interactable = true;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("character_name", CharacterName.text);
        form.AddField("body_type", selectbleBodyType);
        form.AddField("head", selectbleHeadType);
        form.AddField("hair", selectbleHairType);

        form.AddField("character_level", 1);
        form.AddField("character_heal", currentModel.GetBaseHeal().ToString());
        form.AddField("character_shield", currentModel.GetBaseShield().ToString());

        form.AddField("jackets", 0);
        form.AddField("pants", 0);
        form.AddField("gloves", 0);
        form.AddField("boots", 0);
        form.AddField("character_state", 0);

        form.AddField("id", connectData.id);
        form.AddField("x-session", connectData.session);
        form.AddField("server_id", mainController.PlayerCharacters.server_id);

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.AddCharacter, form)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "CreateCharacter");
                Debug.LogError("ServerSelectController " + www.error);
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + www.error;
                b_CreateCharacter.interactable = true;

                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "CreateCharacter ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + "www.downloadHandler.text.Length < 1";
                b_CreateCharacter.interactable = true;

                www.Dispose();
                yield break;
            }

            //Debug.LogError(www.downloadHandler.text);
            EnterGameData data = JsonUtility.FromJson<EnterGameData>(www.downloadHandler.text);

            if (!data.success) {
                Debug.LogError(" SOME ERROR " + data.message + " |error code: " + data.code);
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(data.message + " |error code: " + data.code, "RegisterController ");
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + data.message + " | Код ошибки: " + data.code;
                b_CreateCharacter.interactable = true;

                www.Dispose();
                yield break;
            }

            yield return new WaitForFixedUpdate();

            StartCoroutine(GoToCharacterSelect());
            www.Dispose();
        }
    }

    #region setting new character
    public void SetRaceType(int i) {
        selectbleRaceType = i;
        LoadVisual();
    }

    public void SetGenderType(int i) {
        selectbleGenderType = i;
        LoadVisual();
    }

    private void LoadVisual() {
        if (InitModelCoroutin != null) StopCoroutine(InitModelCoroutin);

        switch (selectbleRaceType) {
            case 0://human
                if (selectbleGenderType == 0)// man
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[human_man].playerPrefabForUI));
                    selectbleBodyType = human_man;
                }
                else //woman 
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[human_woman].playerPrefabForUI));
                    selectbleBodyType = human_woman;
                }
                break;
            case 1://robot
                if (selectbleGenderType == 0)// man
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[robot_man].playerPrefabForUI));
                    selectbleBodyType = robot_man;
                }
                else //woman 
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[robot_woman].playerPrefabForUI));
                    selectbleBodyType = robot_woman;
                }
                break;
            case 2://gibrids
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[default_model].playerPrefabForUI));
                    selectbleBodyType = default_model;
                }
                break;
            case 3://other race
                {
                    InitModelCoroutin = StartCoroutine(InitializeNewModel(ScrObj_CharacterData.playerObjects[default_model].playerPrefabForUI));
                    selectbleBodyType = default_model;
                }
                break;
        }
    }

    private Coroutine InitModelCoroutin;
    private IEnumerator InitializeNewModel(GameObject obj) {
        //очищаем данные по старой модели
        if (currentModel != null && currentModel.gameObject != null) {

            for (int i = 0; i < headSlotsBlock.transform.childCount; i++)
                Destroy(headSlotsBlock.transform.GetChild(i));

            for (int i = 0; i < hairSlotsBlock.transform.childCount; i++)
                Destroy(hairSlotsBlock.transform.GetChild(i));

            Destroy(currentModel.gameObject);
            currentModel = null;
        }
        yield return new WaitForFixedUpdate();

        //загружаем новую модель из скриптабл обджекта
        currentModel = Instantiate(obj, visual_pos.position, visual_pos.rotation, visual_pos).GetComponent<CharacterVisualController>();
        healCountText.text = currentModel.GetBaseHeal().ToString();
        shieldCountText.text = currentModel.GetBaseShield().ToString();
        speedCountText.text = currentModel.GetBaseSpeed().ToString();

        //создаем новые плашки для выбора визуала
        Sprite[] spriteArr = currentModel.GetHeadVisual();
        for (int i = 0; i < spriteArr.Length; i++) {
            Toggle toggle = Instantiate(headSelectPanel, Vector2.zero, Quaternion.identity, headSlotsBlock.transform).GetComponent<Toggle>();
            toggle.group = headSlotsBlock;
            toggle.GetComponent<VisualSelectSlot>().SetImage(spriteArr[i]);

            int number = i;
            toggle.onValueChanged.AddListener((value) => {
                currentModel.SelectHead(number);
                selectbleHairType = number;
            });

            if (i == 0) toggle.isOn = true;
        }

        spriteArr = currentModel.GetHairVisual();
        for (int i = 0; i < spriteArr.Length; i++) {
            Toggle toggle = Instantiate(hairSelectPanel, Vector2.zero, Quaternion.identity, hairSelectPanel.transform).GetComponent<Toggle>();
            toggle.group = hairSlotsBlock;
            toggle.GetComponent<VisualSelectSlot>().SetImage(spriteArr[i]);

            int number = i;
            toggle.onValueChanged.AddListener((value) => {
                currentModel.SelectHead(number);
                selectbleHairType = number;
            });
               
            if (i == 0) toggle.isOn = true;
        }

        InitModelCoroutin = null;
    }

    private void ClearWindows() {
        if (currentModel != null && currentModel.gameObject != null) {
            CharacterName.text = "";
            healCountText.text = "";
            shieldCountText.text = "";
            speedCountText.text = "";

            for (int i = 0; i < headSlotsBlock.transform.childCount; i++)
                Destroy(headSlotsBlock.transform.GetChild(i));

            for (int i = 0; i < hairSlotsBlock.transform.childCount; i++)
                Destroy(hairSlotsBlock.transform.GetChild(i));

            Destroy(currentModel.gameObject);// yield return new WaitForFixedUpdate();

            currentModel = null;
        }
    }
    #endregion
}

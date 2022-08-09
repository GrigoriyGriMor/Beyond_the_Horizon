using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ServerSelectController : MonoBehaviour
{
    [Header("Список серверов")]
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private GameObject serverPanel;
    
    [Header("Кнопки")] 
    [SerializeField] private Button b_SelectServer;
    [SerializeField] private Button b_Back;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    [HideInInspector]
    public AuthWindowsController mainController;

    private EnterGameData ServersData;

    public void Init(EnterGameData data) {

        ServersData = data;

        if (data.servers.Length != 0)
            for (int i = 0; i < data.servers.Length; i++) { 
                ServerPanel _object = Instantiate(serverPanel, Vector3.zero, Quaternion.identity, toggleGroup.transform).GetComponent<ServerPanel>();
                _object.GetComponent<Toggle>().group = toggleGroup;
                _object.Init(data.servers[i], mainController);

                if (i == 0) _object.GetComponent<Toggle>().isOn = true;
            }

        b_SelectServer.onClick.AddListener(() => StartCoroutine(ConnectServer()));
        b_Back.onClick.AddListener(() => BackToLogIn());
    }

    private IEnumerator ConnectServer() {
        for (int i = 0; i < toggleGroup.transform.childCount; i++) {
            if (toggleGroup.transform.GetChild(i).GetComponent<Toggle>().isOn) {

                using (UnityWebRequest www = UnityWebRequest.Get(WebData.ServerCharInfoPath + $"?id={ServersData.id}&x-session={ServersData.session}&server_id={ServersData.servers[i].id}")) {
                    www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
                    yield return www.SendWebRequest();

                    if (www.isHttpError || www.isNetworkError) {
                        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "ServerSelectController");
                        Debug.LogError("ServerSelectController " + www.error);
                        errorPanel.SetActive(false);
                        errorPanel.SetActive(true);
                        errorText.text = "Ошибка подключения \n" + www.error;
                        www.Dispose();
                        yield break;
                    }

                    if (www.downloadHandler.text.Length < 1) {
                        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "ServerSelectController ");
                        Debug.LogError("www.downloadHandler.text.Length < 1");
                        errorPanel.SetActive(false);
                        errorPanel.SetActive(true);
                        errorText.text = "Ошибка подключения \n" + "www.downloadHandler.text.Length < 1";
                        www.Dispose();
                        yield break;
                    }

                    //Debug.LogError(www.downloadHandler.text);
                    PlayerCharacterInServerData data = JsonUtility.FromJson<PlayerCharacterInServerData>(www.downloadHandler.text);
                    data.server_id = ServersData.servers[i].id;

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

                    for (int j = 0; j < toggleGroup.transform.childCount; j++)
                        Destroy(toggleGroup.transform.GetChild(j).gameObject);
                    
                    mainController.OpenWindow(SupportClass.windows.selectCharacter);
                    www.Dispose();
                }
            }
        }
    }

    private void BackToLogIn() {
        for (int i = 0; i < toggleGroup.transform.childCount; i++) {
            Destroy(toggleGroup.transform.GetChild(i).gameObject);
        }

        mainController.OpenWindow(SupportClass.windows.logIn);
    }
}

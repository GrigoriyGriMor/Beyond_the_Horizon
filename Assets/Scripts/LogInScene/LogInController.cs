using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LogInController : MonoBehaviour
{
    [Header("Ввод данных для входа")]
    [SerializeField] private InputField Input_logIn;
    [SerializeField] private InputField Input_Password;

    [Header("Кнопки")]
    [SerializeField] private Button b_LogIn;
    [SerializeField] private Button b_Registration;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;

    [HideInInspector]
    public AuthWindowsController mainController;

    private void Awake() {

        if (PlayerPrefs.HasKey("PlayerLogIn"))
            Input_logIn.text = PlayerPrefs.GetString("PlayerLogIn");

        errorText.text = "";
        errorPanel.SetActive(false);
        b_LogIn.onClick.AddListener(() => StartCoroutine(LogInContinue()));
        b_Registration.onClick.AddListener(() => GoToRegistration());
    }

    private IEnumerator LogInContinue() {
        b_LogIn.interactable = false;

        if (Input_logIn.text.Length <= 0 || Input_Password.text.Length <= 0) {
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Поля незаполнены";
            b_LogIn.interactable = true;

            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("login", Input_logIn.text);
        form.AddField("password", Input_Password.text);

        // отправляем запрос с данными на сервер и ждем подтверждения
        // если данные верны сохраняем логин в префсы

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.LoginPath, form)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "LogInController");
                Debug.LogError("LogInController " + www.error);
                b_LogIn.interactable = true;
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + www.error;
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "LogInController ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                b_LogIn.interactable = true;
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + "www.downloadHandler.text.Length < 1";
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
                b_LogIn.interactable = true;
                errorText.text = "Ошибка подключения \n" + data.message + " |error code: " + data.code;
                www.Dispose();
                yield break;
            }

            // отправляем запрос с данными на сервер и ждем подтверждения
            mainController.SetNewResponse(data);
            yield return new WaitForFixedUpdate();

            b_LogIn.interactable = true;
            PlayerPrefs.SetString("PlayerLogIn", Input_logIn.text);

            Input_Password.text = "";
            mainController.OpenWindow(SupportClass.windows.selectServer);

            www.Dispose();
        } 
    }

    private void GoToRegistration() {
        Input_Password.text = "";
        mainController.OpenWindow(SupportClass.windows.registration);
    }
}
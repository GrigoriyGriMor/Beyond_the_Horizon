using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class RegistrationController : MonoBehaviour
{
    [Header("Ввод данных для входа")]
    [SerializeField] private InputField Input_Email;
    [SerializeField] private InputField Input_logIn;
    [SerializeField] private InputField Input_Password;
    [SerializeField] private InputField Input_SecondPassword;

    [Header("Кнопки")]
    [SerializeField] private Button b_Back;
    [SerializeField] private Button b_Registration;

    [SerializeField] private GameObject errorPanel;
    [SerializeField] private Text errorText;
    [SerializeField]

    [HideInInspector]
    public AuthWindowsController mainController;

    private void Awake() {
        errorText.text = "";
        errorPanel.SetActive(false);
        b_Back.onClick.AddListener(() => GoToLogInWindows());
        b_Registration.onClick.AddListener(() => StartCoroutine(Registration()));
    }

    private IEnumerator Registration() {
        b_Registration.interactable = false;

        if (Input_logIn.text.Length <= 0 || Input_Password.text.Length <= 0 || Input_Email.text.Length <= 0) {
            errorPanel.SetActive(false);
            errorPanel.SetActive(true);
            errorText.text = "Какие-то поля незаполнены";
            b_Registration.interactable = true;
            yield break;
        }

        if (Input_Password.text != Input_SecondPassword.text) {
            errorPanel.SetActive(false); 
            errorPanel.SetActive(true);
            errorText.text = "Введенные пароли не совпадают";
            b_Registration.interactable = true;
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("name", Input_logIn.text);
        form.AddField("password", Input_Password.text);
        form.AddField("email", Input_Email.text);
         
        // отправляем запрос с данными на сервер и ждем подтверждения
        // если данные верны сохраняем логин в префсы

        using (UnityWebRequest www = UnityWebRequest.Post(WebData.RegisterPath, form)) {
            www.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(www.error, "RegistrationController");
                Debug.LogError("RegistrationController " + www.error);
                b_Registration.interactable = true;
                errorPanel.SetActive(false);
                errorPanel.SetActive(true);
                errorText.text = "Ошибка подключения \n" + www.error;
                www.Dispose();
                yield break;
            }

            if (www.downloadHandler.text.Length < 1) {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "RegisterController ");
                Debug.LogError("www.downloadHandler.text.Length < 1");
                b_Registration.interactable = true;
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
                b_Registration.interactable = true;
                errorText.text = "Ошибка подключения \n" + data.message + " |error code: " + data.code;
                www.Dispose();
                yield break;
            }

            WWWForm form1 = new WWWForm();
            form1.AddField("login", Input_Email.text);
            form1.AddField("password", Input_Password.text);

            using (UnityWebRequest www1 = UnityWebRequest.Post(WebData.LoginPath, form1)) {
                www1.SetRequestHeader(WebData.HeaderName, WebData.HeaderValue);
                yield return www1.SendWebRequest();

                if (www1.downloadHandler.text.Length < 1) {
                    if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("www.downloadHandler.text.Length < 1", "LogInController ");
                    Debug.LogError("www.downloadHandler.text.Length < 1");
                    errorPanel.SetActive(false);
                    errorPanel.SetActive(true);
                    errorText.text = "Ошибка подключения \n" + "www.downloadHandler.text.Length < 1";
                    www1.Dispose();
                    yield break;
                }

                data = JsonUtility.FromJson<EnterGameData>(www1.downloadHandler.text);

                if (!data.success) {
                    Debug.LogError(" SOME ERROR " + data.message + " |error code: " + data.code);
                    if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(data.message + " |error code: " + data.code, "RegisterController ");
                    errorPanel.SetActive(false);
                    errorPanel.SetActive(true);
                    errorText.text = "Ошибка подключения \n" + data.message + " |error code: " + data.code;
                    www1.Dispose();
                    yield break;
                }

                // отправляем запрос с данными на сервер и ждем подтверждения
                mainController.SetNewResponse(data);
                yield return new WaitForFixedUpdate();

                b_Registration.interactable = true;

                Input_Email.text = "";
                Input_logIn.text = "";
                Input_Password.text = "";
                Input_SecondPassword.text = "";
                mainController.OpenWindow(SupportClass.windows.selectServer);

                www1.Dispose();
            }
            www.Dispose();
        }
    }

    private void GoToLogInWindows() {
        Input_Email.text = "";
        Input_logIn.text = "";
        Input_Password.text = "";
        Input_SecondPassword.text = "";

        mainController.OpenWindow(SupportClass.windows.logIn);
    }
}
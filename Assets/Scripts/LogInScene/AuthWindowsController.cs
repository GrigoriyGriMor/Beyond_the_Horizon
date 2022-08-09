using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine.Networking;

public class AuthWindowsController : MonoBehaviour
{
    [Header("Окна меню LogIn")]
    [SerializeField] private LogInController logInWindows;
    [SerializeField] private RegistrationController registrationWindows;
    [SerializeField] private ServerSelectController serverSelectWindows;
    [SerializeField] private CharacterSelectController characterSelectWindows;
    [SerializeField] private CharacterCreateController createCharacterWindows;

    private EnterGameData ResponseServersData = new EnterGameData();
    private string errorText;

    private void Awake() {
        logInWindows.mainController = this;
        registrationWindows.mainController = this;
        serverSelectWindows.mainController = this;
        characterSelectWindows.mainController = this;
        createCharacterWindows.mainController = this;

        ResponseServersData.id = -1;

        OpenWindow(SupportClass.windows.logIn);
    }

    public void SetNewResponse(EnterGameData response) {
        ResponseServersData = response;
    }

    public EnterGameData GetResponseData() {
        return ResponseServersData;
    }

    [HideInInspector] public PlayerCharacterInServerData PlayerCharacters;
    public void SetServerCharacterInfo(PlayerCharacterInServerData playerCharacterInfo) {
        PlayerCharacters = playerCharacterInfo;      
    }

    public string GetError() {
        return errorText;
    }

    public void OpenWindow(SupportClass.windows windows) {
        switch (windows) {
            case SupportClass.windows.logIn:
                ResponseServersData = new EnterGameData();
                ResponseServersData.id = -1;

                logInWindows.gameObject.SetActive(true);
                registrationWindows.gameObject.SetActive(false);
                serverSelectWindows.gameObject.SetActive(false);
                characterSelectWindows.gameObject.SetActive(false);
                createCharacterWindows.gameObject.SetActive(false);
                break;
            case SupportClass.windows.registration:
                logInWindows.gameObject.SetActive(false);
                registrationWindows.gameObject.SetActive(true);
                serverSelectWindows.gameObject.SetActive(false);
                characterSelectWindows.gameObject.SetActive(false);
                createCharacterWindows.gameObject.SetActive(false);
                break;
            case SupportClass.windows.selectServer:
                logInWindows.gameObject.SetActive(false);
                registrationWindows.gameObject.SetActive(false);
                serverSelectWindows.gameObject.SetActive(true);
                characterSelectWindows.gameObject.SetActive(false);
                createCharacterWindows.gameObject.SetActive(false);

                serverSelectWindows.Init(ResponseServersData);
                break;
            case SupportClass.windows.selectCharacter:
                logInWindows.gameObject.SetActive(false);
                registrationWindows.gameObject.SetActive(false);
                serverSelectWindows.gameObject.SetActive(false);
                characterSelectWindows.gameObject.SetActive(true);
                createCharacterWindows.gameObject.SetActive(false);

                characterSelectWindows.Init(PlayerCharacters);
                break;
            case SupportClass.windows.createCharacter:
                logInWindows.gameObject.SetActive(false);
                registrationWindows.gameObject.SetActive(false);
                serverSelectWindows.gameObject.SetActive(false);
                characterSelectWindows.gameObject.SetActive(false);
                createCharacterWindows.gameObject.SetActive(true);
                break;
            default:
                logInWindows.gameObject.SetActive(true);
                registrationWindows.gameObject.SetActive(false);
                serverSelectWindows.gameObject.SetActive(false);
                characterSelectWindows.gameObject.SetActive(false);
                createCharacterWindows.gameObject.SetActive(false);
                break;
        }
    }
}

//данные, которые приходят о сервера в ответ на запрос входа 
[Serializable]
public class EnterGameData {
    public string[] error;

    public bool success;
    public int code;
    public string message;

    public int id; 
    public string name;
    public string session;
    public string session_expiration;
    public ServerBaseData[] servers;
}

[Serializable]
public class ServerBaseData {
    public int id; 
    public string name;
    public string address;
    public int port;
    public int status;//от 0 до 3 (где: 0 = неактивен, 1 = активен, 2 = тех. работы)
    public int characters;
}

//данные которые отправляются на сервер после выбора сервера и попытки подключения к нему
[Serializable]
public class ServerSelectData {
    public int id;
    public string session;
    public string session_expiration;
    //public int server_number;
    public string server_name;
}

//данные о персонажах на конкретном сервере, которые приходят после успешного выбора сервера
[Serializable]
public class PlayerCharacterInServerData {
    public string[] error;

    public int server_id;

    public bool success;
    public int code;
    public string message;

    public CharacterInServerInfo[] data;
}

[Serializable]
public class CharacterInServerInfo {
    public int id;
    public int user_id;
    public int body_type;
    public int head;
    public int hair;
    public int jackets;
    public int pants;
    public int gloves;
    public int boots;


    public string character_name;
    public int character_level;
    public int character_heal;
    public int character_shield;

    public int character_state; // Доступен или на удалении
}
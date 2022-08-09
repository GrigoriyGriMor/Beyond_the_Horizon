using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerPanel : MonoBehaviour
{
    [SerializeField] private Text serverName;
    [SerializeField] private Image serverStatus;
    [SerializeField] private Sprite[] statusImage = new Sprite[3];
    [SerializeField] private Text playerCharacterCoutltnt;

    private AuthWindowsController mainController;

    private string serverAdress;
    private int serverPort;
    private int player_ID;

    public void Init(ServerBaseData data, AuthWindowsController _mainController) {
        serverAdress = data.address;
        serverPort = data.port;

        serverName.text = data.name;
        playerCharacterCoutltnt.text = data.characters.ToString();
        try {
            serverStatus.sprite = statusImage[data.status];
        }
        catch (System.Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), $"{name} error");
        }
        mainController = _mainController;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Text;
using System.Threading;

public class ClientDataInput : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private ClientDataReceiving clientReceivingModule;

    [SerializeField] private GameObjectData ScrObjGameObjectData;

    [Header("Wait Time")]
    [SerializeField] private float waitSendTime = 0.01f;

    [Header("Server IP")]
    [SerializeField] private string ipAdress = "192.168.1.139";
    [SerializeField] private int serverPort = 55095;

    IPEndPoint connectIP;
    ControllersInputData newPackage;
    private byte[] data = new byte[38];
    private byte[] keepAlive = new byte[14];

    UdpClient clientSender = new UdpClient();

    private short _current_dX = 0;
    private short _current_dY = 0;

    private EnterGameData serverInfo;
    private CharacterInServerInfo playerVisualInfo;
    private int serverNumber = 0;

    public void Init(EnterGameData _serverInfo, CharacterInServerInfo _playerVisualInfo, int _serverNumber)
    {
        for (int i = 0; i < _serverInfo.servers.Length; i++)
            if (_serverInfo.servers[i].id == _serverNumber) {
              //  ipAdress = _serverInfo.servers[i].address;
                serverPort = _serverInfo.servers[i].port;
            }

        serverInfo = _serverInfo;
        playerVisualInfo = _playerVisualInfo;
        serverNumber = _serverNumber;

        SceneManager.sceneLoaded += OnSceneLoaded;

        #region set base param in newPackage
        newPackage.proto = 0xEFBE;//+
        newPackage.version = 0x0000;//+
        newPackage.token = BitConverter.ToUInt64(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0);
        newPackage.time = BitConverter.ToUInt64(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0);
        newPackage.move_dX = 0x00; //+
        newPackage.move_dY = 0x00;//+
        newPackage.mouse_dX = 0x0000; //+
        newPackage.mouse_dY = 0x0000;//+
        newPackage.keys = 0b10000000000000000000000000000000;

        data = SupportClass.StructureToByteArray(newPackage);
        #endregion
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        
        StartCoroutine(LogIn());
    }

    private IEnumerator LogIn() {
        yield return new WaitForSeconds(0.5f);

        //базовый пакет по игроку 
        PlayerConnectData connectData = new PlayerConnectData();
        connectData.proto = 0xEEBE;
        connectData.version = 0x0000;
        connectData.Id = (ushort)serverInfo.id;
        connectData.token = BitConverter.ToUInt64(Encoding.UTF8.GetBytes(serverInfo.session), 0);

        //пакет сообщающий серверу о том, что клиент еще на связи
        KeepAlive _keepAlive = new KeepAlive();
        _keepAlive.proto = 0xBEBE;
        _keepAlive.version = 0x0000;
        _keepAlive.id = (ushort)serverInfo.id;
        _keepAlive.token = BitConverter.ToUInt64(Encoding.UTF8.GetBytes(serverInfo.session), 0);
        keepAlive = SupportClass.StructureToByteArray(_keepAlive);

        char[] playerName = new char[10];
        for (int i = 0; i < playerVisualInfo.character_name.Length; i++)
            playerName[i] = playerVisualInfo.character_name[i];

        connectData.name = playerName;
        connectData.body_type = (ushort)playerVisualInfo.body_type;

        // connectIP = new IPEndPoint(Dns.GetHostEntry(ipAdress).AddressList[0], serverPort);
           connectIP = new IPEndPoint(IPAddress.Parse(ipAdress), serverPort);
           clientSender.Connect(connectIP);

        player = Instantiate(ScrObjGameObjectData.playerObjects[playerVisualInfo.body_type].playerClientPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        yield return new WaitForFixedUpdate();

        player.InitPlayer(playerVisualInfo.character_name, connectData.Id, serverInfo.session);
        newPackage.id = connectData.Id;

        player.GetComponent<InDamageModule>().SetHeal(playerVisualInfo.character_heal);
        player.GetComponent<InDamageModule>().SetShield(playerVisualInfo.character_shield);

        byte[] data_1 = SupportClass.StructureToByteArray(connectData);

        try 
        {
             clientSender.SendAsync(data_1, data_1.Length);
        }
        catch (Exception ex)
        {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Не удалось подключиться к серверу, попробуйте еще раз");
        }

        yield return new WaitForFixedUpdate();
        clientReceivingModule.InitClientReceiving(player, ipAdress, serverPort, clientSender, connectData.Id);
        yield return new WaitForFixedUpdate();
        player.StartThisGame();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (LoadScreenController.Instance) LoadScreenController.Instance.LoadScreenDeactive();

         StartCoroutine(SendData());
         StartCoroutine(SendAliveControl());
    }

    //отправка данных инпута на сервер
    private IEnumerator SendData()
    {
        yield return new WaitForSeconds(waitSendTime);

        GetMouseAxis();
        newPackage.time = (ulong)(Time.time * 1000);

        data = SupportClass.StructureToByteArray(newPackage);
        try {
            clientSender.SendAsync(data, data.Length);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Отправка данных не удалась (ClientDataInput)");
        }

        StartCoroutine(SendData());
    }

    private IEnumerator SendAliveControl() {
        yield return new WaitForSeconds(0.5f);

        try {
            clientSender.SendAsync(keepAlive, keepAlive.Length);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Отправка данных не удалась KeepAlive");
        }

        StartCoroutine(SendAliveControl());
    }

    #region SetNewParam
    public void SetNewKeysState(Keys keys)//получаем параметры клавиатуры
    {
        newPackage.keys = (uint)keys;
    }

    public void SetNewMoveAxis(Vector2 Axis)//получаем смещение axis WASD
    {
        newPackage.move_dX = Axis.x;
        newPackage.move_dY = Axis.y;
    }

    public void SetNewMousePos(Vector2 pos)//получаем позицию мыши на экране
    {
        _current_dX += (short)(pos.x * 100);
        _current_dY += (short)(pos.y * 100);
    }

    public void GetMouseAxis() {
        newPackage.mouse_dX = _current_dX;
        newPackage.mouse_dY = _current_dY;

        _current_dX = 0;
        _current_dY = 0;
    }
    #endregion

    private void OnApplicationQuit() {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Player was disconnect");

        PlayerDisconnectData disconectData = new PlayerDisconnectData();

        disconectData.proto = 0xECBE;
        disconectData.version = 0x0000;

        disconectData.id = player.playerID;
        disconectData.token = newPackage.token;

        byte[] data = SupportClass.StructureToByteArray(disconectData);
        try {
            clientSender.SendAsync(data, data.Length);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Отправка данных не удалась (ClientDataInput)");
        }

        Close();
    }

    private void Close() {
        if (clientSender != null) {
            clientSender.Close();
            clientSender.Dispose();
        }

        Debug.LogError("Disconnect");
    }
}
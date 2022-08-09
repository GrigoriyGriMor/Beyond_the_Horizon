using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPMainServer : MonoBehaviour
{
    [SerializeField] private ServerPlayerConnect SPC;

    [Header("base port")]
    //[SerializeField] private const string Adress = "cloudsgoods.com";
    [SerializeField] private const string Adress = "192.168.1.139";
    [SerializeField] private const int Port = 55095;

    [SerializeField] private const int myPort = 49402;

    UdpClient clientUDPRead;
    UdpClient clientUDPSender = new UdpClient();

    IPEndPoint localIP;
    IPEndPoint serverIP;

    Thread connectThread;

    public void Start() 
    {
        if (RPCController.Instance) { 
            RPCController.Instance.SetServerController(this);
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("RPC Controller Connect"); }
        else
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("RPCController not found ", "UDP SERVER");

        StartCoroutine(InitServerListner());
    }

    private IEnumerator InitServerListner() {
        yield return new WaitForSeconds(0.5f);
        localIP = new IPEndPoint(IPAddress.Any, myPort);
        clientUDPRead = new UdpClient(localIP);//дл€ чтени€

        // serverIP = new IPEndPoint(Dns.GetHostEntry(Adress).AddressList[0], Port);
        serverIP = new IPEndPoint(IPAddress.Parse(Adress), Port);//дл€ отправки
        clientUDPSender.Connect(serverIP);//дл€ отправки

        connectThread = new Thread(Receive);
        connectThread.Start();
    }

    Coroutine coroutine;
    public void Receive()
    {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" Ќачали слушать ");

        while (true)
        {
            try {
                byte[] _buffer_recv = clientUDPRead.Receive(ref serverIP);

#region CONNECT New player (BEEE)
                if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0xBE && _buffer_recv[1] == 0xEE)) {
                    if (coroutine == null) coroutine = StartCoroutine(ConnectPlayer(_buffer_recv));
                }
#endregion

#region disconnect (BEEC)
                //DISSCONNECT
                if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0xEC && _buffer_recv[1] == 0xBE)) {

                    PlayerDisconnectData newData = SupportClass.StructureStopPlayerSessionReturn(_buffer_recv);
                    if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(newData.id + " player disconect package come ");

                    if (RPCController.Instance) RPCController.Instance.DisconnectPlayer(newData);
                }
                #endregion

                #region INPUT DATA CONTROL (BEEF)
                if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0xBE && _buffer_recv[1] == 0xEF)) {
                    InputClientData newData = new InputClientData();
                    newData = SupportClass.StructureAllTimeDataReturn(_buffer_recv);

                    //важно! пока-что пакеты инпута приход€т по одному от каждого клиента в один момент времени, это может быть узким местом сетевого кода
                    ThreadPool.QueueUserWorkItem((q) => RPCController.Instance.InputPlayerDataIn(newData));
                }
                #endregion

                #region SERVER REQUEST CONTROL  огда сервер запрашивает состо€ние игры (BE00)
                if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0x00 && _buffer_recv[1] == 0xBE)) {
                    if (_buffer_recv.Length >= 4 && (_buffer_recv[2] == 0x01 && _buffer_recv[3] == 0x00))
                        ThreadPool.QueueUserWorkItem((q) => RPCController.Instance.C_ServerStateMainRequest());
                    else
                        ThreadPool.QueueUserWorkItem((q) => RPCController.Instance.ServerPackegeInit(_buffer_recv));
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "UDPServer Reveice error: ");
            }
        }
        
    }

    private void OnApplicationQuit() {
        Close(); 
    }

    public void SendData(WorldData str) {
        if (!clientUDPSender.Client.Connected)
            return;

        byte[] data = SupportClass.StructureToByteArray(str);
        try {
            clientUDPSender.Send(data, data.Length);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Send(WorldData data): ");
        }
    }

    public void SendData(RPC_Object_Register_Data str) {
        if (!clientUDPSender.Client.Connected) 
            return;

        byte[] data = SupportClass.StructureToByteArray(str);
        try {
            clientUDPSender.Send(data, data.Length);
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Send(RPC_Object_Register_Data data): ");
        }
    }

    private void Close()
    {
        if (connectThread != null)
        {
            connectThread.Interrupt();
            connectThread.Abort();
        }

        if (clientUDPRead != null)
        {
            clientUDPRead.Close();
            clientUDPRead.Dispose();
        }

        if (clientUDPSender != null) {
            clientUDPSender.Close();
            clientUDPSender.Dispose();
        }

        Debug.LogError("Disconnect");
    }

    //игроки единственный объект по которому сервер получает информацию извне, все остальные объекты контролируютс€ только на сервере
    public IEnumerator ConnectPlayer(byte[] buffer_recv) {
        yield return new WaitForFixedUpdate();

        PlayersOnServerData newPlayer = new PlayersOnServerData();//создаем блок
        PlayerServerConnectData newData = SupportClass.StructureStartToServerPlayerSessionReturn(buffer_recv);//разбираем структуру

        try {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("UDPServer: Create player " + newData.Id);

            newPlayer.playerInputManager = SPC.ConnectNewClient(newData.name, newData.body_type);
            newPlayer.playerInputManager.player.playerID = newData.Id;
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("UDPServer: " + ex.ToString());
        }

        yield return new WaitForFixedUpdate();
        coroutine = null;
    }
}
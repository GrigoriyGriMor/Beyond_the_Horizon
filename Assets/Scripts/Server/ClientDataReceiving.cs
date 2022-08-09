using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

public class ClientDataReceiving : MonoBehaviour
{
    [SerializeField] private PlayerController playerBaseController;
    [SerializeField] private List<PlayersOnClientData> playerControllerManager = new List<PlayersOnClientData>();
    [SerializeField] private List<EnemyBase> warriorControllerManager = new List<EnemyBase>();
    [SerializeField] private ServerPlayerConnect SPC;

    [Header("Server IP")]
    private string ipAdress = "192.168.1.139";
    private int serverPort = 55095;
    IPEndPoint serverIP;
    UdpClient clientReciever;

    [SerializeField] private const int myPort = 49402;

    Thread connectThread;

   // private byte[] _buffer_recv;

    public void InitClientReceiving(PlayerController _player, string serverAdress, int _serverPort, UdpClient socket, ushort playerId) {
        playerBaseController = _player;
        ipAdress = serverAdress;
        serverPort = _serverPort;
        clientReciever = socket;

        PlayersOnClientData basePlayer = new PlayersOnClientData();
        basePlayer.player_Id = playerId;
        basePlayer.playerController = playerBaseController;
        playerControllerManager.Add(basePlayer);

        try {
            //serverIP = new IPEndPoint(Dns.GetHostEntry(ipAdress).AddressList[0], serverPort);
            serverIP = new IPEndPoint(IPAddress.Parse(ipAdress), serverPort);

            connectThread = new Thread(Recieve);
            connectThread.Start();
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), name);
        }
    }

    public void Recieve() {
            while (true) {
                try {
                byte[] _buffer_recv = clientReciever.Receive(ref serverIP);

                    //new worldData receiving
                    if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0xBE && _buffer_recv[1] == 0x00)) {
                        WorldData data_1 = SupportClass.StructureRequestWorldDataReturn(_buffer_recv);
                        ThreadPool.QueueUserWorkItem((q) => SetNewParam(data_1));
                    }

                    if (_buffer_recv.Length > 2 && (_buffer_recv[0] == 0x10 && _buffer_recv[1] == 0xBE)) {
                        RPC_Object_Register_Data data_1 = SupportClass.StructureRequestRPCRegisterDataReturn(_buffer_recv);
                        RPC_Object_Data data_2 = new RPC_Object_Data();
                        data_2.playerID = data_1.playerID;
                        data_2.data = data_1.data;

                    if (data_1.object_type == 1)
                        if (data_1.object_state == 1 && data_1.playerID != 0)
                            if (data_1.playerID != playerBaseController.playerID) {
                                for (int i = 0; i < playerControllerManager.Count; i++)
                                    if (playerControllerManager[i].player_Id == data_1.playerID)
                                        return;

                                ThreadPool.QueueUserWorkItem((q) => StartCoroutine(NewPlayerCloneConnect(data_2)));
                            }

                    if (data_1.object_type == 2)
                        if (data_1.object_state == 1 && data_1.playerID != 0) {
                            for (int i = 0; i < warriorControllerManager.Count; i++)
                                if (warriorControllerManager[i].GetWarriorID() == data_1.playerID)
                                    return;

                            ThreadPool.QueueUserWorkItem((q) => StartCoroutine(NewWarriorCloneConnect(data_2)));
                        }
                    }
                }
                catch (Exception ex) {
                    if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Recieve error: ");
                }
            }
    }

    private void SetNewParam(WorldData data_1) {
        try {
            //player parce
            //если это игрок
            if (data_1.rpc_data.object_type == 1) {
                //если состояние объекта активное (то есть следим за ним)
                if (data_1.rpc_data.object_state == 2)
                    for (int j = 0; j < playerControllerManager.Count; j++) {
                        if (playerControllerManager[j] != null) {
                            if (data_1.rpc_data.playerID == playerControllerManager[j].player_Id) 
                            { //проверяем на наличие игрока в списке
                                
                                PlayerCoreData pData = SupportClass.StructurePCDDataReturn(data_1.rpc_data.data);
                                if (data_1.rpc_data.playerID == playerBaseController.playerID) {//если игрок есть в списке проверяем игрок ли это или нет
                                    ThreadPool.QueueUserWorkItem((q) => SetPlayerSincParam(pData, playerControllerManager[j].playerController));
                                }
                                else {
                                    ThreadPool.QueueUserWorkItem((q) => SetPlayerSincParam(pData, playerControllerManager[j].playerController));
                                    SetSincInputParam(pData, playerControllerManager[j].playerController);
                                }
                                return;
                            }
                        }
                    }

                if (data_1.rpc_data.object_state == 1 && data_1.rpc_data.playerID != 0) {
                    if (data_1.rpc_data.playerID != playerBaseController.playerID) {
                        for (int i = 0; i < playerControllerManager.Count; i++)
                            if (playerControllerManager[i].player_Id == data_1.rpc_data.playerID)
                                return;

                        ThreadPool.QueueUserWorkItem((q) => StartCoroutine(NewPlayerCloneConnect(data_1.rpc_data)));
                        return;
                    }
                }

                if (data_1.rpc_data.object_state == 3 && data_1.rpc_data.playerID != 0) {
                    if (data_1.rpc_data.playerID != playerBaseController.playerID) {
                        ThreadPool.QueueUserWorkItem((q) => RemovePlayer(data_1.rpc_data.playerID));
                        return;
                    }
                }
            }
            //player

            //warrior parce
            //если это враг
            if (data_1.rpc_data.object_type == 2) {

                if (data_1.rpc_data.object_state == 2)//если это активный враг (то есть за которым следим)
                    for (int j = 0; j < warriorControllerManager.Count; j++) {
                        if (warriorControllerManager[j] != null) {
                            if (data_1.rpc_data.playerID == warriorControllerManager[j].GetWarriorID()) { //проверяем на наличие warrior в списке
                                WarriorData wData = SupportClass.StructureWCDDataReturn(data_1.rpc_data.data);
                                ThreadPool.QueueUserWorkItem((q) => SetWarriorSincParam(wData, warriorControllerManager[j]));
                                return;
                            }
                        }
                    }

                if (data_1.rpc_data.object_state == 1 && data_1.rpc_data.playerID != 0) {
                    for (int i = 0; i < warriorControllerManager.Count; i++)
                        if (warriorControllerManager[i].GetWarriorID() == data_1.rpc_data.playerID)
                            return;

                    ThreadPool.QueueUserWorkItem((q) => StartCoroutine(NewWarriorCloneConnect(data_1.rpc_data)));
                    return;
                }

                if (data_1.rpc_data.object_state == 3 && data_1.rpc_data.playerID != 0) {
                    ThreadPool.QueueUserWorkItem((q) => RemoveWarrior(data_1.rpc_data.playerID));
                    return;
                }
            }
            //warrior
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Some error: " + ex.ToString());
        }
    }

    #region New Object Connect
    public IEnumerator NewPlayerCloneConnect(RPC_Object_Data data_1) {
        yield return new WaitForFixedUpdate();
        
        PlayerServerConnectData connectData = SupportClass.StructureStartToServerPlayerSessionReturn(data_1.data);

        PlayersOnClientData newP = new PlayersOnClientData();
        newP.player_Id = data_1.playerID;
        newP.playerController = SPC.ConnectNewClone(connectData.name, connectData.body_type);

        if (newP != null) playerControllerManager.Add(newP);

        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Новый игрок создан: " + connectData.name + " | " + newP.player_Id);
    }

    public IEnumerator NewWarriorCloneConnect(RPC_Object_Data data_1) {
        yield return new WaitForFixedUpdate();

        PlayerServerConnectData connectData = SupportClass.StructureStartToServerPlayerSessionReturn(data_1.data);

        EnemyBase newW = SPC.ConnectNewWarriorClone(connectData.body_type);

        if (newW != null) {
            newW.SetID(data_1.playerID);
            warriorControllerManager.Add(newW);
        }

        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Новый warrior создан: " + data_1.playerID);
    }
    #endregion

    #region Set param
    void SetPlayerSincParam(PlayerCoreData dataPlayer, PlayerController CurrentPlayer) {
        try {
            Vector3 playerPos = new Vector3(dataPlayer.playerPosX, dataPlayer.playerPosY, dataPlayer.playerPosZ);
            Vector3 playerRotation = new Vector3(dataPlayer.playerRotateX, dataPlayer.playerRotateY, dataPlayer.playerRotateZ);
            Vector3 playerVisualRotate = new Vector3(dataPlayer.playerVisualRotateX, dataPlayer.playerVisualRotateY, dataPlayer.playerVisualRotateZ);
            Vector3 cameraRotate = new Vector3(dataPlayer.cameraRotateX, dataPlayer.cameraRotateY, dataPlayer.cameraRotateZ);
            Vector3 aimRotate = new Vector3(dataPlayer.aimRotateX, dataPlayer.aimRotateY, dataPlayer.aimRotateZ);
            Vector3 velocityP = new Vector3(dataPlayer.velocityX, dataPlayer.velocityY, dataPlayer.velocityZ);

            int gunN;
            switch (dataPlayer.usedGunN) {
                case 2:
                    gunN = -1;
                    break;
                case 0:
                    gunN = 0;
                    break;
                case 1:
                    gunN = 1;
                    break;
                default:
                    gunN = -1;
                    break;
            }

            ThreadPool.QueueUserWorkItem((q) => CurrentPlayer.SetServerParamSinc(playerPos, playerRotation, playerVisualRotate, cameraRotate, aimRotate, velocityP, 
                dataPlayer.healPoint, dataPlayer.shieldPoint, gunN, dataPlayer.playerState));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("player sinc param error: " + ex.ToString());
        }
    }

    void SetWarriorSincParam(WarriorData dataPlayer, EnemyBase CurrentWarrior) {
        try {
            Vector3 w_Pos = new Vector3(dataPlayer.w_pos_X, dataPlayer.w_pos_Y, dataPlayer.w_pos_Z);
            Vector3 w_Rotation = new Vector3(dataPlayer.w_rotate_X, dataPlayer.w_rotate_Y, dataPlayer.w_rotate_Z);

            Vector3 w_target_Pox = new Vector3(dataPlayer.w_target_posX, dataPlayer.w_target_posY, dataPlayer.w_target_posZ);
            ushort w_attack = dataPlayer.distance_w_attack;

            ThreadPool.QueueUserWorkItem((q) => CurrentWarrior.SetServerParamSinc(w_Pos, w_Rotation, dataPlayer.w_heal_count, dataPlayer.w_shield_count,
                w_target_Pox, w_attack, dataPlayer.animLayerWeight, dataPlayer.animParam));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("warrior sinc param error: " + ex.ToString());
        }
    }
    #endregion

    #region Remove object
    private void RemovePlayer(uint ID) {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" try delete player " + ID);

        try {
            for (int i = 0; i < playerControllerManager.Count; i++) {
                if (playerControllerManager[i] != null && ID == playerControllerManager[i].player_Id) {
                    Destroy(playerControllerManager[i].playerController.gameObject);
                    playerControllerManager.RemoveAt(i);
                }
            }
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("RemovePlayer ", ex.ToString());
        }
    }

    private void RemoveWarrior(uint ID) {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" try delete warrior " + ID);

        try {
            for (int i = 0; i < warriorControllerManager.Count; i++) {
                if (warriorControllerManager[i] != null && ID == warriorControllerManager[i].GetWarriorID()) {
                    Destroy(warriorControllerManager[i].gameObject);
                    warriorControllerManager.RemoveAt(i);
                }
            }
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("RemoveWarrior ", ex.ToString());
        }
    }
    #endregion

    void SetSincInputParam(PlayerCoreData dataPlayer, PlayerController CurrentPlayer) {
        ThreadPool.QueueUserWorkItem((q) => CurrentPlayer.inputModule.NewInputIteration(dataPlayer.inputData));
    }

    private void OnApplicationQuit() { 
        Close();
    }

    private void Close() {
        if (connectThread != null) {
            connectThread.Interrupt();
            connectThread.Abort();
        }

        Debug.LogError("Disconnect Thread");
    }
}

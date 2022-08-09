using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class RPCController : MonoBehaviour
{
    private static RPCController instance;
    public static RPCController Instance => instance;

    private UDPMainServer serverController;

    #region Players
        [HideInInspector] public List<InputPlayerManager> activePlayerAtScene = new List<InputPlayerManager>();
    #endregion

    #region Warriors
        [HideInInspector] public List<EnemyBase> activeWarriorAtScene = new List<EnemyBase>();
    #endregion

    private List<uint> id_Pool = new List<uint>();
    private uint GetID() {//get new id for item
        uint newID = (id_Pool.Count > 0 ? (id_Pool[id_Pool.Count - 1] + 1) : 1);

        for (int i = 0; i < activePlayerAtScene.Count; i++)
            if (activePlayerAtScene[i].player.playerID == newID) {
                newID += 1;
                i = 0;
            }

        id_Pool.Add(newID);

        return newID;
    }

    private void Awake() {
        instance = this;
    }

    public void SetServerController(UDPMainServer s) {
        serverController = s;
    }

    int lastCount = 0;
    public void Update() {
        if (lastCount != activePlayerAtScene.Count) {
            ConsoleScript.Instance.AddConsoleText("PLayer Count: " + activePlayerAtScene.Count);
            lastCount = activePlayerAtScene.Count;
        }
    }

    #region Build Server Package
    public void ServerPackegeInit(byte[] _buffer_recv) {
        //отправка данных по активным игрокам

        try {
            for (int itemIndex = 0; itemIndex < activePlayerAtScene.Count; itemIndex++) {
                WorldData newData = new WorldData();

                //определение порта дл€ обратной передачи
                RequestGameState port = SupportClass.StructureRequestDataReturn(_buffer_recv);
                newData.proto = 0x00BE;
                newData.version = port.version;

                RPC_Object_Data players = new RPC_Object_Data();

                players.object_type = 1;
                players.object_state = 2;
                players.playerID = activePlayerAtScene[itemIndex].player.playerID;
                players.data = SupportClass.StructureToByteArray(activePlayerAtScene[itemIndex].player.GetPlayerData());

                newData.rpc_data = players;

                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
            }
        }
        catch (System.Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("1_" + ex.ToString());
            return;
        }

        try {
            //warrior package
            for (int _itemIndex = 0; _itemIndex < activeWarriorAtScene.Count; _itemIndex++) {
                WorldData newData = new WorldData();

                //определение порта дл€ обратной передачи
                RequestGameState port = SupportClass.StructureRequestDataReturn(_buffer_recv);
                newData.proto = 0x00BE;
                newData.version = port.version;

                RPC_Object_Data warriors = new RPC_Object_Data();

                warriors.object_type = 2;
                warriors.object_state = 2;
                warriors.playerID = activeWarriorAtScene[_itemIndex].GetWarriorID();
                warriors.data = SupportClass.StructureToByteArray(activeWarriorAtScene[_itemIndex].GetWarriorData());

                newData.rpc_data = warriors;

                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
            }
        }
        catch (System.Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("2_" + ex.ToString());
            return;
        }
    }
    #endregion

    public void C_ServerStateMainRequest() {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("«апрос всех состо€ний");

        try {
            for (int i = 0; i < activePlayerAtScene.Count; i++) {
                WorldData newData = new WorldData();
                newData.proto = 0x00BE;

                RPC_Object_Data players = new RPC_Object_Data();
                RPC_Object_Register_Data register_obj = new RPC_Object_Register_Data();
                register_obj.proto = 0xBE10;

                players.object_type = 1;
                register_obj.object_type = 1;

                players.object_state = 1;
                register_obj.object_state = 1;

                players.playerID = activePlayerAtScene[i].player.playerID;
                register_obj.playerID = activePlayerAtScene[i].player.playerID;

                PlayerServerConnectData newPack = new PlayerServerConnectData();
                newPack.name = activePlayerAtScene[i].player.playerName.text.ToCharArray();
                newPack.body_type = activePlayerAtScene[i].player.body_type;

                players.data = SupportClass.StructureToByteArray(newPack);
                register_obj.data = SupportClass.StructureToByteArray(newPack);

                newData.rpc_data = players;

                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(register_obj));
            }

            for (int i = 0; i < activeWarriorAtScene.Count; i++) {
                WorldData w_newData = new WorldData();
                w_newData.proto = 0x00BE;

                RPC_Object_Data warriors = new RPC_Object_Data();
                RPC_Object_Register_Data w_register_obj = new RPC_Object_Register_Data();
                w_register_obj.proto = 0xBE10;

                warriors.object_type = 2;
                w_register_obj.object_type = 2;

                warriors.object_state = 1;
                w_register_obj.object_state = 1;

                warriors.playerID = activeWarriorAtScene[i].GetWarriorID();
                w_register_obj.playerID = activeWarriorAtScene[i].GetWarriorID();

                PlayerServerConnectData w_newPack = new PlayerServerConnectData();
                w_newPack.body_type = (ushort)activeWarriorAtScene[i].body_type;

                warriors.data = SupportClass.StructureToByteArray(w_newPack);
                w_register_obj.data = SupportClass.StructureToByteArray(w_newPack);

                w_newData.rpc_data = warriors;

                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(w_newData));
                ThreadPool.QueueUserWorkItem((q) => serverController.SendData(w_register_obj));
            }
        }
        catch (System.Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "RPCContoller");
            return;
        }
    }

    #region main register PRC functions
    public void RegisterNewObj(RPCObject obj) {
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText("Ќовый RPC зарегистрирован");

        switch (obj.object_type) {
            case SupportClass.RPC_Type.player:
                ThreadPool.QueueUserWorkItem((q) => StartCoroutine(RegisterNewPlayer(obj)));
                break;
            case SupportClass.RPC_Type.warrior:
                obj.warrior.SetID(GetID());
                ThreadPool.QueueUserWorkItem((q) => StartCoroutine(RegisterNewWarrior(obj)));
                break;
            case SupportClass.RPC_Type.NPC:

                break;
            case SupportClass.RPC_Type.otherObject:

                break;
        }
    }

    private IEnumerator RegisterNewPlayer(RPCObject obj) {
        yield return new WaitForFixedUpdate();
        
        WorldData newData = new WorldData();
        newData.proto = 0x00BE;

        RPC_Object_Data players = new RPC_Object_Data();
        RPC_Object_Register_Data register_obj = new RPC_Object_Register_Data();
        register_obj.proto = 0xBE10;

        players.object_type = 1;
        register_obj.object_type = 1;

        players.object_state = 1;
        register_obj.object_state = 1;

        

        players.playerID = obj.player.player.playerID;
        
        register_obj.playerID = obj.player.player.playerID;
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" ID new player " + obj.player.player.playerID);

        PlayerServerConnectData newPack = new PlayerServerConnectData(); 
        newPack.name = obj.player.player.playerName.text.ToCharArray(); //если на серверном персонаже нет ссылки на TextName Text, то будет выдавать тут ошибку: NullReference RegisterNewPlayer
        newPack.body_type = obj.player.player.body_type;
        players.data = SupportClass.StructureToByteArray(newPack);
        register_obj.data = SupportClass.StructureToByteArray(newPack);

        activePlayerAtScene.Add(obj.player);

        newData.rpc_data = players;



        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(register_obj));

    }

    private IEnumerator RegisterNewWarrior(RPCObject obj) {
        yield return new WaitForFixedUpdate();

        WorldData w_newData = new WorldData();
        w_newData.proto = 0x00BE;

        RPC_Object_Data warriors = new RPC_Object_Data();
        RPC_Object_Register_Data w_register_obj = new RPC_Object_Register_Data();
        w_register_obj.proto = 0xBE10;

        warriors.object_type = 2;
        w_register_obj.object_type = 2;

        warriors.object_state = 1;
        w_register_obj.object_state = 1;

        warriors.playerID = obj.warrior.GetWarriorID();
        w_register_obj.playerID = obj.warrior.GetWarriorID();
        if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" ID new warrior " + obj.warrior.GetWarriorID());

        PlayerServerConnectData w_newPack = new PlayerServerConnectData();
        w_newPack.body_type = (ushort)obj.warrior.body_type;

        warriors.data = SupportClass.StructureToByteArray(w_newPack);
        w_register_obj.data = SupportClass.StructureToByteArray(w_newPack);

        activeWarriorAtScene.Add(obj.warrior);

        w_newData.rpc_data = warriors;

        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(w_newData));
        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(w_register_obj));
    }


    public void RemoveSceneObj(RPCObject obj) {
        switch (obj.object_type) {
            case SupportClass.RPC_Type.player:
                if (activePlayerAtScene.Contains(obj.player)) {
                    if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(" player was destroy in array");

                    ThreadPool.QueueUserWorkItem((q) => {
                        WorldData newData = new WorldData();
                        newData.proto = 0x00BE;

                        RPC_Object_Data players = new RPC_Object_Data();
                        RPC_Object_Register_Data remove_obj = new RPC_Object_Register_Data();
                        remove_obj.proto = 0xBE10;

                        players.object_type = 1;
                        remove_obj.object_type = 1;

                        players.object_state = 3;
                        remove_obj.object_state = 3;

                        players.playerID = obj.player.player.playerID;
                        remove_obj.playerID = obj.player.player.playerID;

                        newData.rpc_data = players;

                        activePlayerAtScene.Remove(obj.player);
                        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
                        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(remove_obj));
                    });
                }
                break;
            case SupportClass.RPC_Type.warrior:
                if (activeWarriorAtScene.Contains(obj.warrior)) {
                    ThreadPool.QueueUserWorkItem((q) => {
                        WorldData newData = new WorldData();
                        newData.proto = 0x00BE;

                        RPC_Object_Data warriors = new RPC_Object_Data();
                        RPC_Object_Register_Data remove_obj = new RPC_Object_Register_Data();
                        remove_obj.proto = 0xBE10;

                        warriors.object_type = 2;
                        remove_obj.object_type = 2;

                        warriors.object_state = 3;
                        remove_obj.object_state = 3;

                        warriors.playerID = obj.warrior.GetWarriorID();
                        remove_obj.playerID = obj.warrior.GetWarriorID();

                        newData.rpc_data = warriors;

                        activeWarriorAtScene.Remove(obj.warrior);
                        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(newData));
                        ThreadPool.QueueUserWorkItem((q) => serverController.SendData(remove_obj));
                    });
                }
                break;
            case SupportClass.RPC_Type.NPC:

                break;
            case SupportClass.RPC_Type.otherObject:

                break;
        }
    }
    #endregion

    public void DisconnectPlayer(PlayerDisconnectData newData) {
        try {
            for (int i = 0; i < activePlayerAtScene.Count; i++) {
                if (newData.id == activePlayerAtScene[i].player.playerID) {
                    Destroy(activePlayerAtScene[i].player.gameObject);
                }
            }

            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(newData.id + " player was disconected");
        }
        catch (System.Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "DisconnectPlayer");
        }
    }

    #region input data to player init
    public void InputPlayerDataIn(InputClientData newData) {
        if (activePlayerAtScene.Count != 0)
            for (int i = 0; i < activePlayerAtScene.Count; i++) {
                if (newData.playerID == activePlayerAtScene[i].player.playerID)
                    activePlayerAtScene[i].NewInputIteration(newData);
            }
    }
#endregion 
}

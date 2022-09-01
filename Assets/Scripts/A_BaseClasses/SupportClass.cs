using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;

[Serializable]
public class SupportClass 
{
    public enum PlayerStateMode { Idle, Combat, Sprint }
    public enum ItemType { weapon, skin, coins, otherItem };

    public enum windows { logIn, registration, selectServer, selectCharacter, createCharacter }

    public enum itemEvents { Talk, PickUp, Open, Use }
    public enum interactiveItemType { NPC, BulletBox, Billboard, Door, Other, Car }

    public enum RPC_Type { player, warrior, NPC, car, otherObject }

    public enum SendType { mainSend, groupSend, privateSend }

    public enum InventorySlotType { mainCell, weaponCell, skinCell, moduleCell }

    public static byte[] StructureToByteArray(object structure) {
        int lenght = Marshal.SizeOf(structure);
        byte[] _array = new byte[lenght];

        IntPtr ptr = Marshal.AllocHGlobal(lenght);
        Marshal.StructureToPtr(structure, ptr, true);
        Marshal.Copy(ptr, _array, 0, lenght);
        Marshal.FreeHGlobal(ptr);

        return _array;
    }

    public static InputClientData StructureAllTimeDataReturn(byte[] arr) {
        InputClientData packet = new InputClientData();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (InputClientData)Marshal.PtrToStructure(prt, typeof(InputClientData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static PlayerConnectData StructureStartPlayerSessionReturn(byte[] arr) {
        PlayerConnectData packet = new PlayerConnectData();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (PlayerConnectData)Marshal.PtrToStructure(prt, typeof(PlayerConnectData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }
        
        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static PlayerServerConnectData StructureStartToServerPlayerSessionReturn(byte[] arr) {
        PlayerServerConnectData packet = new PlayerServerConnectData();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (PlayerServerConnectData)Marshal.PtrToStructure(prt, typeof(PlayerServerConnectData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }
        
        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static PlayerDisconnectData StructureStopPlayerSessionReturn(byte[] arr) {
        PlayerDisconnectData packet = new PlayerDisconnectData();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (PlayerDisconnectData)Marshal.PtrToStructure(prt, typeof(PlayerDisconnectData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);
        
        return packet;
    }

    public static RequestGameState StructureRequestDataReturn(byte[] arr) {
        RequestGameState packet = new RequestGameState();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (RequestGameState)Marshal.PtrToStructure(prt, typeof(RequestGameState));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static WorldData StructureRequestWorldDataReturn(byte[] arr) {
        WorldData packet = new WorldData();
        
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (WorldData)Marshal.PtrToStructure(prt, typeof(WorldData)); 
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static PlayerCoreData StructurePCDDataReturn(byte[] arr) {
        PlayerCoreData packet = new PlayerCoreData();

        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (PlayerCoreData)Marshal.PtrToStructure(prt, typeof(PlayerCoreData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static WarriorData StructureWCDDataReturn(byte[] arr) {
        WarriorData packet = new WarriorData();

        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (WarriorData)Marshal.PtrToStructure(prt, typeof(WarriorData));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str warriorData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static RPC_Object_Data StructurePDDataReturn(byte[] arr) {
        RPC_Object_Data packet = new RPC_Object_Data();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);

        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (RPC_Object_Data)Marshal.PtrToStructure(prt, typeof(RPC_Object_Data));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }

    public static RPC_Object_Register_Data StructureRequestRPCRegisterDataReturn(byte[] arr) {
        RPC_Object_Register_Data packet = new RPC_Object_Register_Data();

        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, prt, size);

        try {
            packet = (RPC_Object_Register_Data)Marshal.PtrToStructure(prt, typeof(RPC_Object_Register_Data));
        }
        catch (Exception ex) {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }

        Marshal.FreeHGlobal(prt);

        return packet;
    }
}

//—труктура отправки данных от клиента по состо€ни€м нажатых клавиш
#region Controllers Input Data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct ControllersInputData {
    public ushort proto;//2
    public ushort version;//2   // верси€ протокола 
    public ushort id;//2
    public UInt64 token;//8  // сессионный ключ игрока
    public UInt64 time;//8   // количество миллисекунд с начала сессии

    public float move_dX; //4      //дельта смещени€ по горизонтали (переводитс€ Unity3d из данных о нажатии клавиш WASD или джостика геймпада)
    public float move_dY;//4
    public short mouse_dX; //2       // дельта смещени€ мыши по X
    public short mouse_dY; //2       // дельта смещени€ мыши по Y 
    public uint keys;  //4    // состо€ние клавиатуры (клавиш управлени€ игры)  

    public ControllersInputData(ushort _proto, ushort _version, ushort _id, UInt64 _token, UInt64 _time, float m_dX, float m_dY, short _dX, short _dY, uint _keys) {
        proto = _proto;
        version = _version;
        id = _id;
        token = _token;
        time = _time;
        move_dX = m_dX;
        move_dY = m_dY;
        mouse_dX = _dX;
        mouse_dY = _dY;
        keys = _keys;
    }
}
#endregion

//ƒанные которые получает серверный клиент после того, как сервер обработает данные о нажатых клавишах клиентом
#region Input Client Data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct InputClientData {
    public ushort proto;
    public ushort version;
    public ushort playerID;

    public byte btnlookCameraR;
    public byte btnSwapFightMode;
    public byte btnJump;    // состо€ние кнопок: KEYSTATE_OFF=0; KEYSTATE_PRESS=1; KEYSTATE_DRAG=2; KEYSTATE_RELEASE=3
    public byte btnCrouch;
    public byte btnSprint;
    public byte btnWeapon1;
    public byte btnWeapon2;
    public byte btnFire;
    public byte btnReload;
    public byte btnAim;

    public int mouseDX;   // дельта смещени€ мыши по X
    public int mouseDY;   // дельта смещени€ мыши по Y

    public float AxisX;     // дельта смещени€ по горизонтали (переводитс€ Unity3d из данных о нажатии клавиш WASD или джостика геймпада)
    public float AxisY;

    public InputClientData(ushort _proto, ushort _version, ushort _playerID, byte _btnlookCameraR, byte _btnSwapFightMode, byte _btnJump, byte _btnCrouch, byte _btnSprint,  
         byte _btnWeapon1, byte _btnWeapon2, byte _btnFire, byte _btnReload, byte _btnAim, int _mouseDX, int _mouseDY, float _AxisX, float _AxisY) {
        proto = _proto;
        version = _version;
        playerID = _playerID;

        btnlookCameraR = _btnlookCameraR;
        btnSwapFightMode = _btnSwapFightMode;
        btnJump = _btnJump;    // состо€ние кнопок: KEYSTATE_OFF=0; KEYSTATE_PRESS=1; KEYSTATE_DRAG=2; KEYSTATE_RELEASE=3
        btnCrouch = _btnCrouch; 
        btnSprint = _btnSprint;
        btnWeapon1 = _btnWeapon1;
        btnWeapon2 = _btnWeapon2;
        btnFire = _btnFire;
        btnReload = _btnReload;
        btnAim = _btnAim;

        mouseDX = _mouseDX;
        mouseDY = _mouseDY;

        AxisX = _AxisX;
        AxisY = _AxisY;
    }
}
#endregion

#region Connect|Disconect Client
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct PlayerConnectData {//BEEE 26

    public ushort proto;//2
    public ushort version;//2
    public ushort Id;//2
    public UInt64 token;//8
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public char[] name;//10
    public ushort body_type;//2

    public PlayerConnectData(ushort _proto, ushort _version, ushort _id, UInt64 _token, char[] _name, ushort _body_type) {
        proto = _proto;
        version = _version;   
        Id = _id;
        token = _token;
        name = _name;
        body_type = _body_type;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct PlayerServerConnectData {//BEEE 18

    public ushort proto;//2
    public ushort version;//2
    public ushort Id;//2
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public char[] name;//10
    public ushort body_type;//2

    public PlayerServerConnectData(ushort _proto, ushort _version, ushort _id, char[] _name, ushort _body_type) {
        proto = _proto;
        version = _version;
        Id = _id;
        name = _name; 
        body_type = _body_type;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct PlayerDisconnectData {
    public ushort proto;//2
    public ushort version;//2
    public ushort id;//2
    public ulong token;//8

    public PlayerDisconnectData(ushort _proto, ushort _version, ushort _id, ulong _token) {
        proto = _proto;
        version = _version;
        id = _id;
        token = _token;
    }
}
#endregion

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct KeepAlive {
    public ushort proto;//2
    public ushort version;//2
    public ushort id;//2
    public UInt64 token;//8

    public KeepAlive(ushort _proto, ushort _version, ushort _id, UInt64 _token) {
        proto = _proto;
        version = _version;
        id = _id;
        token = _token;
    }
}

// —труктура которую мы ожидаем от сервера дл€ отправки обратно на сервер состо€ни€ World Data
#region Request Game State
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct RequestGameState {
    public ushort proto;
    public ushort version;

    public RequestGameState(ushort _proto, byte _type) {
        proto = _proto;
        version = _type;
    }
}
#endregion

//ƒанные передаваемые сервером по всем объектам за которыми следит игрок
#region World Data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct WorldData {//528
    public ushort proto;//2
    public ushort version;//2
    public RPC_Object_Data rpc_data;

    public WorldData(ushort _proto, ushort _version, RPC_Object_Data _players) {
        proto = _proto;
        version = _version;
        rpc_data = _players;
    }
}
#endregion

//ƒанные по конкретному клиенту, возвращаемые сервером после обработки
#region Player Data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct RPC_Object_Data {

    public UInt32 object_type;// 1 = player, 2 = warrior, 3 = car, 4 = otherObject
    public UInt32 object_state;// 1 = new, 2 = following, 3 = remote 
    public UInt32 playerID;//4

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
    public byte[] data;

    public RPC_Object_Data(UInt32 oT, UInt32 oS, UInt32 id, byte[] arr) {
        object_type = oT;
        object_state = oS;
        playerID = id;

        data = arr;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct RPC_Object_Register_Data {
    public ushort proto;
    public ushort version;

    public UInt32 object_type;// 1 = player, 2 = warrior, 3 = car, 4 = otherObject
    public UInt32 object_state;// 1 = new, 2 = following, 3 = remote 
    public UInt32 playerID;//4

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
    public byte[] data;

    public RPC_Object_Register_Data(ushort pt, ushort ver, UInt32 oT, UInt32 oS, UInt32 id, byte[] arr) {
        proto = pt;
        version = ver;

        object_type = oT;
        object_state = oS;
        playerID = id;

        data = arr;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct AnimatorParamData {
    public int indexParam;
    public int type;// 0 = float | 1 = bool | 2 = trigger | 3 = integer
    public float defaultFloat;
    public int defaultInt;
    public int defaultBool;
    public int defaultTrigger;// 0 = deactive, 1 = active

    public AnimatorParamData(int _name, int _type, float _float, int _int, int _bool, int _triggerIndex) {
        indexParam = _name;
        type = _type;
        defaultFloat = _float;
        defaultInt = _int;
        defaultBool = _bool;
        defaultTrigger = _triggerIndex;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct PlayerCoreData {
    public int playerID;//4
    public float playerPosX;//4
    public float playerPosY;//4
    public float playerPosZ;//4

    public float playerRotateX;//4
    public float playerRotateY;//4
    public float playerRotateZ;//4

    public float playerVisualRotateX;//4
    public float playerVisualRotateY;//4
    public float playerVisualRotateZ;//4

    public float cameraRotateX;//4
    public float cameraRotateY;//4
    public float cameraRotateZ;//4

    public float aimRotateX;//4
    public float aimRotateY;//4
    public float aimRotateZ;//4

    public float velocityX;//4
    public float velocityY;//4
    public float velocityZ;//4

    //параметры
    public float healPoint;//4
    public float shieldPoint;//4

    public byte usedGunN;
    public byte playerState;
    //аниматор
  /*  [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public float[] animLayerWeight;//12*/

    /* [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
      public AnimatorParamData[] animParam; */

    public InputClientData inputData;

    public PlayerCoreData(int _playerID, float _playerPosX, float _playerPosY, float _playerPosZ, float _playerRotateX, float _playerRotateY, float _playerRotateZ, float _playerVisualRotateX,
        float _playerVisualRotateY, float _playerVisualRotateZ, float _cameraRotateX, float _cameraRotateY, float _cameraRotateZ, float _aimRotateX, float _aimRotateY, float _aimRotateZ,
            float _velocityX, float _velocityY, float _velocityZ, float _healPoint, float _shieldPoint, byte _usedGunN, byte _playerState,/*float[] _animLayerWeight,*/ /*AnimatorParamData[] _animParam*/ InputClientData _inputData) {
        playerID = _playerID;
        playerPosX = _playerPosX;
        playerPosY = _playerPosY;
        playerPosZ = _playerPosZ;
        playerRotateX = _playerRotateX;
        playerRotateY = _playerRotateY;
        playerRotateZ = _playerRotateZ;
        playerVisualRotateX = _playerVisualRotateX;
        playerVisualRotateY = _playerVisualRotateY;
        playerVisualRotateZ = _playerVisualRotateZ;
        cameraRotateX = _cameraRotateX;
        cameraRotateY = _cameraRotateY;
        cameraRotateZ = _cameraRotateZ;
        aimRotateX = _aimRotateX;
        aimRotateY = _aimRotateY;
        aimRotateZ = _aimRotateZ;
        velocityX = _velocityX;
        velocityY = _velocityY;
        velocityZ = _velocityZ;
        healPoint = _healPoint;
        shieldPoint = _shieldPoint;
        usedGunN = _usedGunN;
        playerState = _playerState;
       // animLayerWeight = _animLayerWeight;
        inputData = _inputData;

//        animParam = _animParam; 
    }
}
#endregion

#region Warrior data
[StructLayout(LayoutKind.Sequential, Pack = 1)]
[Serializable]
public struct WarriorData {
    public uint warriorID;

    public uint w_type; // 1 = ближник, 2 = дальник

    public float w_pos_X;
    public float w_pos_Y;
    public float w_pos_Z;

    public float w_rotate_X;
    public float w_rotate_Y;
    public float w_rotate_Z;

    public float w_heal_count;
    public float w_shield_count;

    public float w_target_posX;
    public float w_target_posY;
    public float w_target_posZ;

    public ushort distance_w_attack; // 0 = idle, 1 = fire

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public float[] animLayerWeight;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public AnimatorParamData[] animParam;

    public WarriorData(uint _wID, uint _wType, float wpX, float wpY, float wpZ, float wrX, float wrY, float wrZ, float wHeal, float wShield, float wtPX, float wtPY, 
        float wtPZ, ushort dwa, float[] animArr, AnimatorParamData[] pArr) {
        warriorID = _wID;
        w_type = _wType;

        w_pos_X = wpX;
        w_pos_Y = wpY;
        w_pos_Z = wpZ;

        w_rotate_X = wrX;
        w_rotate_Y = wrY;
        w_rotate_Z = wrZ;

        w_heal_count = wHeal;
        w_shield_count = wShield;

        w_target_posX = wtPX;
        w_target_posY = wtPY;
        w_target_posZ = wtPZ;

        distance_w_attack = dwa;

        animLayerWeight = animArr;

        animParam = pArr;

    }
}


    #endregion

    [Serializable]
public class PlayersOnServerData {
    public uint player_Id;
    public InputPlayerManager playerInputManager;
}

[Serializable]
public class PlayersOnClientData {
    public uint player_Id;
    public PlayerController playerController;
}

[Serializable]
public class ItemTransaction {
    public ItemBaseParametrs item;
    public int itemCount;
}

[Serializable]
public class ItemParametrsArray {
    [Header("“олько дл€ описани€")]
    public string parameterName;
    public string parameterValue;
}
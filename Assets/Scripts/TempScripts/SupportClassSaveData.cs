using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[Serializable]
public class SupportClassSaveData : MonoBehaviour
{
    public static byte[] StructureToByteArray(object structure)
    {
        int lenght = Marshal.SizeOf(structure);
        byte[] _array = new byte[lenght];
        print("lenght " + lenght);
        IntPtr ptr = Marshal.AllocHGlobal(lenght);
        Marshal.StructureToPtr(structure, ptr, false);
        Marshal.Copy(ptr, _array, 0, lenght);
        Marshal.FreeHGlobal(ptr);
        return _array;
    }
    //public static byte[] getBytes(object str)
    //{
    //    int size = Marshal.SizeOf(str);
    //    byte[] arr = new byte[size];
    //    IntPtr ptr = Marshal.AllocHGlobal(size);
    //    Marshal.StructureToPtr(str, ptr, true);
    //    Marshal.Copy(ptr, arr, 0, size);
    //    Marshal.FreeHGlobal(ptr);
    //    return arr;
    //}

    public static PacketQuestData StructureAllTimeDataReturn(byte[] arr)
    {
        PacketQuestData packet = new PacketQuestData();
        int size = Marshal.SizeOf(packet);
        IntPtr prt = Marshal.AllocHGlobal(size);
        Marshal.Copy(arr, 0, prt, size);
        try
        {
            packet = (PacketQuestData)Marshal.PtrToStructure(prt, typeof(PacketQuestData));
        }
        catch (Exception ex)
        {
            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(ex.ToString(), "Array to Str InputClientData");
        }
        finally
        {
            Marshal.FreeHGlobal(prt);
        }
        return packet;


     
    }

    ////  DataSaveQuest packet = new DataSaveQuest();
    //int size = Marshal.SizeOf(arr.Length);
    //Debug.Log(size);
    //IntPtr prt = Marshal.AllocHGlobal(size);
    //Marshal.Copy(arr, 0, prt, size);
    //DataSaveQuest packet = (DataSaveQuest)Marshal.PtrToStructure(prt, typeof(DataSaveQuest));
    //Marshal.FreeHGlobal(prt);
    //return packet;
    //public void ByteArrayToStructure(byte[] bytearray, ref object obj)
    //{
    //    int len = Marshal.SizeOf(obj);
    //    IntPtr i = Marshal.AllocHGlobal(len);
    //    Marshal.Copy(bytearray, 0, i, len);
    //    obj = Marshal.PtrToStructure(i, obj.GetType());
    //    Marshal.FreeHGlobal(i);
    //}
    //public static DataSaveQuest fromBytes(byte[] arr)
    //{
    //    DataSaveQuest str = new DataSaveQuest();
    //    int size = Marshal.SizeOf(str);
    //    IntPtr ptr = IntPtr.Zero;
    //    try
    //    {
    //        ptr = Marshal.AllocHGlobal(size);
    //        Marshal.Copy(arr, 0, ptr, size);
    //        str = (DataSaveQuest)Marshal.PtrToStructure(ptr, str.GetType());
    //    }
    //    finally
    //    {
    //        Marshal.FreeHGlobal(ptr);
    //    }
    //    return str;
    //}

    //public static T fromBytes<T>(byte[] arr)
    //{
    //    T str = default(T);
    //    int size = Marshal.SizeOf(str);
    //    IntPtr ptr = Marshal.AllocHGlobal(size);
    //    Marshal.Copy(arr, 0, ptr, size);
    //    str = (T)Marshal.PtrToStructure(ptr, str.GetType());
    //    Marshal.FreeHGlobal(ptr);
    //    return str;
    //}

    //public static byte[] ObjectToByteArray(object obj)
    //{
    //    BinaryFormatter bf = new BinaryFormatter();
    //    using (var ms = new MemoryStream())
    //    {
    //        bf.Serialize(ms, obj);
    //        return ms.ToArray();
    //    }
    //}

    //public static object ByteArrayToObject(byte[] arrBytes)
    //{
    //    using (var memStream = new MemoryStream())
    //    {
    //        var binForm = new BinaryFormatter();
    //        memStream.Write(arrBytes, 0, arrBytes.Length);
    //        memStream.Seek(0, SeekOrigin.Begin);
    //        var obj = binForm.Deserialize(memStream);
    //        return obj;
    //    }
    //}


    //Данные которые получает серверный клиент после того, как сервер обработает данные о нажатых клавишах клиентом

    //

    #region DataQuest
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [Serializable]
    public struct PacketQuestData
    {
        public ushort proto;
        public ushort version;
        public int questID;
        public int stepQuest;
        public int numberAction;
        public int currentCount;

        public PacketQuestData(ushort _proto, ushort _version, int _questID, int _stepQuest, int _numberAction, int _currentCount)
        {
            proto = _proto;
            version = _version;
            questID = _questID;
            stepQuest = _stepQuest;
            numberAction = _numberAction;
            currentCount = _currentCount;
        }
    }
    #endregion


  



}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportedClass
{
    public static string GenerateUniKey(string iniciator)
    {
        string infoBlock = iniciator + UnityEngine.Random.Range(0, 100);
        return infoBlock;
    }
}

[Serializable]
public class MyIndividualBlock
{
    public string userID;
    public string userName;
}
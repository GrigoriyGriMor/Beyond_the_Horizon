using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ReloadInfo", menuName = "Reload Info", order = 101)]
public class ReloadParam : ScriptableObject
{
    [SerializeField] private WeaponReloadInfo reloadInfo;

    public WeaponReloadInfo GetInfo()
    {
        return reloadInfo;
    }
}

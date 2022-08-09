using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[AddComponentMenu("JU TPS/Gameplay/Weapon System/Weapon Aim Rotation Center")]
public class WeaponAimRotationCenter : MonoBehaviour
{
    public int WeaponPositionsLengh;
    public List<string> WeaponPositionName = new List<string>();
    public List<Transform> WeaponPositionTransform = new List<Transform>();
    public List<int> ID = new List<int>();

    public List<Vector3> _storedLocalPositions = new List<Vector3>();
    public List<Quaternion> _storedLocalRotations = new List<Quaternion>();

    public void CreateWeaponPositionReference(string name)
    {
        WeaponPositionName.Add(name);
        WeaponPositionTransform.Add(default(Transform));

        ID.Add(WeaponPositionsLengh);

        Vector3 NewPosition = new Vector3(0,0,0);
        _storedLocalPositions.Add(NewPosition);

        Quaternion NewRotation = new Quaternion(0,0,0,0);
        _storedLocalRotations.Add(NewRotation);

        WeaponPositionsLengh++;
        StoreLocalTransform();
    }
    public void RemoveWeaponPositionReference(int index)
    {
        WeaponPositionName.RemoveAt(index);
        WeaponPositionTransform.RemoveAt(index);
        ID.RemoveAt(index);
        WeaponPositionsLengh = WeaponPositionName.Count - 1;
        _storedLocalPositions.RemoveAt(index);
        _storedLocalRotations.RemoveAt(index);
        StoreLocalTransform();
    }
    public void StoreLocalTransform()
    {
        for (int i = 0; i < WeaponPositionTransform.Count; i++)
        {
            if (WeaponPositionTransform[i] == null) return;

            _storedLocalPositions[i] = WeaponPositionTransform[i].localPosition;
            _storedLocalRotations[i] = WeaponPositionTransform[i].localRotation;
        }
    }
    private void Start()
    {
        StoreLocalTransform();
    }
}

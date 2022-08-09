using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemParamPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _value;

    public void SetParam(string _n, string _v) {
        _name.text = _n;
        _value.text = _v;
    }
}

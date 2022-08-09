using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoodParamInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text t_name;
    [SerializeField] private TMP_Text t_value;

    public void Init(string _name, string _value) {
        t_name.text = _name;
        t_value.text = _value;
    }
}

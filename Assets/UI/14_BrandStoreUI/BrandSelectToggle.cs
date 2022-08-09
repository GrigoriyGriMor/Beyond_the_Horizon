using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrandSelectToggle : MonoBehaviour
{
    [SerializeField] private Text toggle_name;
    private string toggle_tag;
    private int toggle_tag_int;
    
    [SerializeField] private Toggle toggle;

    private void Start() {
        if (toggle == null)
            Debug.LogError("Toggle component not found in object BrandSelectToggle");
    }

    public void Init(string _name, string _tag, ToggleGroup tg = null) {
        toggle_name.text = _name;
        toggle_tag = _tag;

        if (tg != null)
            toggle.group = tg;
    }

    public void Init(string _name, int _tag, ToggleGroup tg = null) {

        toggle_name.text = _name;
        toggle_tag_int = _tag;

        if (tg != null)
            toggle.group = tg;
    }

    public bool GetStatus() {
        return toggle.isOn;
    }

    public string GetTglTag() {
        return toggle_tag;
    }

    public int GetTglIntTag() {
        return toggle_tag_int;
    }

    public void OffToggle() {
        toggle.isOn = false;
    }
}

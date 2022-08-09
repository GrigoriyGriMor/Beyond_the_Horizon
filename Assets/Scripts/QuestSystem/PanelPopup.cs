using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelPopup : MonoBehaviour
{
    public  Image imagePopup;
    public Text textTitle;
    public Text text;

    public void SetValuePanelPopup(Sprite imagePopup, string text, string textTitle)
    {
        this.imagePopup.sprite = imagePopup; 
        this.textTitle.text = text.ToString();
        this.text.text = textTitle.ToString();
    }
}

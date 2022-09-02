using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUISpriteChanger : MonoBehaviour
{
    [Header("Если UI элемент")]
    [SerializeField] private Image buttonImage;

    [Header("Если Sprite объект на сцене")]
    [SerializeField] private SpriteRenderer buttonSprite;

    [Header("first-включенная / second-выключенная")]
    [SerializeField] private Sprite s_first;
    [SerializeField] private Sprite s_second;

     void Start()
     {
        if (SoundManagerAllControll.Instance) SoundManagerAllControll.Instance.SoundButtonInit(gameObject.GetComponent<ButtonUISpriteChanger>());
     }

    public void ChangeSprite(bool On)
    {
        if (buttonImage != null)
        {
            if (!On)
                buttonImage.sprite = s_second;
            else
                buttonImage.sprite = s_first;
        }
        else
        if (buttonSprite != null)
        {
            if (!On)
                buttonSprite.sprite = s_second;
            else
                buttonSprite.sprite = s_first;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUISpriteChanger : MonoBehaviour
{
    // private Button b_parent;
    [SerializeField] private Image buttonImage;
    [SerializeField] private SpriteRenderer buttonSprite;
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

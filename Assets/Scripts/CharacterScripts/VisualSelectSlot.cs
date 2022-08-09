using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualSelectSlot : MonoBehaviour
{
    [SerializeField] Image visual_sprite;

    public void SetImage(Sprite sprite) {
        visual_sprite.sprite = sprite;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityButtonNameRequest : MonoBehaviour
{
    [SerializeField] private InputPlayerManager player;
    [SerializeField] private Text buttonName;

    [Header("Выбирете действие")]
    [SerializeField] private Keys selectDoing;

    private void Start() {
        if (player != null) {
            switch (selectDoing) {
                case Keys.lookCameraR:
                    buttonName.text = player.lookCameraRotation.ToString();
                    break;
                case Keys.exitFightMode:
                    buttonName.text = player.exitFightMode.ToString();
                    break;
                case Keys.jump:
                    buttonName.text = player.jump.ToString();
                    break;
                case Keys.crouch:
                    buttonName.text = player.crouch.ToString();
                    break;
                case Keys.sprint:
                    buttonName.text = player.sprint.ToString();
                    break;
                case Keys.weapon_1:
                    buttonName.text = player.weapon_1.ToString();
                    break;
                case Keys.weapon_2:
                    buttonName.text = player.weapon_2.ToString();
                    break;
                case Keys.fire:
                    buttonName.text = player.fire.ToString();
                    break;
                case Keys.reloadWeapon:
                    buttonName.text = player.reloadWeapon.ToString();
                    break;
                case Keys.aiming:
                    buttonName.text = player.aiming.ToString();
                    break;
                case Keys.useObject:
                    buttonName.text = player.uesObject.ToString();
                    break;
                case Keys.useGrenade:
                    buttonName.text = player.useGrenade.ToString();
                    break;
            }
        }
    }
}

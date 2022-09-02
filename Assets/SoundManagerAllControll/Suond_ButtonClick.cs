using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suond_ButtonClick : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    public void PlaySound() {
        if (SoundManagerAllControll.Instance && clip) SoundManagerAllControll.Instance.ClipPlay(clip, SoundManagerAllControll.SoundPriority.UI);
    }
}

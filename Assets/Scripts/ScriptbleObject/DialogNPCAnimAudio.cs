using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogNPC", menuName = "DialogNPC", order = 100)]
public class DialogNPCAnimAudio : ScriptableObject
{
    public TriggerAnimAudioClip[] triggerAnimAudioClips;
}


[System.Serializable]
public class TriggerAnimAudioClip
{
    /// <summary>
    /// Текст NPC
    /// </summary>
    public string textNPC;

    /// <summary>
    /// Триггер анимации
    /// </summary>
    public string triggerAnimator;

    /// <summary>
    /// Клип Аудио
    /// </summary>
    public AudioClip audioClip;
  
}


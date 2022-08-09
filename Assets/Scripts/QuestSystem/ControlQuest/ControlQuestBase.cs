using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControlQuestBase : MonoBehaviour
{
    [Header("ID Quest ")]
    [SerializeField]
    protected int ID;

    [Header("Step Quest ")]
    [SerializeField]
    protected int stepQuest;

    [Header("Action Count")]
    [SerializeField]
    protected int numberAction;

    protected QuestManager questManager;

    //[HideInInspector]
    //public DialogBase dialogBase;

    [HideInInspector]
    public void CheckQuestPlayer(QuestManager questManager)
    {
        if (questManager)
        {
            questManager.CheckStep(ID, stepQuest, numberAction);
        }
    }
}

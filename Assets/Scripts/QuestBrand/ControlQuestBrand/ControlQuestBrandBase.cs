using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlQuestBrandBase : MonoBehaviour
{
    [Header("ID Quest Brand ")]
    [SerializeField]
    protected int ID;

    [Header("Step Quest ")]
    [SerializeField]
    protected int stepQuest;

    [Header("Action Count")]
    [SerializeField]
    protected int numberAction;

    protected QuestBrandManager questBrandManager;

    public void SetQuestBrandID(int ID)
    {
        this.ID = ID;
    }

    [HideInInspector]
    public void CheckQuestPlayer(QuestBrandManager questBrandManager)
    {
        if (questBrandManager)
        {
            questBrandManager.CheckStep(ID, stepQuest, numberAction);
        }
        else
        {
            Debug.LogError(" Not QuestBrandManager");
        }
    }

}

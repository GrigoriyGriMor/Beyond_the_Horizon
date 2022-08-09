using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorQuestBrandBase : MonoBehaviour
{
    [Header("Quest Brand Data")]
    public QuestData questData;
    
    [HideInInspector]
    protected void SetActiveQuestBrand(QuestBrandManager questBrandManager)
    {
        if (questBrandManager)
        {
            questBrandManager.SetActiveQuestBrand(questData.ID);
        }
        else
        {
            Debug.LogError(" Not QuestBrandManager");
        }
    }
}

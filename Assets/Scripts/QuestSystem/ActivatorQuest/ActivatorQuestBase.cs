using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorQuestBase : MonoBehaviour
{   [Header(" ID Quest")]
    public int ID;

    public void SetQuestPlayer(QuestManager questManager)
    {
        if (questManager)
        {
            questManager.CreateQuest(ID);
        }
        else
        {
            Debug.LogError(" NotQuestManager");
        }
    }
}

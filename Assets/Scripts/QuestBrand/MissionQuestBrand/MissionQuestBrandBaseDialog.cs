using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionQuestBrandBaseDialog : MissionQuestBrandBase
{
    private NpcController npcController;

    private void Start()
    {
        if (!TryGetComponent(out npcController)) Debug.LogError(" Not NpcController");
    }

    public void CheckQuestDialog()
    {
        questBrandManager = npcController.currentPlayer.GetComponent<QuestBrandManager>();
        if (questBrandManager)
        {
            CheckQuestPlayer(questBrandManager);
        }
        else
        {
            Debug.Log(" Not QuestBrandManager");
        }
    }
}

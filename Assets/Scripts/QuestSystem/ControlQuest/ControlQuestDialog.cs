using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlQuestDialog : ControlQuestBase
{
   // [SerializeField]
    //private QuestManager questManager;
   // [SerializeField]
    private NpcController npcController;
    

    private void Start()
    {
        if (!TryGetComponent(out npcController)) Debug.LogError(" Not NpcController");
    }

    public void CheckQuestDialog()
    {
        questManager = npcController.currentPlayer.GetComponent<QuestManager>();
        if (questManager)
        {
            CheckQuestPlayer(questManager);
        }
        else
        {
            Debug.Log(" NotQuestManager");
        }
    }
}

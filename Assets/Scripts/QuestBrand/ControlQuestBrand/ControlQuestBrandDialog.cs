using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlQuestBrandDialog : ControlQuestBrandBase
{
    // [SerializeField]
    //private QuestManager questManager;
    // [SerializeField]
    private NpcController npcController;

    [Header("След диалог если Quest активный")]
    [SerializeField]
    private int nextDialogActiveQuest;

    [Header("След диалог по шагам")]
    [SerializeField]
    private NextDialogSteps[] nextDialogSteps;

    [Header("След диалог если Quest выполненый")]
    [SerializeField]
    private int nextDialogDoneQuest;

    private void Start()
    {
        if (!TryGetComponent(out npcController)) Debug.LogError(" Not NpcController");
    }


    public void SetNextDialog(QuestBrandManager questBrandManagerPlayer)
    {
        questBrandManager = questBrandManagerPlayer;
        if (questBrandManager)
        {
            if (questBrandManager.CheckDoneQuest(ID))
            {
                npcController.dialogBase.indexArrayDialog = nextDialogDoneQuest;
                return;
            }

            if (questBrandManager.CheckQuest(ID, out int currentStepNumber))
            {
                if (nextDialogSteps.Length > 0)
                {
                    for (int indexNextDialogStep = 0; indexNextDialogStep < nextDialogSteps.Length; indexNextDialogStep++)
                    {
                        if (currentStepNumber == nextDialogSteps[indexNextDialogStep].questStep)
                        {
                            npcController.dialogBase.indexArrayDialog = nextDialogSteps[indexNextDialogStep].nextDialog;
                            return;
                        }
                    }
                }
                npcController.dialogBase.indexArrayDialog = nextDialogActiveQuest;
                return;
            }
        }
        else
        {
            Debug.LogError(" NotQuestManager");
        }
    }

    public void CheckQuestDialog()
    {
        if (npcController)
        {
            questBrandManager = npcController.currentPlayer.GetComponent<QuestBrandManager>();
        }
        else
        {
            Debug.Log(" Not NpcController");
        }

        if (questBrandManager)
        {
            CheckQuestPlayer(questBrandManager);
        }
        else
        {
            Debug.Log(" NotQuestManager");
        }
    }

    [System.Serializable]
    public class NextDialogSteps
    {
        public int questStep;
        public int nextDialog;
    }

}

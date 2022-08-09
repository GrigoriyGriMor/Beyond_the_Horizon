using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorQuestDialog : ActivatorQuestBase
{
    //[SerializeField]
    private QuestManager questManager;
    [HideInInspector]
    public NpcController npcController;

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
        TryGetComponent(out npcController);
    }

    public void SetNextDialog(QuestManager questManagerPlayer)
    {
        questManager = questManagerPlayer;
        if (questManager)
        {
            if (questManager.CheckDoneQuest(ID))
            {
                npcController.dialogBase.indexArrayDialog = nextDialogDoneQuest;
                return;
            }

            if (questManager.CheckQuest(ID, out int currentStepNumber))
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


    public void SetQuestDialog()
    {
        questManager = npcController.currentPlayer.GetComponentInChildren<QuestManager>();

        if (questManager)
        {
            SetQuestPlayer(questManager);
        }
        else
        {
            Debug.LogError(" NotQuestManager");
        }
    }

    [System.Serializable]
    public class NextDialogSteps
    {
        public int questStep;
        public int nextDialog;
    }
}

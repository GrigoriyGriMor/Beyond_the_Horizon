using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorQuestBrandDialog : ActivatorQuestBrandBase
{
    //[SerializeField]
    private QuestBrandManager questBrandManager;

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

    [HideInInspector]
    public void SetNextDialog(QuestBrandManager questBrandManagerPlayer)
    {
        questBrandManager = questBrandManagerPlayer;
        if (questBrandManager)
        {
            if (questBrandManager.CheckDoneQuest(questData.ID))
            {
                npcController.dialogBase.indexArrayDialog = nextDialogDoneQuest;
                return;
            }

            if (questBrandManager.CheckQuest(questData.ID, out int currentStepNumber))
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

    public void SetQuestBrandDialog()
    {
        if (npcController) questBrandManager = npcController.currentPlayer.GetComponentInChildren<QuestBrandManager>();

        if (questBrandManager)
        {
            SetActiveQuestBrand(questBrandManager);
        }
        else
        {
            Debug.LogError(" Not QuestBrandManager");
        }
    }


    [System.Serializable]
    public class NextDialogSteps
    {
        public int questStep;
        public int nextDialog;
    }
}

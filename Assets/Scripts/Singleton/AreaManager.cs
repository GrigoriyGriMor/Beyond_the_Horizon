using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private static AreaManager instance;
    public static AreaManager Instance => instance;
    [SerializeField]
    private PlayerController playerController;
    [Header("-------------------------------------")]
    [Header(" Обычные квесты нужно заполнить")]
    public List<TargetQuest> targetQuests; 

    [Header ("-------------------------------------")]
    [Header ("Не заполнять")]
    public List<TargetQuest> targetQuestsBrand;
    [HideInInspector]
    public TargetManager targetManager;
    [HideInInspector]
    public QuestManager questManager;

    private void Awake()
    {
        instance = this;
        StartCoroutine(SetPlayer());
    }

   /* private void Update()
    {
      //  if (!playerController && PlayerParameters.Instance) SetPlayerController(PlayerParameters.Instance.GetPlayerController());  //crutch
    }*/
    private IEnumerator SetPlayer() {
        while (!PlayerParameters.Instance) {
            yield return new WaitForFixedUpdate();
        }

        while (playerController == null) {
            yield return new WaitForFixedUpdate();
            playerController = PlayerParameters.Instance.GetPlayerController();
        }

        SetPlayerController(playerController);//need correct
    }

public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
        targetManager = playerController.GetComponent<TargetManager>();
        questManager = playerController.GetComponent<QuestManager>();
    }
}

[System.Serializable]
public class TargetQuest
{
    public int ID;
    public List<StepTarget> stepTargets;
}

[System.Serializable]
public class StepTarget
{
    public List<ActionTarget> actionTargets;
}

[System.Serializable]
public class ActionTarget
{
    public Transform actionTarget;
}

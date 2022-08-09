using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum ModeNPC
{
    staticNpc,
    dinamicNpc,
    staticDinamicNpc
}

public enum ModeDialog
{
    random,
    dialog,
    call
}

public class NpcController : AbstractIO
{
    [HideInInspector]
    public DialogBase dialogBase;
    public Animator animator;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    public InteractbleObjectController interactbleObjectController;
    [Header(" Name NPC ")]
    public Text refTextNPC;

    [HideInInspector]
    public Transform currentPlayer;

    // состояние NPC
    //[HideInInspector]
    public StateNPC stateNPC;
    public virtual void StartDialog(PlayerController playerController)
    {
        //Debug.LogError(" Start Dialog NPC"+ playerController.playerID);
    }
    public virtual void EndDialog()
    {
       // Debug.LogError(" End Dialog NPC");
    }
}

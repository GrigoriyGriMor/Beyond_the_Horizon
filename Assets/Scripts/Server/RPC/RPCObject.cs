using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPCObject : MonoBehaviour
{
    [Header("Крепить на объект с контроллером!!!")]

    public bool following = false;
    public SupportClass.RPC_Type object_type = SupportClass.RPC_Type.otherObject;
    [HideInInspector] public InputPlayerManager player;
    [HideInInspector] public EnemyBase warrior; 

    private void Awake() {

        switch (object_type) {
            case SupportClass.RPC_Type.player:
                    player = gameObject.GetComponent<InputPlayerManager>();
                    if (player == null)
                        player = gameObject.GetComponentInChildren<InputPlayerManager>();
                break;
            case SupportClass.RPC_Type.warrior:
                warrior = gameObject.GetComponent<EnemyBase>();
                if (warrior == null)
                    warrior = gameObject.GetComponentInChildren<EnemyBase>();
                break;
            case SupportClass.RPC_Type.NPC:

                break;
            case SupportClass.RPC_Type.otherObject:

                break;
        }
    }

    private void OnEnable() {
        if (RPCController.Instance) RPCController.Instance.RegisterNewObj(this);
    }

    private void OnDisable() {
        if (!following) return;
        if (RPCController.Instance) RPCController.Instance.RemoveSceneObj(this);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionQuestBrandBase : MonoBehaviour
{
    [Header("ID Quest Brand")]
    [SerializeField]
    public int ID;

    [Header("Step Quest Brand")]
    [SerializeField]
    protected int stepQuest;

    [Header("Action Count ")]
    [SerializeField]
    protected int numberAction;  
    
    [Header(" Use for check Car ")]
    [SerializeField]
    protected bool checkForCar;



    [SerializeField]
    protected QuestBrandManager questBrandManager;

    private void Start()
    {
        
    }

    [HideInInspector]
    public void SetQuestBrandID(int ID)
    {
        this.ID = ID;
    }

    [HideInInspector]
    public void CheckQuestPlayer(QuestBrandManager questBrandManager)
    {
        //print(" Bot Death Brand Quest");
        if (questBrandManager)
        {
            questBrandManager.CheckStep(ID, stepQuest, numberAction);
        }
        else
        {
            print(" Not QuestBrandManager "); 
        }
    }
}

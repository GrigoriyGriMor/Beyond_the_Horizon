using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionQuestBrandCar : MissionQuestBrandBase
{
    public void CheckQuestBrand()
    {
        questBrandManager = GetComponent<CarBase>().GetQuestBrandManager();
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

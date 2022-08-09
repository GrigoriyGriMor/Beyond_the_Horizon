using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MissionQuestBrandTrigger : MissionQuestBrandBase
{
    private void OnTriggerEnter(Collider other)
    {
        //print(" trigger ");
        if (checkForCar)
        {
            if (other.TryGetComponent(out CarBase carBase))
            {
                CheckQuestPlayer(carBase.GetQuestBrandManager());
                return;
            }
            else
            {
                print(" Not CarBase");
            }
        }
        else
        {
            if (other.TryGetComponent(out QuestBrandManager questBrandManager))
            {
                CheckQuestPlayer(questBrandManager);
                return;
            }
            else
            {
                print(" Not QuestBrandManager");
            }
        }
    }


    //private void OnCollisionEnter(Collision other)
    //{
    //    if (other.transform.TryGetComponent(out QuestBrandManager questBrandManager))
    //    {
    //        CheckQuestPlayer(questBrandManager);
    //        return;
    //    }

    //    if (other.transform.TryGetComponent(out CarBase carBase))
    //    {
    //        //CheckQuestPlayer(carBase.GetQuestManager());
    //        return;
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorQuestBrandTrigger : ActivatorQuestBrandBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out QuestBrandManager questBrandManager))
        {
            SetActiveQuestBrand(questBrandManager);
            return;
        }

        if (other.TryGetComponent(out CarBase carBase))
        {
           // SetQuestPlayer(carBase.GetQuestManager());
          //  return;
        }
    }


}

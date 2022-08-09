using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlQuestTrigger : ControlQuestBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out QuestManager questManager))
        {
            CheckQuestPlayer(questManager);
            return;
        }

        if (other.TryGetComponent(out CarBase carBase))
        {
            CheckQuestPlayer(carBase.GetQuestManager());
            return;
        }
    }
}

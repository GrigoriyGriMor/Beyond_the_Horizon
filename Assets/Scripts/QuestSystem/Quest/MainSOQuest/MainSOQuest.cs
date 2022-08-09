using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMainSOQuest", menuName = "NewMainSOQuest", order = 100)]
public class MainSOQuest : ScriptableObject
{
    [SerializeField]
    private Quest[] quests;

    public Quest GetQuest(int ID)
    {
        for (int indexQuest = 0; indexQuest < quests.Length; indexQuest++)
        {
            if (quests[indexQuest].questData.ID == ID)
            return quests[indexQuest];
        }
        return null;
    }
}

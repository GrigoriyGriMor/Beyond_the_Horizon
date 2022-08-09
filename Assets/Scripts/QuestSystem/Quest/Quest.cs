using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewQuest", menuName = "NewQuest", order = 100)]
public class Quest : ScriptableObject
{
    public QuestData questData;
}

[System.Serializable]
public class QuestData : ICloneable
{
    public int ID;
    public string titleQuest;
    public string discriptionQuest;
    public TypeQuest typeQuest;
    public bool timerQuest;
    public int day = 1;
    public int hour = 0;
    public int minute = 0;
    public DateTime questDateTime; 
    public int levelPlayer;
    public StatusQuest statusQuest;
    [Header(" Награда ")]
    public int experience;
    public int coins;
    public ItemBaseParametrs[] itemBase;
    [HideInInspector]
    public int currentStepNumber;
    [Header(" Image ")]
    public Sprite image1x1;
    public Sprite image2x1;
    public StepsQuest[] arrayStepsQuests;

    public object Clone()
    {
        var questData = (QuestData)this.MemberwiseClone();
        StepsQuest[] tempArrayStepsQuest = new StepsQuest[arrayStepsQuests.Length];
        for (int index = 0; index < questData.arrayStepsQuests.Length; index++)
        {
            tempArrayStepsQuest[index] = (StepsQuest)this.arrayStepsQuests[index].Clone();
            questData.arrayStepsQuests = tempArrayStepsQuest;
        }
        return questData;
    }
}

[System.Serializable]
public class StepsQuest : ICloneable
{
    public int nextStepNumber;
    public string titleStep;
    public string descriptionStep;
    public ActionCount[] actionCounts;
    //public Sprite[] imageQuest;

    public object Clone()
    {
        var stepsQuest = (StepsQuest)this.MemberwiseClone();
        ActionCount[] tempActionCounts = new ActionCount[actionCounts.Length];
        for(int index = 0; index < stepsQuest.actionCounts.Length; index++)
        {
            tempActionCounts[index] = (ActionCount)this.actionCounts[index].Clone();
            stepsQuest.actionCounts = tempActionCounts;
        }

        return stepsQuest;
    }
}

[System.Serializable]
public class ActionCount : ICloneable
{
    public string textAction;
    [HideInInspector]
    public int currentCount;
    public int maxCount;

    public object Clone()
    {
        var actionCount = (ActionCount)this.MemberwiseClone();
        return actionCount;
    }
}

public enum TypeQuest
{
    story,
    daily
}

public enum StatusQuest
{
    none,
    inProcess,
    done
}

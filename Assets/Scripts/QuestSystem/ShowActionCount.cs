using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowActionCount : MonoBehaviour
{
    public StringAction[] arrayStringAction;

    public void ShowCurrentQuest(int index, bool chechBox, string textAction, string textCount, bool offCount)
    {
        if (arrayStringAction.Length > index)
        {
            if (arrayStringAction.Length > 0)
            {
                arrayStringAction[index].gameObject.SetActive(true);
                arrayStringAction[index].SetAnimation();
                arrayStringAction[index].SetStringAction(chechBox, textAction, textCount, offCount);
            }
        }
    }
}
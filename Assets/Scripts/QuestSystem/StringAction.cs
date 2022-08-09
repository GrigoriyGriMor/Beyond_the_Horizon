using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringAction : MonoBehaviour
{
    [SerializeField]
    private Transform chechBox;
    [SerializeField]
    private Text textAction;
    [SerializeField]
    private Transform countField;
    [SerializeField]
    private Text textCount;
    [SerializeField]
    private Animator animator;

    public void SetStringAction(bool chechBox, string textAction, string textCount, bool offCount)
    {
        this.chechBox.gameObject.SetActive(chechBox);
        this.textAction.text = textAction.ToString();
        this.textCount.text = textCount.ToString();
        countField.gameObject.SetActive(offCount);
    }
  
    public void SetAnimation()
    {
        if (animator) animator.SetTrigger("IN");
    }

}

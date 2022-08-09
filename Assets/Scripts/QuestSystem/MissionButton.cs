using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionButton : MonoBehaviour
{
    [SerializeField]
    private Image missionIcon;
    [SerializeField]
    private Transform chechBox;
    [SerializeField]
    private Text missionTitle;
    [SerializeField]
    private Transform missionTimer;
    [SerializeField]
    private Text missionTimerText;
    [SerializeField]
    private Transform tagImage;
    private bool isActiveTagImage;

    public void SetStringAction(bool chechBox, Sprite missionIcon, string missionTitle)
    {
        this.missionIcon.sprite = missionIcon;
        this.chechBox.gameObject.SetActive(chechBox);
        this.missionTitle.text = missionTitle;
       
    }

    public bool GetTagImage()
    {
        return isActiveTagImage;
    }

    public void SetTagImage(bool isActiveTagImage)
    {
        this.isActiveTagImage = isActiveTagImage;
        tagImage.gameObject.SetActive(isActiveTagImage);
    }

    public void SetMissionTimer(bool missionTimer, string missionTimerText)
    {
        this.missionTimer.gameObject.SetActive(missionTimer);
        this.missionTimerText.text = missionTimerText;
    }

}

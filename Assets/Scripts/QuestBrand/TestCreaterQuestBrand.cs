using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCreaterQuestBrand : MonoBehaviour
{
    private float timerCreateQuestBrand = 0.0f;

    [SerializeField]
    private CreateQuestBrend[] createQuestBrend;

    [SerializeField]
    private QuestBrandData currentQuestBrandData;

    [SerializeField]
    private string[] nameBrand = new string[] { "Victoria", "LogOfMemory", "Ural", " Moscvich" };

    [SerializeField]
    private int minOffsetPosition = -5;
    [SerializeField]
    private int maxOffsetPosition = 5;


    [SerializeField]
    private Texture[] logoBrand;

    private void Start()
    {
        StartCoroutine(TimerCreaterQuestBrand());
    }

    private void CreaterQuest(int index)
    {
        if (index > this.createQuestBrend.Length - 1 && this.createQuestBrend.Length > 0) return;
        CreateQuestBrend createQuestBrend = Instantiate(this.createQuestBrend[index]);
        createQuestBrend.Init(SetQuestBrandDataTest(currentQuestBrandData));
    }


    private QuestBrandData SetQuestBrandDataTest(QuestBrandData currentQuestBrandData)
    {
        currentQuestBrandData.ID = Random.Range(1, 1000);
        currentQuestBrandData.typeQuest = (byte)Random.Range(1, 4);
        currentQuestBrandData.day = (byte)Random.Range(1, 31);
        currentQuestBrandData.hour = (byte)Random.Range(1, 25);
        currentQuestBrandData.min = (byte)Random.Range(1, 59);
        currentQuestBrandData.nameBrand = GenerateString();
        currentQuestBrandData.refLogoBrand = GenerateLogoBrand();
        currentQuestBrandData.countAwards = Random.Range(1, 59);
        currentQuestBrandData.stateQuestBrand = (byte)Random.Range(1, 4);
        currentQuestBrandData.offSetPosGetQuestX = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.offSetPosGetQuestZ = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.offSetPosMissionObjectX = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.offSetPosMIssionObjectZ = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.offSetPosEndObjectX = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.offSetPosEndObjectZ = Random.Range(minOffsetPosition, maxOffsetPosition);
        currentQuestBrandData.count = Random.Range(1, 100);

        return currentQuestBrandData;
    }

    private int tempIndexNameBrand;

    private string GenerateString()
    {
        tempIndexNameBrand = Random.Range(0, nameBrand.Length);
        return nameBrand[tempIndexNameBrand];
    }

    private Texture GenerateLogoBrand()
    {
        return logoBrand[tempIndexNameBrand];
    }

    private IEnumerator TimerCreaterQuestBrand()
    {
        while (PlayerParameters.Instance.GetPlayerController() == null)
            yield return new WaitForFixedUpdate(); 

        int countQuest = 0;

        while (countQuest < 3)
        {
            yield return new WaitForSeconds(timerCreateQuestBrand);
            timerCreateQuestBrand = Random.Range(1, 5);

            CreaterQuest(countQuest);

            //print(" countQuest = " + countQuest);
            countQuest++;

        }
    }

}

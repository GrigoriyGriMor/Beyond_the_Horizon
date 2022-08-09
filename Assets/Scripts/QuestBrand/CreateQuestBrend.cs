using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateQuestBrend : MonoBehaviour
{
    [SerializeField]
    private string textNameNPC = "{0} Quest NPC";
    [SerializeField]
    private Transform objectGetQuest;
    [SerializeField]
    private Transform[] objectMissionObject = new Transform[1];
    [SerializeField]
    private Transform objectEndObject;
    [SerializeField]
    private SpawenBotsQuestBrand spawenBotsQuestBrand;
    private PlayerController playerController;
    private QuestBrandManager questBrandManager;
    [SerializeField]
    private QuestBrandData questBrandData;

    [SerializeField]
    private Sprite defaultLogoBrand;

    [SerializeField]
    private MeshRenderer[] boards;

    //[SerializeField]
    //private QuestBrandData currentQuestBrandData;

    void Start()
    {

    }

    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
        questBrandManager = playerController.GetComponent<QuestBrandManager>();
    }

    void Update()
    {
        if (!playerController) SetPlayerController(PlayerParameters.Instance.GetPlayerController());  //crutch

    }

    public void Init(QuestBrandData questBrandData)
    {
        StartCoroutine(StartSetQuestBrand(questBrandData));
    }

    private void CreateQuestBrand(QuestBrandData questBrandData)
    {
        this.questBrandData = new QuestBrandData(questBrandData.ID, questBrandData.typeQuest, questBrandData.day, questBrandData.hour, questBrandData.min, questBrandData.nameBrand, questBrandData.refLogoBrand, questBrandData.countAwards,
        questBrandData.stateQuestBrand, questBrandData.offSetPosGetQuestX, questBrandData.offSetPosGetQuestZ, questBrandData.offSetPosMissionObjectX, questBrandData.offSetPosMIssionObjectZ, questBrandData.offSetPosEndObjectX, questBrandData.offSetPosEndObjectZ, questBrandData.count);

        SetPositionObjects(objectGetQuest, new Vector3(questBrandData.offSetPosGetQuestX, 0, questBrandData.offSetPosGetQuestZ));
        SetPositionObjects(objectMissionObject[0], new Vector3(questBrandData.offSetPosMissionObjectX, 0, questBrandData.offSetPosMIssionObjectZ));
        SetPositionObjects(objectEndObject, new Vector3(questBrandData.offSetPosEndObjectX, 0, questBrandData.offSetPosEndObjectZ));

        SetTextDialog(objectGetQuest, this.questBrandData);
        SetTextDialog(objectEndObject, this.questBrandData);

        SetActivatorSetting(this.questBrandData);
        SetControlSetting(this.questBrandData);

        if (spawenBotsQuestBrand)
        {
            spawenBotsQuestBrand.SetSettingMissionQuiestBrand(questBrandData.ID);
            spawenBotsQuestBrand.InitSpawner();
        }
        else
        {
            SetMissionSetting(this.questBrandData);
        }

        SetBillboardBrand(questBrandData);

        SetAreaManager(questBrandData);
    }

    private void SetAreaManager(QuestBrandData questBrandData)
    {
        if (AreaManager.Instance)
        {
            for (int index = 0; index < AreaManager.Instance.targetQuestsBrand.Count; index++)
            {
                if (AreaManager.Instance.targetQuestsBrand[index].ID == questBrandData.ID) return;
            }
            TargetQuest targetQuest = new TargetQuest();
            AreaManager.Instance.targetQuestsBrand.Add(targetQuest);
            int currentIndex = AreaManager.Instance.targetQuestsBrand.IndexOf(targetQuest);

            if (objectGetQuest.TryGetComponent(out ActivatorQuestBrandBase activatorQuestBrandBase))
            {
                AreaManager.Instance.targetQuestsBrand[currentIndex].ID = activatorQuestBrandBase.questData.ID;

                AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets = new List<StepTarget>();
                for (int i = 0; i < activatorQuestBrandBase.questData.arrayStepsQuests.Length; i++)
                {
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets.Add(new StepTarget());
                }

                for (int i = 0; i < AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets.Count; i++)
                {
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[i].actionTargets = new List<ActionTarget>();
                    for (int j = 0; j < activatorQuestBrandBase.questData.arrayStepsQuests[i].actionCounts.Length; j++)
                    {
                        ActionTarget actionTarget = new ActionTarget();
                        AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[i].actionTargets.Add(actionTarget);
                        int currentIndexActionTarget = AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[i].actionTargets.IndexOf(actionTarget);
                    }
                }

                if (AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets.Count > 2)
                {
                    int indexStep = 0;
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[indexStep].actionTargets[0].actionTarget = objectMissionObject[0];
                    indexStep = 1;
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[indexStep].actionTargets[0].actionTarget = objectMissionObject[1];
                    indexStep = 2;
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[indexStep].actionTargets[0].actionTarget = objectEndObject;
                }
                else
                {
                    int indexStep = 0;
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[indexStep].actionTargets[0].actionTarget = objectMissionObject[0];
                    indexStep = 1;
                    AreaManager.Instance.targetQuestsBrand[currentIndex].stepTargets[indexStep].actionTargets[0].actionTarget = objectEndObject;
                }
            }
        }
    }

    private void SetPositionObjects(Transform objectQuesBrandt, Vector3 posObject)
    {
        objectQuesBrandt.position = objectQuesBrandt.position + posObject;
    }

    private void SetTextDialog(Transform objectQuesBrandt, QuestBrandData questBrandData)
    {
        if (objectQuesBrandt.TryGetComponent(out NPCDialogPlayer npcDialogPlayer))
        {
            npcDialogPlayer.SetNameBrand(questBrandData.nameBrand);
        }
        else
        {
            print(" Not NPCDialogPlayer");

        }
        SetNameBrandNPC(objectQuesBrandt, questBrandData);
    }

    private void SetNameBrandNPC(Transform objectQuesBrandt, QuestBrandData questBrandData)
    {
        if (objectQuesBrandt.TryGetComponent(out AbstractIO abstractIO))
        {
            abstractIO.SetName(String.Format(textNameNPC, questBrandData.nameBrand));
        }
        else
        {
            print(" Not AbstractIO");
        }
    }

    //private IEnumerator GetTextureLogoBrand(string refLogoBrand)
    //{
    //    using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(WebData.Domain + refLogoBrand))
    //    {
    //        yield return req.SendWebRequest();
    //        if (req.isNetworkError || req.isHttpError)
    //        {
    //            if (ConsoleScript.Instance) ConsoleScript.Instance.AddConsoleText(req.error, "BilboardController");
    //            req.Dispose();
    //            yield break;
    //        }
    //        textureLogoBrand = DownloadHandlerTexture.GetContent(req);
    //        req.Dispose();
    //    }

    //    for (int j = 0; j < boards.Length; j++)
    //    {
    //        boards[j].materials[0].mainTexture = textureLogoBrand;
    //    }
    //}


    private void SetActivatorSetting(QuestBrandData questBrandData)
    {
        if (objectGetQuest.TryGetComponent(out ActivatorQuestBrandBase activatorQuestBrandBase))
        {
            activatorQuestBrandBase.questData.ID = questBrandData.ID;
            activatorQuestBrandBase.questData.day = questBrandData.day;
            activatorQuestBrandBase.questData.hour = questBrandData.hour;
            activatorQuestBrandBase.questData.minute = questBrandData.min;
            activatorQuestBrandBase.questData.coins = questBrandData.countAwards;
            activatorQuestBrandBase.questData.titleQuest = System.String.Format(activatorQuestBrandBase.questData.titleQuest, questBrandData.nameBrand);
            activatorQuestBrandBase.questData.discriptionQuest = System.String.Format(activatorQuestBrandBase.questData.discriptionQuest, questBrandData.nameBrand);

            if (ConvertTextureToSprite(questBrandData.refLogoBrand))
            {
                activatorQuestBrandBase.questData.image2x1 = ConvertTextureToSprite(questBrandData.refLogoBrand);
                activatorQuestBrandBase.questData.image1x1 = ConvertTextureToSprite(questBrandData.refLogoBrand);
            }
            else
            {
                activatorQuestBrandBase.questData.image2x1 = defaultLogoBrand;
                activatorQuestBrandBase.questData.image1x1 = defaultLogoBrand;
            }
            SetQuestBrand(activatorQuestBrandBase.questData);
        }
    }


    private Sprite ConvertTextureToSprite(Texture texture)
    {
        return Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    private void SetControlSetting(QuestBrandData questBrandData)
    {
        if (objectEndObject.TryGetComponent(out ControlQuestBrandBase controlQuestBrandBase))
        {
            controlQuestBrandBase.SetQuestBrandID(questBrandData.ID);
        }
    }

    private void SetMissionSetting(QuestBrandData questBrandData)
    {
        for (int i = 0; i < objectMissionObject.Length; i++)
        {
            if (objectMissionObject[i].TryGetComponent(out MissionQuestBrandBase missionQuestBrandBase))
            {
                missionQuestBrandBase.SetQuestBrandID(questBrandData.ID);
            }
        }
    }

    private void SetBillboardBrand(QuestBrandData questBrandData)
    {
        for(int index = 0; index < boards.Length; index++)
       boards[index].materials[1].mainTexture = questBrandData.refLogoBrand;
    }

    private void SetQuestBrand(QuestData questData)
    {
        if (questBrandManager)
        {
            questBrandManager.AddQuestBrand(questData);
        }
        else
        {
            Debug.Log(" Not questBrandManager");
        }
    }

    private IEnumerator StartSetQuestBrand(QuestBrandData questBrandData)
    {
        yield return new WaitForSeconds(1.0f);
        CreateQuestBrand(questBrandData);
    }
}

[System.Serializable]
public class QuestBrandData
{
    public int ID;
    public byte typeQuest; // Type Quest 1 - transfer an item; 2 - Destroy the monster; 3 - Drive by car;
    public byte day;
    public byte hour;
    public byte min;
    public string nameBrand;
    public Texture refLogoBrand;
    public int countAwards;
    [HideInInspector]
    public byte stateQuestBrand; // State Quest Brand 1 - Возможный; 2 - Активный; 3 - Выполнен;
    public float offSetPosGetQuestX;
    public float offSetPosGetQuestZ;
    public float offSetPosMissionObjectX;
    public float offSetPosMIssionObjectZ;
    public float offSetPosEndObjectX;
    public float offSetPosEndObjectZ;
    public int count;

    public QuestBrandData(int ID, byte typeQuest, byte day, byte hour, byte min, string nameBrand, Texture refLogoBrand, int countAwards,
        byte stateQuestBrand, float offSetPosGetQuestX, float offSetPosGetQuestZ, float offSetPosDoneObjectX, float offSetPosDoneObjectZ, float offSetPosEndObjectX, float offSetPosEndObjectZ, int count)
    {
        this.ID = ID;
        this.typeQuest = typeQuest;
        this.day = day;
        this.hour = hour;
        this.min = min;
        this.nameBrand = nameBrand;
        this.refLogoBrand = refLogoBrand;
        this.countAwards = countAwards;
        this.stateQuestBrand = stateQuestBrand;
        this.offSetPosGetQuestX = offSetPosGetQuestX;
        this.offSetPosGetQuestZ = offSetPosGetQuestZ;
        this.offSetPosMissionObjectX = offSetPosDoneObjectX;
        this.offSetPosMIssionObjectZ = offSetPosDoneObjectZ;
        this.offSetPosEndObjectX = offSetPosEndObjectX;
        this.offSetPosEndObjectZ = offSetPosEndObjectZ;
        this.count = count;
    }
}




///On player
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    [HideInInspector]
    public List<QuestData> arrayQuest;
    [HideInInspector]
    public List<int> arrayDoneQuest;

    private string key = "DataQuest";
    //[SerializeField]
    private int currentLevelPlayer;
    [SerializeField]
    private MainSOQuest mainSOQuest;
    [HideInInspector]
    public List<MissionButton> missionButtons;
    [HideInInspector]
    public List<MissionButton> missionDoneButtons;
    [SerializeField]
    private Transform tagImage;
    [Header("-----------------------------------------")]

    [Header("Кнопка MakeActive ")]
    [SerializeField]
    private Button ButtonMakeActive;

    [Header("Временные миссии")]
    [SerializeField]
    private Transform missionTime;
    [SerializeField]
    private Text textTimeCount;
    [SerializeField]
    private Button prefabTimeButton;
    [Header("-----------------------------------------")]

    [Header("Актив миссии")]
    [SerializeField]
    private Transform missionActive;
    [SerializeField]
    private Text textActiveCount;
    [SerializeField]
    private Button prefabActiveButton;
    [Header("-----------------------------------------")]

    [Header("Выпол миссии")]
    [SerializeField]
    private Transform missionCompleted;
    [SerializeField]
    private Text textComletedCount;
    [SerializeField]
    private Button prefabCompletedButton;
    [Header("-----------------------------------------")]

    [Header("Описание миссии")]
    [SerializeField]
    private Transform missionDescriptionViewPort;
    [SerializeField]
    private Image imageDescription;
    [SerializeField]
    private Text titleDesxription;
    [SerializeField]
    private Text textDiscription;
    [SerializeField]
    private ShowActionCount panelSteps;
    [Header("-----------------------------------------")]

    [Header("Награды миссии")]
    [SerializeField]
    private Text coinsMission;
    [SerializeField]
    private Text expirenceMission;
    [SerializeField]
    private Text itemMission;
    [Header("-----------------------------------------")]

    [Header("Панель отображения шагов")]
    [SerializeField]
    private Text titleTime;
    [SerializeField]
    private ShowActionCount showActionCount;
    [SerializeField]
    private Transform prefabStringAction;
    [Header("-----------------------------------------")]

    [Header("Всплывающая панель Quest")]
    [SerializeField]
    private PanelPopup panelPopup;
    [Header("-----------------------------------------")]

    [Header("Уведомление актив Quest")]
    [SerializeField]
    private Text notCountQuestText;
    [Header("-----------------------------------------")]

    [Header("Target актив Quest")]
    [SerializeField]
    private TargetManager targetManager;
    [Header("-----------------------------------------")]

    [Header("Радиус спавна лута")]
    [SerializeField]
    private float radius = 1.0f;
    private Transform thisTransform;


    // текущий показанный квест
    //[SerializeField]
    private int indexCurrentShowQuest;
    // текущий выбранный квест который в данный момент был защитан в игре
    //[SerializeField]
    private int indexSelectShowQuest;
    // текущая кнопка
    private int indexCurrentButton;
    // таймер уведомления 
    private Coroutine refTimerPanelPopup;
    // таймер проверки времени квестов
    private Coroutine refCheckTimeQuest;

    //public t typeQuest;

    [Header("Particles")]
    [SerializeField] private ParticleSystem missionCreate;
    [SerializeField] private ParticleSystem missioComplite;

    private void Awake()
    {
        thisTransform = transform;
    }

    private void OnEnable()
    {
        Init();
        refCheckTimeQuest = StartCoroutine(TimerCheckTimeQuest());
    }

    private void OnDisable()
    {
        // DeleteData();
        // SaveQuestData();
        if (refCheckTimeQuest != null) StopCoroutine(refCheckTimeQuest);
    }

    private void Init()
    {
        //LoadQuestData();
        tagImage.gameObject.SetActive(false);
        ShowNotCountQuestText(0);
        ShowJournal();
        SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);
        panelPopup.gameObject.SetActive(false);
        if (ButtonMakeActive) ButtonMakeActive.onClick.AddListener(() => ClickMakeActive());

    }

    private void CreateButtonQuest()
    {
        while (missionButtons.Count < arrayQuest.Count)
        {
            CreateButton();
        }
        ShowButtonQuest();
    }

    public void CreateQuest(int ID)
    {
        if (mainSOQuest)
        {
            if (mainSOQuest.GetQuest(ID))
            {
                if (currentLevelPlayer >= mainSOQuest.GetQuest(ID).questData.levelPlayer)
                {
                    if (!CheckDoneQuest(mainSOQuest.GetQuest(ID).questData.ID))           // проверяем в законченых квестах
                    {
                        if (!CheckQuest(mainSOQuest.GetQuest(ID).questData.ID))           // проверяем  в тек квестах
                        {
                            QuestData tempQuestData = (QuestData)mainSOQuest.GetQuest(ID).questData.Clone();
                            arrayQuest.Add(tempQuestData);
                            if (missionCreate) missionCreate.Play();
                            int currentIndex = arrayQuest.IndexOf(tempQuestData);

                            if (arrayQuest[currentIndex].timerQuest)
                            {
                                SetQuestDateTime(currentIndex, GetCurrentDateTime(), arrayQuest[currentIndex].day, arrayQuest[currentIndex].hour, arrayQuest[currentIndex].minute);
                            }
                            CreateButtonQuest();
                            if (refTimerPanelPopup == null) refTimerPanelPopup = StartCoroutine(TimerPanelPopup(currentIndex, "New mission:"));
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            Debug.LogError(" Not MainSOQuest ");
        }
    }

    private void Update()
    {

        CheckButton();
    }

    private void CreateButton()
    {
        MissionButton tempButton = Instantiate(prefabActiveButton, Vector3.zero, Quaternion.identity, missionActive).GetComponent<MissionButton>();
        missionButtons.Add(tempButton);
        tempButton.GetComponent<Button>().onClick.AddListener(() => ClickButton(tempButton));
        missionButtons[missionButtons.IndexOf(tempButton)].SetTagImage(true); // вкл имаг нов задание
        indexSelectShowQuest = missionButtons.IndexOf(tempButton);
    }

    private void CreateDoneButton(int index)
    {
        MissionButton tempButton = Instantiate(prefabActiveButton, Vector3.zero, Quaternion.identity, missionCompleted).GetComponent<MissionButton>();
        tempButton.GetComponent<Button>().interactable = false;
        missionDoneButtons.Add(tempButton);
        missionDoneButtons[missionDoneButtons.IndexOf(tempButton)].SetStringAction(false, arrayQuest[index].image1x1, arrayQuest[index].titleQuest);
    }

    private void ClickButton(MissionButton tempButton)
    {
        missionButtons[missionButtons.IndexOf(tempButton)].SetTagImage(false);
        //ShowButtonQuest();
        ShowTagImage();  // вкл имаг нового задание в журнале
        ShowPanelDiscription(missionButtons.IndexOf(tempButton));
    }

    public void ClickMakeActive()
    {
        // Debug.Log(" Make Active ");
        missionButtons[indexCurrentShowQuest].SetStringAction(false, arrayQuest[indexCurrentShowQuest].image1x1, arrayQuest[indexCurrentShowQuest].titleQuest);
        indexCurrentShowQuest = indexCurrentButton;
        missionButtons[indexCurrentButton].SetStringAction(true, arrayQuest[indexCurrentButton].image1x1, arrayQuest[indexCurrentButton].titleQuest);
        SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);
    }

    private void ShowPanelPopup(int index, string textMission)
    {
        panelPopup.gameObject.SetActive(true);
        panelPopup.SetValuePanelPopup(arrayQuest[index].image1x1, textMission, arrayQuest[index].titleQuest);
    }

    private void ShowPanelDiscription(int index)
    {
        indexCurrentButton = index;
        imageDescription.sprite = arrayQuest[index].image2x1;
        titleDesxription.text = arrayQuest[index].titleQuest;
        textDiscription.text = arrayQuest[index].discriptionQuest;
        SetShowMessegeQuest(panelSteps, index, index);
        coinsMission.text = arrayQuest[index].coins.ToString();
        expirenceMission.text = arrayQuest[index].experience.ToString();
    }

    private void SetShowMessegeQuest(ShowActionCount showActionCount, int indexCurrentShowQuest, int indexSelectShowQuest)
    {
        int indexCount = 0;
        if (arrayQuest.Count > indexCurrentShowQuest)
        {
            bool offCount;
            titleTime.gameObject.SetActive(arrayQuest[indexCurrentShowQuest].timerQuest);

            for (; indexCount < arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts.Length; indexCount++)
            {
                bool checkBox = arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].currentCount >=
                arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount;

                string textCount = "";
                if (arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount > 1)
                {
                    offCount = true;

                    int currentCount = arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].currentCount;
                    textCount = string.Format("{0:d2}", currentCount) + "/";

                    currentCount = arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount;
                    textCount += string.Format("{0:d2}", currentCount);
                }
                else
                {
                    offCount = false;
                }

                if (indexCurrentShowQuest == indexSelectShowQuest)
                {
                    showActionCount.ShowCurrentQuest(indexCount, checkBox, arrayQuest[indexCurrentShowQuest].arrayStepsQuests[arrayQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].textAction, textCount, offCount);
                }

                if (!checkBox)
                {
                    targetManager.ShowTargetQuest(indexCount, arrayQuest[indexCurrentShowQuest].ID, arrayQuest[indexCurrentShowQuest].currentStepNumber, indexCount, TargetManager.TypeTarget.basic); // показываем
                }
                else
                {
                    targetManager.arrayTargetMiniMaps[indexCount].gameObject.SetActive(false);      // выкл 
                    targetManager.arrayTargetMiniMaps[indexCount].SetActiveImageArea(false);
                }
            }
        }
        else
        {
            titleTime.gameObject.SetActive(false);
        }

        for (; indexCount < showActionCount.arrayStringAction.Length; indexCount++)
        {
            showActionCount.arrayStringAction[indexCount].gameObject.SetActive(false);  // выкл остальные
            targetManager.arrayTargetMiniMaps[indexCount].gameObject.SetActive(false);   // выкл остальные
            targetManager.arrayTargetMiniMaps[indexCount].SetActiveImageArea(false);
        }
    }

    private void ShowNotCountQuestText(int countQuest)
    {
        notCountQuestText.text = "";
        if (countQuest > 0)
        {
            notCountQuestText.text = string.Format("{0:d2}", countQuest);                 //+= countQuest.ToString();
        }
    }

    public void DeleteQuest(int ID)
    {
        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (ID == arrayQuest[index].ID)
            {
                arrayDoneQuest.Add(arrayQuest[index].ID);
                CreateDoneButton(index);
                if (refTimerPanelPopup == null) refTimerPanelPopup = StartCoroutine(TimerPanelPopup(index, "Mission completed"));
                missionButtons[index].SetTagImage(false);
                arrayQuest.RemoveAt(index);
                if (missioComplite) missioComplite.Play();
                indexCurrentShowQuest = 0;
                indexSelectShowQuest = index;
                SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);
                ShowButtonQuest();
                return;
            }

        }
    }

    public void CheckStep(int ID, int stepQuest, int numberAction)
    {
        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (arrayQuest[index].ID == ID)
            {
                if (arrayQuest[index].currentStepNumber == stepQuest)
                {
                    if (arrayQuest[index].arrayStepsQuests[arrayQuest[index].currentStepNumber].actionCounts[numberAction].currentCount <
                        arrayQuest[index].arrayStepsQuests[arrayQuest[index].currentStepNumber].actionCounts[numberAction].maxCount)
                    {
                        arrayQuest[index].arrayStepsQuests[arrayQuest[index].currentStepNumber].actionCounts[numberAction].currentCount++;
                    }
                    if (CheckDoneActionCounts(index))
                    {
                        if (arrayQuest[index].currentStepNumber == arrayQuest[index].arrayStepsQuests[arrayQuest[index].currentStepNumber].nextStepNumber)
                        {
                            DeleteQuest(ID);
                        }
                        else
                        {
                            arrayQuest[index].currentStepNumber = arrayQuest[index].arrayStepsQuests[arrayQuest[index].currentStepNumber].nextStepNumber;
                        }
                    }
                    indexSelectShowQuest = index;
                    ShowButtonQuest();
                    ShowTagImage();
                }
            }
        }
    }

    private bool CheckDoneActionCounts(int indexArrayQuest)
    {
        for (int indexActionCount = 0; indexActionCount < arrayQuest[indexArrayQuest].arrayStepsQuests[arrayQuest[indexArrayQuest].currentStepNumber].actionCounts.Length; indexActionCount++)
        {
            if (!(arrayQuest[indexArrayQuest].arrayStepsQuests[arrayQuest[indexArrayQuest].currentStepNumber].actionCounts[indexActionCount].currentCount >=
                arrayQuest[indexArrayQuest].arrayStepsQuests[arrayQuest[indexArrayQuest].currentStepNumber].actionCounts[indexActionCount].maxCount))
            {
                return false;
            }
        }
        return true;
    }

    private void ShowButtonDoneQuest()
    {
        for (int index = 0; index < missionDoneButtons.Count; index++)
        {
            missionDoneButtons[index].gameObject.SetActive(false);
        }

        if (arrayDoneQuest.Count > 0)
        {
            for (int index = 0; index < arrayDoneQuest.Count; index++)
            {
                missionDoneButtons[index].gameObject.SetActive(true);
            }
        }
    }

    private void ShowButtonQuest()
    {
        for (int index = 0; index < missionButtons.Count; index++)
        {
            missionButtons[index].gameObject.SetActive(false);
        }
        textActiveCount.text = arrayQuest.Count.ToString();
        ShowNotCountQuestText(arrayQuest.Count);
        if (arrayQuest.Count > 0)
        {
            for (int index = 0; index < arrayQuest.Count; index++)
            {
                missionButtons[index].gameObject.SetActive(true);
                bool activeButton = indexCurrentButton == index;
                missionButtons[index].SetStringAction(activeButton, arrayQuest[index].image1x1, arrayQuest[index].titleQuest);
            }
            missionDescriptionViewPort.gameObject.SetActive(true);
            ShowPanelDiscription(indexCurrentShowQuest);
            SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexSelectShowQuest);
        }
        else
        {
            missionDescriptionViewPort.gameObject.SetActive(false);
        }
        textComletedCount.text = arrayDoneQuest.Count.ToString();

        ShowTagImage();
        ShowButtonDoneQuest();
    }

    private void ShowTagImage()
    {
        bool isActiveTagImage = false;
        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (!isActiveTagImage) isActiveTagImage = missionButtons[index].GetTagImage();
        }
        tagImage.gameObject.SetActive(isActiveTagImage);  // вкл имаг нового задание в журнале
    }

    private void ShowJournal()
    {
        CreateButtonQuest();
    }

    private void CheckButton()
    {

        if (Input.GetKeyDown(KeyCode.L))
        {
            // LoadQuestData2();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //CreateQuest(arrayQuest[0]);
            //CreateButtonQuest();
            //ShowButtonQuest();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            //SaveQuestData();
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            //SetShowQuest();
            //Debug.Log(arrayQuest[0].statusQuest);
            //arrayQuest[0].statusQuest = StatusQuest.taken;
            //Debug.Log(arrayQuest[0].statusQuest);
        }
    }

    public bool CheckDoneQuest(int ID)
    {
        for (int index = 0; index < arrayDoneQuest.Count; index++)
        {
            if (arrayDoneQuest[index] == ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckQuest(int ID)
    {
        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (arrayQuest[index].ID == ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckQuest(int ID, out int currentStepNumber)
    {
        currentStepNumber = 0;

        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (arrayQuest[index].ID == ID)
            {
                currentStepNumber = arrayQuest[index].currentStepNumber;
                return true;
            }
        }
        return false;
    }

    private void CheckTimeQuests()
    {
        DateTime currentDateTime = GetCurrentDateTime();

        for (int index = 0; index < arrayQuest.Count; index++)
        {
            if (arrayQuest[index].timerQuest)
            {
                if (arrayQuest[index].questDateTime > currentDateTime)
                {
                    var deltaDateTime = arrayQuest[index].questDateTime - currentDateTime;
                    string textDeltaTime = $"{string.Format("{0:d2}", deltaDateTime.Days)}day {string.Format("{0:d2}", deltaDateTime.Hours)}hour  {string.Format("{0:d2}", deltaDateTime.Minutes)}min";
                    missionButtons[index].SetMissionTimer(true, textDeltaTime);
                    titleTime.text = $"Mission ends in: {textDeltaTime}";
                }
                else
                {
                    DeleteQuest(arrayQuest[index].ID);
                }
            }
            else
            {
                missionButtons[index].SetMissionTimer(false, "");
            }
        }
    }

    private void SetQuestDateTime(int index, DateTime dateTimeNow, int day, int hour, int minute)
    {
        day = day < 1 ? 1 : day;
        arrayQuest[index].questDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, day, hour, minute, dateTimeNow.Second);
    }

    private DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }

    private void DeleteData()
    {
        PlayerPrefs.DeleteKey(key);
    }

    //private void LootDropQuest(int index)
    //{
    //    print("Loot Droping");

    //private int experience;
    //private int coins;
    //private ItemBaseParametrs[] itemBase;

    ////ex arrayQuest[index]


    //}

    [HideInInspector]
    public byte[] arr;

    private void LoadQuestData()
    {
        if (PlayerPrefs.HasKey(key))
        {
            DataSaveQuest dataSaveQuest = new DataSaveQuest();
            string value = PlayerPrefs.GetString(key);
            dataSaveQuest = JsonUtility.FromJson<DataSaveQuest>(value);
            Debug.Log(" Load data Quest");

            // load basic data Quest
            for (int indexQuestData = 0; indexQuestData < dataSaveQuest.dataQuest.Length; indexQuestData++)
            {
                arrayQuest.Add((QuestData)mainSOQuest.GetQuest(dataSaveQuest.dataQuest[indexQuestData].ID).questData.Clone());
                arrayQuest[indexQuestData].ID = dataSaveQuest.dataQuest[indexQuestData].ID;
                arrayQuest[indexQuestData].currentStepNumber = dataSaveQuest.dataQuest[indexQuestData].currentStepNumber;

                for (int indexStepsQuests = 0; indexStepsQuests < dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests.Length; indexStepsQuests++)
                {
                    arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].nextStepNumber =
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].stepNumber;

                    for (int indexActionCount = 0; indexActionCount < arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length; indexActionCount++)
                    {
                        arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount =
                        dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount;
                    }
                }
            }


            // load done quest
            for (int indexDoneQuest = 0; indexDoneQuest < dataSaveQuest.dataDoneQuest.Length; indexDoneQuest++)
            {
                arrayDoneQuest.Add(dataSaveQuest.dataDoneQuest[indexDoneQuest]);
            }

        }
    }

    private void SaveQuestData()
    {
        // save current Basic Quest
        DataSaveQuest dataSaveQuest = new DataSaveQuest();
        dataSaveQuest.dataQuest = new DataQuest[arrayQuest.Count];
        for (int indexQuestData = 0; indexQuestData < arrayQuest.Count; indexQuestData++)
        {
            dataSaveQuest.dataQuest[indexQuestData] = new DataQuest();
            dataSaveQuest.dataQuest[indexQuestData].ID = arrayQuest[indexQuestData].ID;
            dataSaveQuest.dataQuest[indexQuestData].currentStepNumber = arrayQuest[indexQuestData].currentStepNumber;

            dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests = new StepsSaveQuest[arrayQuest[indexQuestData].arrayStepsQuests.Length];
            for (int indexStepsQuests = 0; indexStepsQuests < arrayQuest[indexQuestData].arrayStepsQuests.Length; indexStepsQuests++)
            {
                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests] = new StepsSaveQuest();
                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].stepNumber =
                    arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].nextStepNumber;

                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts =
                    new ActionSaveCount[arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length];
                for (int indexActionCount = 0; indexActionCount < arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length; indexActionCount++)
                {
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount] = new ActionSaveCount();
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount =
                        arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount;
                }
            }
        }

        // save done quest
        dataSaveQuest.dataDoneQuest = new int[arrayDoneQuest.Count];
        for (int indexDoneQuest = 0; indexDoneQuest < arrayDoneQuest.Count; indexDoneQuest++)
        {
            dataSaveQuest.dataDoneQuest[indexDoneQuest] = new int();
            dataSaveQuest.dataDoneQuest[indexDoneQuest] = arrayDoneQuest[indexDoneQuest];
        }

        string value = JsonUtility.ToJson(dataSaveQuest);
        PlayerPrefs.SetString(key, value);
        PlayerPrefs.Save();
        Debug.Log(" Save data Quest");
    }

    private void LoadQuestDataServer()
    {
        ////dataSaveQuest2 = SupportClassSaveData.StructureAllTimeDataReturn(arr);


        //// load basic data Quest
        //for (int indexQuestData = 0; indexQuestData < dataSaveQuest2.dataQuest.Length; indexQuestData++)
        //{
        //    arrayQuest.Add((QuestData)mainSOQuest.GetQuest(dataSaveQuest2.dataQuest[indexQuestData].ID).questData.Clone());
        //    arrayQuest[indexQuestData].ID = dataSaveQuest2.dataQuest[indexQuestData].ID;
        //    arrayQuest[indexQuestData].currentStepNumber = dataSaveQuest2.dataQuest[indexQuestData].currentStepNumber;

        //    for (int indexStepsQuests = 0; indexStepsQuests < dataSaveQuest2.dataQuest[indexQuestData].arrayStepsQuests.Length; indexStepsQuests++)
        //    {
        //        arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].nextStepNumber =
        //        dataSaveQuest2.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].stepNumber;

        //        for (int indexActionCount = 0; indexActionCount < arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length; indexActionCount++)
        //        {
        //            arrayQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount =
        //            dataSaveQuest2.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount;
        //        }
        //    }
        //}


        //for (int indexDoneQuest = 0; indexDoneQuest < dataSaveQuest2.dataDoneQuest.Length; indexDoneQuest++)
        //{
        //    arrayDoneQuest.Add(dataSaveQuest2.dataDoneQuest[indexDoneQuest]);
        //}
    }

    public void ReceiveClientDataQuest(int _questID, int _stepQuest, int _numberAction, int _currentCount)
    {

    }

    public void SendClientDataQuest(int _questID, int _stepQuest, int _numberAction, int _currentCount)
    {
        ushort _proto = 0xABCD;
        ushort _version = 0x0000;
        SupportClassSaveData.PacketQuestData packetQuestData = new SupportClassSaveData.PacketQuestData(_proto, _version, _questID, _stepQuest, _numberAction, _currentCount);
        arr = SupportClassSaveData.StructureToByteArray(packetQuestData);
        ReceiveServerDataQuest(arr);
    }

    public void ReceiveServerDataQuest(byte[] arr)
    {
        SupportClassSaveData.PacketQuestData packetQuestData = new SupportClassSaveData.PacketQuestData();
        packetQuestData = SupportClassSaveData.StructureAllTimeDataReturn(arr);
    }

    private IEnumerator TimerPanelPopup(int index, string textMission)
    {
        ShowPanelPopup(index, textMission);
        yield return new WaitForSeconds(6.0f);
        panelPopup.gameObject.SetActive(false);
        refTimerPanelPopup = null;
    }

    private IEnumerator TimerCheckTimeQuest()
    {
        while (true)
        {
            WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);
            yield return waitForSeconds;
            CheckTimeQuests();
        }
    }
}


#region DataSaveQuest

[System.Serializable]
public struct DataQuest
{
    public int ID;
    public int currentStepNumber;
    public StepsSaveQuest[] arrayStepsQuests;
}

[System.Serializable]
public struct StepsSaveQuest
{
    public int stepNumber;
    public ActionSaveCount[] actionCounts;
}

[System.Serializable]
public struct ActionSaveCount
{
    public int currentCount;
}
#endregion

[System.Serializable]
public struct DataSaveQuest
{
    public DataQuest[] dataQuest;
    public int[] dataDoneQuest;

}

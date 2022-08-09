///On player
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class QuestBrandManager : MonoBehaviour
{
    [HideInInspector]
    public List<QuestData> arrayAllQuest;
    [HideInInspector]
    public List<QuestData> arrayActiveQuest;
    [HideInInspector]
    public List<int> arrayDoneQuest;

    private string key = "DataQuest";
    private int currentLevelPlayer;
    [HideInInspector]
    public List<MissionButton> missionAllButtons;
    [HideInInspector]
    public List<MissionButton> missionButtons;
    [HideInInspector]
    public List<MissionButton> missionDoneButtons;
    [SerializeField]
    private Transform tagImage;
    [Header("-----------------------------------------")]

    [Header(" нопка MakeActive ")]
    [SerializeField]
    private Button buttonMakeActive;

    [Header(" нопка ButtonAcceptMissions ")]
    [SerializeField]
    private Button buttonAcceptMissions;

    [Header("¬се миссии")]
    [SerializeField]
    private Transform missionAll;
    [SerializeField]
    private Text textAllCount;
    [SerializeField]
    private Button prefabAllButton;
    [Header("-----------------------------------------")]

    [Header("јктив миссии")]
    [SerializeField]
    private Transform missionActive;
    [SerializeField]
    private Text textActiveCount;
    [SerializeField]
    private Button prefabActiveButton;
    [Header("-----------------------------------------")]

    [Header("¬ыпол миссии")]
    [SerializeField]
    private Transform missionCompleted;
    [SerializeField]
    private Text textComletedCount;
    [SerializeField]
    private Button prefabCompletedButton;
    [Header("-----------------------------------------")]

    [Header("ќписание миссии")]
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

    [Header("Ќаграды миссии")]
    [SerializeField]
    private Text coinsMission;
    [SerializeField]
    private Text expirenceMission;
    [SerializeField]
    private Text itemMission;
    [Header("-----------------------------------------")]

    [Header("ѕанель отображени€ шагов")]
    [SerializeField]
    private Text titleTime;
    [SerializeField]
    private ShowActionCount showActionCount;
    [SerializeField]
    private Transform prefabStringAction;
    [Header("-----------------------------------------")]

    [Header("¬сплывающа€ панель Quest")]
    [SerializeField]
    private PanelPopup panelPopup;
    [Header("-----------------------------------------")]

    [Header("”ведомление актив Quest")]
    [SerializeField]
    private Text notCountQuestText;
    [Header("-----------------------------------------")]

    [Header("Target актив Quest")]
    [SerializeField]
    private TargetManager targetManager;
    [Header("-----------------------------------------")]

    [Header("–адиус спавна лута")]
    [SerializeField]
    private float radius = 1.0f;
    private Transform thisTransform;
    // текущий показанный квест All
    private int indexCurrentShowAllQuest;
    // текуща€ кнопка All
    private int indexCurrentAllButton;
    // текущий показанный квест
    private int indexCurrentShowQuest;
    // текущий выбранный квест который в данный момент был защитан в игре
    private int indexSelectShowQuest;
    // текуща€ кнопка
    private int indexCurrentButton;
    // таймер уведомлени€ 
    private Coroutine refTimerPanelPopup;
    // таймер проверки времени квестов
    private Coroutine refCheckTimeQuest;


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
        if (buttonMakeActive) buttonMakeActive.onClick.AddListener(() => ClickMakeActive());
        if (buttonAcceptMissions) buttonAcceptMissions.onClick.AddListener(() => ClickAcceptMissions());
    }

    private void CreateButtonAllQuest()
    {
        while (missionAllButtons.Count < arrayAllQuest.Count)
        {
            CreateAllButton();
        }
        ShowAllButtonQuest();
    }

    private void CreateButtonQuest()
    {
        while (missionButtons.Count < arrayActiveQuest.Count)
        {
            CreateButton();
        }
        ShowButtonQuest();
    }

    public void AddQuestBrand(QuestData questData)
    {
        for (int i = 0; i < arrayAllQuest.Count; i++)
        {
            if (questData.ID == arrayAllQuest[i].ID) return;
        }

        if (currentLevelPlayer >= questData.levelPlayer)
        {
            if (!CheckDoneQuest(questData.ID))           // провер€ем в законченых квестах
            {
                if (!CheckQuest(questData.ID))           // провер€ем  в тек квестах
                {
                    QuestData tempQuestData = (QuestData)questData.Clone();
                    arrayAllQuest.Add(tempQuestData);
                    int currentIndex = arrayAllQuest.IndexOf(tempQuestData);

                    if (arrayAllQuest[currentIndex].timerQuest)
                    {
                        SetQuestDateTime(currentIndex, GetCurrentDateTime(), arrayAllQuest[currentIndex].day, arrayAllQuest[currentIndex].hour, arrayAllQuest[currentIndex].minute);
                    }

                    CreateButtonAllQuest();
                }
            }
        }
        else
        {
            return;
        }
    }

    public void SetActiveQuestBrand(int ID)
    {
        if (CheckDoneQuest(ID)) return;          // провер€ем в законченых квестах
        if (CheckQuest(ID)) return;     // провер€ем  в тек квестах

        for (int i = 0; i < arrayAllQuest.Count; i++)
        {
            if (arrayAllQuest[i].ID == ID)
            {
                AddActiveQuest(i);
            }
        }

    }

    public void AddActiveQuest(int indexCurrentShowQuest)
    {
        arrayActiveQuest.Add(arrayAllQuest[indexCurrentShowQuest]);
        if (missionCreate) missionCreate.Play();
        int currentIndex = arrayActiveQuest.IndexOf(arrayAllQuest[indexCurrentShowQuest]);
        if (arrayActiveQuest[currentIndex].timerQuest)
        {
            SetQuestDateTime(currentIndex, GetCurrentDateTime(), arrayActiveQuest[currentIndex].day, arrayActiveQuest[currentIndex].hour, arrayActiveQuest[currentIndex].minute);
        }

        CreateButtonQuest();
        arrayAllQuest.RemoveAt(indexCurrentShowQuest);
        ShowAllButtonQuest();
        ShowButtonQuest();
    }

    private void Update()
    {
        CheckButton();
    }

    private void CreateAllButton()
    {
        MissionButton tempButton = Instantiate(prefabAllButton, Vector3.zero, Quaternion.identity, missionAll).GetComponent<MissionButton>();
        missionAllButtons.Add(tempButton);
        tempButton.GetComponent<Button>().onClick.AddListener(() => ClickAllButton(tempButton));
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
        missionDoneButtons[missionDoneButtons.IndexOf(tempButton)].SetStringAction(false, arrayActiveQuest[index].image1x1, arrayActiveQuest[index].titleQuest);
    }

    private void ClickAllButton(MissionButton tempButton)
    {
        buttonAcceptMissions.gameObject.SetActive(true);
        buttonMakeActive.gameObject.SetActive(false);

        missionAllButtons[missionAllButtons.IndexOf(tempButton)].SetTagImage(false);
        ShowButtonQuest();
        ShowPanelAllDiscription(missionAllButtons.IndexOf(tempButton));
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)missionAll.transform);
    }


    private void ClickButton(MissionButton tempButton)
    {
        buttonMakeActive.gameObject.SetActive(true);
        buttonAcceptMissions.gameObject.SetActive(false);
        missionButtons[missionButtons.IndexOf(tempButton)].SetTagImage(false);
        //ShowButtonQuest();
        ShowTagImage();  // вкл имаг нового задание в журнале
        ShowPanelDiscription(missionButtons.IndexOf(tempButton));
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)missionActive.transform);
    }

    public void ClickAcceptMissions()
    {
        //Debug.Log(" Make Active ");
        //missionAllButtons[indexCurrentShowAllQuest].SetStringAction(false, arrayAllQuest[indexCurrentShowAllQuest].image1x1, arrayAllQuest[indexCurrentShowAllQuest].titleQuest);
        indexCurrentShowAllQuest = indexCurrentAllButton;
        missionAllButtons[indexCurrentAllButton].SetStringAction(true, arrayAllQuest[indexCurrentAllButton].image1x1, arrayAllQuest[indexCurrentAllButton].titleQuest);
        AddActiveQuest(indexCurrentShowAllQuest);
        indexCurrentAllButton = 0;
        SetShowMessegeAllQuest(showActionCount, indexCurrentShowAllQuest);
        //ShowButtonDoneQuest();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)missionActive.transform);
    }

    public void ClickMakeActive()
    {
        missionButtons[indexCurrentShowQuest].SetStringAction(false, arrayActiveQuest[indexCurrentShowQuest].image1x1, arrayActiveQuest[indexCurrentShowQuest].titleQuest);
        indexCurrentShowQuest = indexCurrentButton;
        missionButtons[indexCurrentButton].SetStringAction(true, arrayActiveQuest[indexCurrentButton].image1x1, arrayActiveQuest[indexCurrentButton].titleQuest);
        SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);
    }

    private void ShowPanelPopup(int index, string textMission, List<QuestData> currentArray)
    {
        panelPopup.gameObject.SetActive(true);
        panelPopup.SetValuePanelPopup(currentArray[index].image1x1, textMission, currentArray[index].titleQuest);
    }

    private void ShowPanelAllDiscription(int index)
    {
        missionDescriptionViewPort.gameObject.SetActive(false);

        if (arrayAllQuest.Count > 0)
        {
            missionDescriptionViewPort.gameObject.SetActive(true);
        }
        else
        {
            missionDescriptionViewPort.gameObject.SetActive(false);
        }

        indexCurrentAllButton = index;
        imageDescription.sprite = arrayAllQuest[index].image2x1;
        titleDesxription.text = arrayAllQuest[index].titleQuest;
        textDiscription.text = arrayAllQuest[index].discriptionQuest;
        SetShowMessegeAllQuest(panelSteps, index);
        coinsMission.text = arrayAllQuest[index].coins.ToString();
        expirenceMission.text = arrayAllQuest[index].experience.ToString();
    }

    private void SetShowMessegeAllQuest(ShowActionCount showActionCount, int indexCurrentShowQuest)
    {
        int indexCount = 0;
        //if (arrayAllQuest.Count > indexCurrentShowQuest)
        //{
            
        //}
        //else
        //{
        //}

        for (; indexCount < showActionCount.arrayStringAction.Length; indexCount++)
        {
            showActionCount.arrayStringAction[indexCount].gameObject.SetActive(false);  // выкл остальные
        }
    }

    private void ShowPanelDiscription(int index)
    {
        if (arrayActiveQuest.Count > 0)
        {
            missionDescriptionViewPort.gameObject.SetActive(true);
        }
        else
        {
            missionDescriptionViewPort.gameObject.SetActive(false);
        }

        if (index > arrayActiveQuest.Count || arrayActiveQuest.Count == 0) return;
        indexCurrentButton = index;
        imageDescription.sprite = arrayActiveQuest[index].image2x1;
        titleDesxription.text = arrayActiveQuest[index].titleQuest;
        textDiscription.text = arrayActiveQuest[index].discriptionQuest;
        SetShowMessegeQuest(panelSteps, index, index);
        coinsMission.text = arrayActiveQuest[index].coins.ToString();
        expirenceMission.text = arrayActiveQuest[index].experience.ToString();
    }

    private void SetShowMessegeQuest(ShowActionCount showActionCount, int indexCurrentShowQuest, int indexSelectShowQuest)
    {
        int indexCount = 0;
        if (arrayActiveQuest.Count > indexCurrentShowQuest)
        {
            bool offCount;
            titleTime.gameObject.SetActive(arrayActiveQuest[indexCurrentShowQuest].timerQuest);

            for (; indexCount < arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts.Length; indexCount++)
            {
                bool checkBox = arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].currentCount >=
                arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount;

                string textCount = "";
                if (arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount > 1)
                {
                    offCount = true;

                    int currentCount = arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].currentCount;
                    textCount = string.Format("{0:d2}", currentCount) + "/";

                    currentCount = arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].maxCount;
                    textCount += string.Format("{0:d2}", currentCount);
                }
                else
                {
                    offCount = false;
                }


                if (indexCurrentShowQuest == indexSelectShowQuest)
                {
                    showActionCount.ShowCurrentQuest(indexCount, checkBox, arrayActiveQuest[indexCurrentShowQuest].arrayStepsQuests[arrayActiveQuest[indexCurrentShowQuest].currentStepNumber].actionCounts[indexCount].textAction, textCount, offCount);
                }

                if (!checkBox)
                {
                    targetManager.ShowTargetQuest(indexCount, arrayActiveQuest[indexCurrentShowQuest].ID, arrayActiveQuest[indexCurrentShowQuest].currentStepNumber, indexCount, TargetManager.TypeTarget.brand); // показываем
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
        for (int index = 0; index < arrayActiveQuest.Count; index++)
        {
            if (ID == arrayActiveQuest[index].ID)
            {
                arrayDoneQuest.Add(arrayActiveQuest[index].ID);
                CreateDoneButton(index);
                if (refTimerPanelPopup == null) refTimerPanelPopup = StartCoroutine(TimerPanelPopup(index, "Mission completed", arrayActiveQuest));
                missionButtons[index].SetTagImage(false);
                arrayActiveQuest.RemoveAt(index);
                if (missioComplite) missioComplite.Play();
                indexCurrentShowQuest = 0;
                indexCurrentShowAllQuest = 0;
                indexSelectShowQuest = index;
                SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);
                ShowButtonQuest();
                SetShowMessegeAllQuest(showActionCount, indexCurrentShowAllQuest);
                ShowAllButtonQuest();
                return;
            }

        }
    }

    public void DisableTargetMinimaps()
    {
        for (int i = 0; i < targetManager.arrayTargetMiniMaps.Length; i++)
        {
            targetManager.arrayTargetMiniMaps[i].gameObject.SetActive(false);   // выкл остальные
        }
    }

    public void CheckStep(int ID, int stepQuest, int numberAction)
    {
        for (int index = 0; index < arrayActiveQuest.Count; index++)
        {
            if (arrayActiveQuest[index].ID == ID)
            {
                if (arrayActiveQuest[index].currentStepNumber == stepQuest)
                {
                    if (arrayActiveQuest[index].arrayStepsQuests[arrayActiveQuest[index].currentStepNumber].actionCounts[numberAction].currentCount <
                        arrayActiveQuest[index].arrayStepsQuests[arrayActiveQuest[index].currentStepNumber].actionCounts[numberAction].maxCount)
                    {
                        arrayActiveQuest[index].arrayStepsQuests[arrayActiveQuest[index].currentStepNumber].actionCounts[numberAction].currentCount++;
                    }
                    if (CheckDoneActionCounts(index))
                    {
                        if (arrayActiveQuest[index].currentStepNumber == arrayActiveQuest[index].arrayStepsQuests[arrayActiveQuest[index].currentStepNumber].nextStepNumber)
                        {
                            DeleteQuest(ID);
                        }
                        else
                        {
                            arrayActiveQuest[index].currentStepNumber = arrayActiveQuest[index].arrayStepsQuests[arrayActiveQuest[index].currentStepNumber].nextStepNumber;
                        }
                    }
                    indexSelectShowQuest = index;
                    ShowButtonQuest();
                    ShowTagImage();
                    return;             /////////////////////////////////////////////
                }
            }
        }
    }

    private bool CheckDoneActionCounts(int indexArrayQuest)
    {
        for (int indexActionCount = 0; indexActionCount < arrayActiveQuest[indexArrayQuest].arrayStepsQuests[arrayActiveQuest[indexArrayQuest].currentStepNumber].actionCounts.Length; indexActionCount++)
        {
            if (!(arrayActiveQuest[indexArrayQuest].arrayStepsQuests[arrayActiveQuest[indexArrayQuest].currentStepNumber].actionCounts[indexActionCount].currentCount >=
                arrayActiveQuest[indexArrayQuest].arrayStepsQuests[arrayActiveQuest[indexArrayQuest].currentStepNumber].actionCounts[indexActionCount].maxCount))
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

    private void ShowAllButtonQuest()
    {
        for (int index = 0; index < missionAllButtons.Count; index++)
        {
            missionAllButtons[index].gameObject.SetActive(false);
        }
        textAllCount.text = arrayAllQuest.Count.ToString();
        //ShowNotCountQuestText(arrayAllQuest.Count);
        if (arrayAllQuest.Count > 0)
        {
            for (int index = 0; index < arrayAllQuest.Count; index++)
            {
                missionAllButtons[index].gameObject.SetActive(true);
                bool activeButton = indexCurrentAllButton == index;
                missionAllButtons[index].SetStringAction(false, arrayAllQuest[index].image1x1, arrayAllQuest[index].titleQuest); ///////////////////////////
            }
            //missionDescriptionViewPort.gameObject.SetActive(true);          ////////////////////////////////////////////////////////
            //ShowPanelAllDiscription(indexCurrentShowAllQuest);
            // SetShowMessegeQuest(showActionCount, indexCurrentShowQuest);
        }
        else
        {
            missionDescriptionViewPort.gameObject.SetActive(false);
            buttonAcceptMissions.gameObject.SetActive(false);
            buttonMakeActive.gameObject.SetActive(true);

        }
        textComletedCount.text = arrayDoneQuest.Count.ToString();

        //ShowTagImage();
        ShowButtonDoneQuest();
    }

    private void ShowButtonQuest()
    {
        for (int index = 0; index < missionButtons.Count; index++)
        {
            missionButtons[index].gameObject.SetActive(false);
        }
        textActiveCount.text = arrayActiveQuest.Count.ToString();
        ShowNotCountQuestText(arrayActiveQuest.Count);
        if (arrayActiveQuest.Count > 0)
        {
            for (int index = 0; index < arrayActiveQuest.Count; index++)
            {
                missionButtons[index].gameObject.SetActive(true);
                bool activeButton = indexCurrentButton == index;
                missionButtons[index].SetStringAction(activeButton, arrayActiveQuest[index].image1x1, arrayActiveQuest[index].titleQuest);
            }
            missionDescriptionViewPort.gameObject.SetActive(true);
            ShowPanelDiscription(indexCurrentShowQuest);
            SetShowMessegeQuest(showActionCount, indexCurrentShowQuest, indexCurrentShowQuest);

        }
        else
        {
            missionDescriptionViewPort.gameObject.SetActive(false);
        }
        textComletedCount.text = arrayDoneQuest.Count.ToString();

       // ShowTagImage();
        ShowButtonDoneQuest();
    }

    private void ShowTagImage()
    {
        bool isActiveTagImage = false;
        for (int index = 0; index < arrayActiveQuest.Count; index++)
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

        if (Input.GetKeyDown(KeyCode.X))
        {
            DeleteQuest(203);
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
        for (int index = 0; index < arrayActiveQuest.Count; index++)
        {
            if (arrayActiveQuest[index].ID == ID)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckQuest(int ID, out int currentStepNumber)
    {
        currentStepNumber = 0;

        for (int index = 0; index < arrayActiveQuest.Count; index++)
        {
            if (arrayActiveQuest[index].ID == ID)
            {
                currentStepNumber = arrayActiveQuest[index].currentStepNumber;
                return true;
            }
        }
        return false;
    }

    private void CheckTimeQuests()
    {
        if (arrayActiveQuest.Count < 1) return;
        DateTime currentDateTime = GetCurrentDateTime();

        for (int index = 0; index < arrayActiveQuest.Count; index++)
        {
            if (arrayActiveQuest[index].timerQuest)
            {
                if (arrayActiveQuest[index].questDateTime > currentDateTime)
                {
                    var deltaDateTime = arrayActiveQuest[index].questDateTime - currentDateTime;
                    string textDeltaTime = $"{string.Format("{0:d2}", deltaDateTime.Days)}day {string.Format("{0:d2}", deltaDateTime.Hours)}hour  {string.Format("{0:d2}", deltaDateTime.Minutes)}min";
                    missionButtons[index].SetMissionTimer(true, textDeltaTime);
                    titleTime.text = $"Mission ends in: {textDeltaTime}";
                }
                else
                {
                    DeleteQuest(arrayActiveQuest[index].ID);
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
        arrayActiveQuest[index].questDateTime = new DateTime(dateTimeNow.Year, dateTimeNow.Month, day, hour, minute, dateTimeNow.Second);
    }

    private DateTime GetCurrentDateTime()
    {
        return DateTime.Now;
    }

    private void DeleteData()
    {
        PlayerPrefs.DeleteKey(key);
    }

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
                // arrayActiveQuest.Add((QuestData)mainSOQuest.GetQuest(dataSaveQuest.dataQuest[indexQuestData].ID).questData.Clone());
                arrayActiveQuest[indexQuestData].ID = dataSaveQuest.dataQuest[indexQuestData].ID;
                arrayActiveQuest[indexQuestData].currentStepNumber = dataSaveQuest.dataQuest[indexQuestData].currentStepNumber;

                for (int indexStepsQuests = 0; indexStepsQuests < dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests.Length; indexStepsQuests++)
                {
                    arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].nextStepNumber =
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].stepNumber;

                    for (int indexActionCount = 0; indexActionCount < arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length; indexActionCount++)
                    {
                        arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount =
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
        dataSaveQuest.dataQuest = new DataQuest[arrayActiveQuest.Count];
        for (int indexQuestData = 0; indexQuestData < arrayActiveQuest.Count; indexQuestData++)
        {
            dataSaveQuest.dataQuest[indexQuestData] = new DataQuest();
            dataSaveQuest.dataQuest[indexQuestData].ID = arrayActiveQuest[indexQuestData].ID;
            dataSaveQuest.dataQuest[indexQuestData].currentStepNumber = arrayActiveQuest[indexQuestData].currentStepNumber;

            dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests = new StepsSaveQuest[arrayActiveQuest[indexQuestData].arrayStepsQuests.Length];
            for (int indexStepsQuests = 0; indexStepsQuests < arrayActiveQuest[indexQuestData].arrayStepsQuests.Length; indexStepsQuests++)
            {
                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests] = new StepsSaveQuest();
                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].stepNumber =
                    arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].nextStepNumber;

                dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts =
                    new ActionSaveCount[arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length];
                for (int indexActionCount = 0; indexActionCount < arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts.Length; indexActionCount++)
                {
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount] = new ActionSaveCount();
                    dataSaveQuest.dataQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount =
                        arrayActiveQuest[indexQuestData].arrayStepsQuests[indexStepsQuests].actionCounts[indexActionCount].currentCount;
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

    private IEnumerator TimerPanelPopup(int index, string textMission, List<QuestData> currentArray)
    {
        ShowPanelPopup(index, textMission, currentArray);
        yield return new WaitForSeconds(4.0f);
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

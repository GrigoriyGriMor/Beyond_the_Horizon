using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class NPCDialogPlayer : DialogBase
{
    //[HideInInspector]
    //public int indexArrayDialog;

    private Text[] answerPlayer;

    [SerializeField]
    private Text nameNPC;

    [SerializeField]
    private Text textNPC;

    [SerializeField]
    private Transform contentScroll;

    [SerializeField]
    private NpcController npcController;

    [SerializeField]
    private Button buttonBack;

    [SerializeField]
    private Button prefabAnswerPlayer;

    [SerializeField]
    private DialogNPC[] arrayDialog;

    [SerializeField]
    private List<Button> buttonAnswerPlayer;

    private StateNPC stateNPC;

    private bool isEnable;

    [SerializeField]
    private Transform UIDialogWindow;

    //[SerializeField]
    private QuestManager questManagerPlayer;

    //[SerializeField]
    private ActivatorQuestDialog activatorQuestDialog;


    //[SerializeField]
    private string nameBrand = "";

    private void OnEnable()
    {
        indexArrayDialog = 0;
        UIDialogWindow.gameObject.SetActive(false);
        isEnable = true;
    }

    private void Start()
    {
        indexArrayDialog = 0;
        TryGetComponent(out activatorQuestDialog);

        CreateButtonAnswerPlayer();
        UIDialogWindow.gameObject.SetActive(false);

        isEnable = true;
    }

    public void SetNameBrand(string nameBrand)
    {
        this.nameBrand = nameBrand;
    }


    private void CreateButtonAnswerPlayer()
    {
        buttonBack.onClick.AddListener(ExitClick);

        int tempLength = 0;

        int lastLength = 0;

        for (int index = 0; index < arrayDialog.Length; index++)
        {
            tempLength = arrayDialog[index].answerPlayer.Length;

            if (lastLength < tempLength)
            {
                lastLength = tempLength;
            }
        }

        // Debug.Log(" lastLength " + lastLength);

        while (buttonAnswerPlayer.Count < lastLength)
        {
            Button tempButton = Instantiate(prefabAnswerPlayer, Vector3.zero, Quaternion.identity, contentScroll);

            buttonAnswerPlayer.Add(tempButton);
        }

        answerPlayer = new Text[buttonAnswerPlayer.Count];

        for (int index = 0; index < buttonAnswerPlayer.Count; index++)
        {
            //buttonAnswerPlayer[index].onClick.AddListener(AnswerClick);

            int nextDialog = index;

            buttonAnswerPlayer[index].onClick.AddListener(() => AnswerClick(nextDialog));

            answerPlayer[index] = buttonAnswerPlayer[index].GetComponentInChildren<Text>();
        }
    }

    private void ShowButtonAnswerPlayer()
    {
        if (arrayDialog.Length > 0)
        {
            for (int index = 0; index < buttonAnswerPlayer.Count; index++)
            {
                buttonAnswerPlayer[index].gameObject.SetActive(false);
            }
            nameNPC.text = npcController.GetName();
            DialogNPCAnimAudio scrObj_dialogNPCAnimAudio = arrayDialog[indexArrayDialog].ScrObj_dialogNPCAnimAudio;
            int randomIndex = Random.Range(0, scrObj_dialogNPCAnimAudio.triggerAnimAudioClips.Length);
            string animNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].triggerAnimator;
            AudioClip audioNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].audioClip;

            if (nameBrand != "")
            {
                textNPC.text = System.String.Format(scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].textNPC, nameBrand);
            }
            else
            {
                textNPC.text = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].textNPC;
            }

            SetAnimator(animNPC);
            SetAudioClip(audioNPC);
            for (int index = 0; index < arrayDialog[indexArrayDialog].answerPlayer.Length; index++)
            {
                buttonAnswerPlayer[index].gameObject.SetActive(true);
                //Debug.Log(index);
                answerPlayer[index].text = arrayDialog[indexArrayDialog].answerPlayer[index].textAnswerPlayer;
            }
        }
    }

    private void SetAnimator(string animNPC)
    {
        Animator animatorNPC = npcController.animator;

        if (stateNPC != StateNPC.dialogCampNPC)
        {
            if (animatorNPC)
            {
                animatorNPC.SetTrigger(animNPC);
            }
        }
    }

    private void SetAudioClip(AudioClip audioNPC)
    {
        AudioSource audioSource = GetComponentInParent<AudioSource>();

        if (audioSource)
        {
            audioSource.clip = audioNPC;
            audioSource.Play();
        }
    }

    private void AnswerClick(int nextDialog)
    {
        arrayDialog[indexArrayDialog].answerPlayer[nextDialog].unityEvent.Invoke();
        if (indexArrayDialog != arrayDialog[indexArrayDialog].answerPlayer[nextDialog].nextDialog)
        {
            indexArrayDialog = arrayDialog[indexArrayDialog].answerPlayer[nextDialog].nextDialog;
            NextAnswer();
        }
        else
        {
            ExitClick();
        }
    }

    private void NextAnswer()
    {
        ShowButtonAnswerPlayer();
    }

    private void ExitClick()
    {
        indexArrayDialog = 0;
        npcController.EndDialog();
    }

    private LookModule lookModule;

    [SerializeField]
    private Transform pointCameraTalking;
    [SerializeField]
    private Transform pointLookCamera;
    [SerializeField]
    private GameObject backPlane;

    public override void StartDialog(StateNPC stateNPC)
    {
        this.stateNPC = stateNPC;
        UIDialogWindow.gameObject.SetActive(true);
        npcController.currentPlayer.TryGetComponent(out questManagerPlayer);

        lookModule = npcController.currentPlayer.GetComponentInChildren<LookModule>();

        if (lookModule)
        {
            lookModule.TalkingNPC(pointCameraTalking, backPlane,  pointLookCamera);
        }
        else
        {
            print("Not LookModule");
        }

        if (activatorQuestDialog && questManagerPlayer)
        {
            activatorQuestDialog.SetNextDialog(questManagerPlayer);
        }
        else
        {
            Debug.Log(" Not ActivatorQuestBase or questManagerPlayer");
        }

        ShowButtonAnswerPlayer();
    }

    public override void EndDialog()
    {
        if (lookModule)
        {
            lookModule.EndTalkingNPC();
        }
        else
        {
            print("Not LookModule");
        }
        UIDialogWindow.gameObject.SetActive(false);
    }
}

/// <summary>
/// Класс Диалогов
/// </summary>
[System.Serializable]
public class DialogNPC
{
    /// <summary>
    /// Триггеры анимации и аудио
    /// </summary>
    public DialogNPCAnimAudio ScrObj_dialogNPCAnimAudio;

    /// <summary>
    /// Ответы Игрока + след диалог
    /// </summary>
    public AnswerPlayer[] answerPlayer;
}


/// <summary>
/// Класс ответы Игрока +
/// след диалог
/// </summary>
[System.Serializable]
public class AnswerPlayer
{
    /// <summary>
    /// Ответ Игрока  
    /// </summary>
    [SerializeField]
    public string textAnswerPlayer;

    /// <summary>
    /// след диалог
    /// </summary>
    [SerializeField]
    public int nextDialog;

    /// <summary>
    /// событие
    /// </summary>
    public UnityEvent unityEvent;
}




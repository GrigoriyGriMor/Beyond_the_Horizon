using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NPCRandomAnswerPlayer : DialogBase
{
    [SerializeField]
    private Transform panelInfoNPC;

    [SerializeField]
    private Text nameNPC;

    [SerializeField]
    private Text textNPC;

    [SerializeField]
    private NpcController npcController;

    [SerializeField]
    private DialogNPCAnimAudio scrObj_dialogNPCAnimAudio;

    [SerializeField]
    private float timeShowPanelNPC = 2.0f;

    private Coroutine refTimerShowPanelNPC;

    //private bool isEnable;

    //private float timerAnimation;

    private StateNPC stateNPC;

    private void OnEnable()
    {
        //if (isEnable)
        //{
        //   // SetRandomDialog();
        //}
        //isEnable = true;
    }

    private void Start()
    {

    }

    private void SetRandomDialog()
    {
        int randomIndex = Random.Range(0, scrObj_dialogNPCAnimAudio.triggerAnimAudioClips.Length);

        string animNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].triggerAnimator;

        AudioClip audioNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].audioClip;

        SetAnimator(animNPC);

        SetAudioClip(audioNPC);

        ShowPanelNPC();

        Debug.Log(" random fraza");
    }

    private void SetAnimator(string animNPC)
    {
        if ((stateNPC != StateNPC.walkSleepNPC && stateNPC != StateNPC.walkNPC) && stateNPC != StateNPC.dialogCampNPC)
        {
            if (npcController.animator)
            {
                npcController.animator.SetTrigger(animNPC);
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

    public override void StartDialog(StateNPC stateNPC)
    {
        this.stateNPC = stateNPC;
        SetRandomDialog();
    }

    private void ShowPanelNPC()
    {
        int randomIndex = Random.Range(0, scrObj_dialogNPCAnimAudio.triggerAnimAudioClips.Length);

        nameNPC.text = npcController.GetName();

        textNPC.text = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].textNPC;

        refTimerShowPanelNPC = StartCoroutine(TimerPanelNpc());
    }

    private IEnumerator TimerPanelNpc()
    {
        panelInfoNPC.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeShowPanelNPC);
        panelInfoNPC.gameObject.SetActive(false);
    }

}






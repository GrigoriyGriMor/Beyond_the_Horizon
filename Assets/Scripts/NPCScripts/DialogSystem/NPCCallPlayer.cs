using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCCallPlayer : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private NpcController npcController;

    [SerializeField]
    private Transform panelInfoNPC;

    [SerializeField]
    private Text nameNPC;

    [SerializeField]
    private Text textNPC;

    [Header("Мах растояние видимости игрока")]
    [SerializeField]
    private float maxDistanceVisiblePlayer = 10.0f;

    [SerializeField]
    private float timeShowPanelNPC = 3.0f;

    [Header("Таймер проверки игроков")]
    [SerializeField]
    private float timeFindAllPlayer = 1.0f;

    [SerializeField]
    private List<PlayerController> findedPlayers;

    [SerializeField]
    private DialogNPCAnimAudio scrObj_dialogNPCAnimAudio;

    private Transform thisTransform;

    private Coroutine refTimerShowPanelNPC;

    private void OnEnable()
    {
        Init();
    }

    private void OnDisable()
    {
        panelInfoNPC.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void Init()
    {
        thisTransform = GetComponent<Transform>();
        StartCoroutine(TimerFindAllPlayer());
    }

    void Start()
    {

    }

    /// <summary>
    /// Находим всех игроков
    /// </summary>
    /// <param name="maxDistanceVisiblePlayer"></param>
    private void FindAllPlayer(float maxDistanceVisiblePlayer)
    {
        Vector3 center = thisTransform.position;

        Collider[] hitColliders = Physics.OverlapSphere(center, maxDistanceVisiblePlayer);

        CheckOutPlayer(hitColliders);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out PlayerController playerController))
            {
                if (CheckPlayerInArray(playerController))
                {
                    CallPlayer();
                }
            }
        }
    }

    /// <summary>
    /// Проверяем ушедших игроков
    /// </summary>
    /// <param name="hitColliders"></param>
    private void CheckOutPlayer(Collider[] hitColliders)
    {
        foreach (var tempPlayer in findedPlayers)
        {
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.TryGetComponent(out PlayerController playerController))
                {
                    if (playerController == tempPlayer)
                    {
                        return;
                    }
                }
            }

            findedPlayers.Remove(tempPlayer);
            break;
        }
    }

    /// <summary>
    /// проверяем есть ли игрок в массиве
    /// </summary>
    /// <param name="playerController"></param>
    private bool CheckPlayerInArray(PlayerController playerController)
    {
        bool result = false;

        foreach (var tempPlayer in findedPlayers)
        {
            if (playerController == tempPlayer)
            {
                return result;
            }
        }

        findedPlayers.Add(playerController);

        return result = true;
    }

    private IEnumerator TimerFindAllPlayer()
    {
        while (true)
        {
            //print("ky");
            FindAllPlayer(maxDistanceVisiblePlayer);
            yield return new WaitForSeconds(timeFindAllPlayer);

        }
    }

    private void CallPlayer()
    {
        int randomIndex = Random.Range(0, scrObj_dialogNPCAnimAudio.triggerAnimAudioClips.Length);

        nameNPC.text = npcController.GetName();

        textNPC.text = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].textNPC;

        string animNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].triggerAnimator;

        AudioClip audioNPC = scrObj_dialogNPCAnimAudio.triggerAnimAudioClips[randomIndex].audioClip;

        ShowPanelNPC();

        SetAnimator(animNPC);

        SetAudioClip(audioNPC);
    }

    private void SetAnimator(string animNPC)
    {
        if (npcController.animator)
        {
            if (npcController.stateNPC != StateNPC.campNPC)
            {
                if (npcController.stateNPC != StateNPC.walkNPC)
                {
                    if (npcController.stateNPC != StateNPC.walkSleepNPC)
                    {
                        npcController.animator.SetTrigger(animNPC);
                    }
                }

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

    private void ShowPanelNPC()
    {
        refTimerShowPanelNPC = StartCoroutine(TimerPanelNpc());
    }

    private IEnumerator TimerPanelNpc()
    {
        panelInfoNPC.gameObject.SetActive(true);
        yield return new WaitForSeconds(timeShowPanelNPC);
        panelInfoNPC.gameObject.SetActive(false);
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManagerAllControll : MonoBehaviour
{

    private static SoundManagerAllControll instance;
    public static SoundManagerAllControll Instance => instance;

    [SerializeField] private int souceCurrent = 5;
    [SerializeField] private List<AudioSource> mySorce = new List<AudioSource>();
    [SerializeField] private List<AudioSource> otherSorce = new List<AudioSource>();


    [SerializeField] private AudioSource backgrounSorce;
    [SerializeField] [Range(0, 1)] private float backgroundVolue = 0.7f;

    [SerializeField] private ButtonUISpriteChanger muteButton;

    private void OnLevelWasLoaded(int level)
    {
        SoundManagerAllControll _thisGO = GetComponent<SoundManagerAllControll>();

        SoundManagerAllControll[] _go = FindObjectsOfType<SoundManagerAllControll>();
        for (int i = 0; i < _go.Length; i++)
            if (_go[i] != _thisGO)
                Destroy(_go[i].gameObject);
    }

    private void Awake()
    {
        instance = this;
        Inicialize();
    }

    private void Inicialize()
    {
        backgrounSorce = gameObject.AddComponent<AudioSource>();//создаем отдельный сорс для бэкграунда
        for (int i = 0; i < souceCurrent; i++)
            mySorce.Add(gameObject.AddComponent<AudioSource>());
    }

    public void SoundButtonInit(ButtonUISpriteChanger button)
    {
        muteButton = button;

        if (backgrounSorce.volume == 0)
            muteButton.ChangeSprite(false);
        else
            muteButton.ChangeSprite(true);

        muteButton.gameObject.GetComponent<Button>().onClick.AddListener(() => OnSounSwitch());
    }

    public void BackgroundClipPlay(AudioClip clip)
    {
        backgrounSorce.clip = clip;
        backgrounSorce.loop = true;
        backgrounSorce.Play();
    }

    public void LeaveScene()
    {
        backgrounSorce.clip = null;

        if (muteButton != null) muteButton.gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        muteButton = null;

        otherSorce.RemoveRange(0, otherSorce.Count);
    }

    public void ClipPlay(AudioClip clip)
    {
        AudioSource sr = GetFree();
        if (sr != null) sr.PlayOneShot(clip);
    }

    public void ClipPlay(AudioClip clip, AudioSource _source)
    {
        for (int i = 0; i < otherSorce.Count; i++)
            if (otherSorce[i] == _source)
            {
                _source.PlayOneShot(clip);
                return;
            }

        otherSorce.Add(_source);
        if (backgrounSorce.volume == 0)
            _source.volume = 0;
        else
            _source.PlayOneShot(clip);
    }

    public void ClipLoopAndPlay(AudioClip clip)
    {
        AudioSource sr = GetFree();
        sr.clip = clip;
        sr.loop = true;
        otherSorce.Add(sr);
        sr.Play();
    }

    public void ClipLoopAndPlay(AudioClip clip, AudioSource _source)
    {
        _source.clip = clip;
        _source.loop = true;
        _source.Play();
    }

    private AudioSource GetFree()
    {
        for (int i = 0; i < mySorce.Count; i++)
        {
            if (mySorce[i].clip == null)
                return mySorce[0];
        }

        return null;
    }

    public void ClearSound()
    {
        for (int i = 0; i < mySorce.Count; i++)
        {
            if (mySorce[i] != null)
            {
                if (mySorce[i].clip != null)
                    mySorce[i].clip = null;
            }
        }
    }

    public void OnSounSwitch()
    {
        if (backgrounSorce.volume == 0)
        {
            backgrounSorce.volume = backgroundVolue;
            for (int i = 0; i < mySorce.Count; i++)
                mySorce[i].volume = 1;

            for (int i = 0; i < otherSorce.Count; i++)
                otherSorce[i].volume = 1;


            if (muteButton)
                muteButton.ChangeSprite(true);
        }
        else
        {
            backgrounSorce.volume = 0.0f;
            for (int i = 0; i < mySorce.Count; i++)
                mySorce[i].volume = 0;

            for (int i = 0; i < otherSorce.Count; i++)
                otherSorce[i].volume = 0;

            if (muteButton)
                muteButton.ChangeSprite(false);
        }
    }

    public void OnBackgroundSound()
    {
        if (backgrounSorce.volume == 0)
            backgrounSorce.volume = backgroundVolue;
        else
            backgrounSorce.volume = 0.0f;
    }

    public void OnSound()
    {
            backgrounSorce.volume = backgroundVolue;
            for (int i = 0; i < mySorce.Count; i++)
                mySorce[i].volume = 1;

            for (int i = 0; i < otherSorce.Count; i++)
                otherSorce[i].volume = 1;


            if (muteButton)
                muteButton.ChangeSprite(true);
    }

    public void OffSound()
    {
            backgrounSorce.volume = 0.0f;
            for (int i = 0; i < mySorce.Count; i++)
                mySorce[i].volume = 0;

            for (int i = 0; i < otherSorce.Count; i++)
                otherSorce[i].volume = 0;

            if (muteButton)
                muteButton.ChangeSprite(false);
    }
}

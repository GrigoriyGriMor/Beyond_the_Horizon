using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScreenController : MonoBehaviour
{
    private static LoadScreenController instance;
    public static LoadScreenController Instance => instance;

    private GameObject loadSc;

    [SerializeField] private Animator anim;

    [Header("Использовать DontDestroyOnLoad?")]
    [SerializeField] private bool useDontDestroyGO = true;


    private void Start()//Отключает экран на старте (т.к. его надо включать самостоятельно там где это необходимо (обычно перед загрузкой сцены))
    {
        if (useDontDestroyGO)
            DontDestroyOnLoad(gameObject);

        instance = this;
        loadSc = gameObject.transform.GetChild(0).gameObject;
       // loadSc.SetActive(false);
    }

    public void LoadScreenActive()
    {
        if (loadSc != null && !loadSc.activeInHierarchy) loadSc.gameObject.SetActive(true);
    }

    public void LoadScreenDeactive()
    {
        StartCoroutine(Deactive());
    }

    private IEnumerator Deactive()
    {
        anim.SetTrigger("Loaded");

        yield return new WaitForSeconds(0.5f);

        if (loadSc != null && loadSc.activeInHierarchy) loadSc.gameObject.SetActive(false);
    }
}

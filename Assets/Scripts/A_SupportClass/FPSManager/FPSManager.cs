using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSManager : MonoBehaviour
{
    private float fps;

    [SerializeField]
    private Text textField;


    private void Start()
    {
        StartCoroutine(UpdateFps());
    }
    //void LateUpdate()
    //{
    //    fps = 1.0f / Time.deltaTime;
    //    fps = Mathf.Round(fps);
    //    textField.text = fps.ToString();
    //}

    private IEnumerator UpdateFps()
    {
        while (true)
        {
            fps = 1.0f / Time.deltaTime;
            fps = Mathf.Round(fps);
            textField.text = fps.ToString();
            yield return new WaitForSeconds(0.1f);
        }
    }

}

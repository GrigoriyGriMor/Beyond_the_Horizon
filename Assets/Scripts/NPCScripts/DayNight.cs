using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    [SerializeField]
    private NPCDinStatic nPCDinStatic;

    [SerializeField]
    private float timerDay = 100.0f;

    [SerializeField]
    private float timerNight = 100.0f;

    void Start()
    {
        StartCoroutine(TimerDayNight());
    }

    private IEnumerator TimerDayNight()
    {
        while (true)
        {
            nPCDinStatic.isNight = !nPCDinStatic.isNight;
            yield return new WaitForSeconds(timerDay);
            nPCDinStatic.isNight = !nPCDinStatic.isNight;
            yield return new WaitForSeconds(timerNight);

        }
    }
}

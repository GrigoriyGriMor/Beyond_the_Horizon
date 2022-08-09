using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    [SerializeField]
    private Image imageColor;

    [SerializeField]
    private float speedChange = 1.0f;

    [SerializeField]
    private Color startImageColor;

    [SerializeField]
    private Color endImageColor;

    [SerializeField]
    private bool isAgression;

    [Header(" Enemy Controller ")]
    [SerializeField]
    private EnemyBase enemyBase;

    private void FixedUpdate()
    {
        if (imageColor)
        {
            if (enemyBase.isAggression)
            {
                imageColor.color = Color.Lerp(startImageColor, endImageColor, Time.captureDeltaTime * speedChange);
            }
            else
            {
                imageColor.color = Color.Lerp(endImageColor, startImageColor, Time.captureDeltaTime * speedChange);
            }
        }
    }
}

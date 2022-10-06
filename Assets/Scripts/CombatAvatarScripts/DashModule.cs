using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashModule : CharacterBase
{
    public KeyCode forward = KeyCode.W;
    public KeyCode back = KeyCode.S;
    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;

    [Header("Trigger Name")]
    [SerializeField] private string dashForward = "DashForward";

    private int countClick = 0;
    [SerializeField]
    private float delayDoubleClick = 0.2f;
    [SerializeField]
    private float delayDash = 2.0f;
    [SerializeField]
    private float moveSpeed = 1.0f;


    Coroutine refTimerTimerDoubleClick;

    private void Update()
    {
        KeyCode[] keyCodes = { forward, back, left, right };
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (CheckDoubleClick(keyCodes[i])) break;
        }
    }

    private bool CheckDoubleClick(KeyCode keyCode)
    {
        if (Input.GetKeyDown(keyCode))
        {
            if (countClick == 2)
            {
                countClick = 0;
                Dash(true);
                return true;
            }
        }
        if (Input.GetKeyUp(keyCode))
        {
            if (countClick == 0)
            {
                refTimerTimerDoubleClick = StartCoroutine(TimerDoubleClick());
            }
            countClick++;
            //Dash(false);
        }
        return false;
    }

    private IEnumerator TimerDoubleClick()
    {
        yield return new WaitForSeconds(delayDoubleClick);
        countClick = 0;
        refTimerTimerDoubleClick = null;
    }

    private void Dash(bool isDash)
    {
        if (isDash)
        {
            StartCoroutine(TimerDash());
            _player.gameIsPlayed = false;
            playerAnim.SetTrigger("dashForward");
            Vector2 moveVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector3 vec = new Vector3(moveVector.x * moveSpeed, _rb.velocity.y, moveVector.y * moveSpeed);
            _rb.velocity = transform.TransformVector(vec);
        }
        else
        {
            _player.gameIsPlayed = true;
        }
    }

    private IEnumerator TimerDash()
    {
        //player.SprintControl(true);
        yield return new WaitForSeconds(delayDash);
        // player.SprintControl(false);
        _player.gameIsPlayed = true;

    }


}

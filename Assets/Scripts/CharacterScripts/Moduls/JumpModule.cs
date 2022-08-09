using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpModule : CharacterBase {
    [SerializeField] private float JumpForce = 15.0f;

    [Header("Trigger Name")]
    [SerializeField] private string jumpForwardATN = "MoveForwardJump";
    [SerializeField] private string jumpBackATN = "MoveBackJump";
    [SerializeField] private string jumpRightATN = "MoveRightJump";
    [SerializeField] private string jumpLeftATN = "MoveLeftJump";

    [SerializeField] private string jumpIdleATN = "Jump";

    [Header("Idle Waiting Time")]
    [SerializeField] private float waitTimeAboutJump = 0.35f;

    public void Jumping(Vector2 moveAxis, SupportClass.PlayerStateMode mode) {
        StartCoroutine(JumpUse(moveAxis, mode));
    }

    private IEnumerator JumpUse(Vector2 moveAxis, SupportClass.PlayerStateMode mode) {
        switch (mode) {
            //IDLE
            case SupportClass.PlayerStateMode.Idle:
                playerAnim.SetTrigger(jumpIdleATN);
               // _player.SetNewTrigger(jumpIdleATN);

                if (state != SupportClass.gameState.clone) {
                    if (moveAxis.x > 0.05f || moveAxis.x < -0.05f || moveAxis.y < -0.05f || moveAxis.y > 0.05f) {
                        _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                        _rb.AddForce(visual.transform.forward * (JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                    }
                    else {
                        yield return new WaitForSeconds(waitTimeAboutJump);
                        _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                    }
                }
                break;
            //COMBAT
            case SupportClass.PlayerStateMode.Combat:
                playerAnim.SetTrigger(jumpIdleATN);
               // _player.SetNewTrigger(jumpIdleATN);

                if (state != SupportClass.gameState.clone) {
                    if (moveAxis.x > 0.05f) {
                        _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                        _rb.AddForce(visual.transform.forward * (JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                    }
                    else {
                        if (moveAxis.x < -0.05f) {
                            _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                            _rb.AddForce(visual.transform.forward * (-JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                        }
                        else {
                            if (moveAxis.y > 0.05f) {
                                _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                                _rb.AddForce(visual.transform.right * (JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                            }
                            else {
                                if (moveAxis.y < -0.05f) {
                                    _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                                    _rb.AddForce(visual.transform.right * (-JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                                }
                                else
                                    _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                            }
                        }
                    }
                }
                break;
            //SPRINT
            case SupportClass.PlayerStateMode.Sprint:
                if (moveAxis.x > 0.05f || moveAxis.x < -0.05f || moveAxis.y < -0.05f || moveAxis.y > 0.05f) 
                {
                    if (state != SupportClass.gameState.clone) {
                        _rb.AddForce(visual.transform.up * JumpForce, ForceMode.Impulse);//up
                        _rb.AddForce(visual.transform.forward * (JumpForce / 5f), ForceMode.Impulse);//some impuls forward
                    }

                    playerAnim.SetTrigger(jumpIdleATN);
                   // _player.SetNewTrigger(jumpIdleATN);

                }
                break;
        }
    }
}

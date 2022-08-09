using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{

    public Animator animator;

    private void Start()
    {

    }

    //====================================================Отправка===================================================================
    public void SetNewTrigger(string triggerName) {
        for (int i = 0; i < animator.parameterCount; i++) {
            if (animator.parameters[i].name == triggerName)
                triggerActive.Add(i);
        }
    }
    public List<int> triggerActive = new List<int>();
    //==============================================================================================================================


    /// <summary>
    /// SetBool
    /// </summary>
    /// <param name="parameterAnimator"></param>
    /// <param name="valueParameter"></param>
    public void SetStateAnimator(string parameterAnimator, bool valueParameter)
    {
        if (animator)
        {
            //Debug.Log("bool");
            animator.SetBool(parameterAnimator, valueParameter);
        }
    }

    public void SetStateAnimator(string parameterAnimator) {
        
        if (animator) {
            animator.SetTrigger(parameterAnimator);
            SetNewTrigger(parameterAnimator);
        }
    }

    /// <summary>
    /// SetFloat
    /// </summary>
    /// <param name="parameterAnimator"></param>
    /// <param name="valueParameter"></param>
    public void SetStateAnimator(string parameterAnimator, float valueParameter)
    {
        if (animator)
        {
            // Debug.Log("float");

            animator.SetFloat(parameterAnimator, valueParameter);
        }
    }

    public void SetAnimatorLayerWeight(int bodyUpLayer, int targetLayerWeight)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(SetLayerWeightCoroutine(bodyUpLayer, targetLayerWeight));
       // animator.SetLayerWeight(bodyUpLayer, weight);                          // вкл слой anim body
    }

    private Coroutine coroutine;
    private IEnumerator SetLayerWeightCoroutine(int bodyUpLayer, int targetLayerWeight) {

        while (Mathf.Abs(targetLayerWeight - animator.GetLayerWeight(bodyUpLayer)) > 0.1f) {
            animator.SetLayerWeight(bodyUpLayer, Mathf.LerpUnclamped(animator.GetLayerWeight(bodyUpLayer), targetLayerWeight, 10 * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }

        animator.SetLayerWeight(bodyUpLayer, targetLayerWeight);

        coroutine = null;
    }

}
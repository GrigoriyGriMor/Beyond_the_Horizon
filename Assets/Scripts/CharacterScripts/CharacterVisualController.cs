using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualController : MonoBehaviour
{
    [Header("Heads")]
    [SerializeField] private VisualRef[] headVisual = new VisualRef[1];

    [Header("Modules")]
    [SerializeField] private VisualRef[] hairStyles = new VisualRef[1];

    [Header("Jacket")]
    [SerializeField] private VisualRef[] jackets = new VisualRef[0];

    [Header("Pants")]
    [SerializeField] private VisualRef[] pants = new VisualRef[0];

    [Header("Gloves")]
    [SerializeField] private VisualRef[] gloves = new VisualRef[0];

    [Header("Boots")] 
    [SerializeField] private VisualRef[] boots = new VisualRef[0];

    [Header("Deafult Param")]
    [SerializeField] private float baseHealPointCount = 0;
    [SerializeField] private float baseShieldPointCount = 0;
    [SerializeField] private float baseSpeedCount = 0;

    public float GetBaseHeal() {
        return baseHealPointCount;
    }

    public float GetBaseShield() {
        return baseShieldPointCount;
    }

    public float GetBaseSpeed() {
        return baseSpeedCount;
    }

    public void SelectHead(int number) {
        for (int i = 0; i < headVisual.Length; i++)
            if (number == i) 
                headVisual[i].visual.SetActive(true);
            else 
                headVisual[i].visual.SetActive(false);
    }

    public void SelectHair(int number) {
        for (int i = 0; i < hairStyles.Length; i++)
            if (number == i)
                hairStyles[i].visual.SetActive(true);
            else
                hairStyles[i].visual.SetActive(false);
    }

    public void SelectJacket(int number) {
        for (int i = 0; i < jackets.Length; i++)
            if (number == i)
                jackets[i].visual.SetActive(true);
            else
                jackets[i].visual.SetActive(false);
    }

    public void SelectPants(int number) {
        for (int i = 0; i < pants.Length; i++)
            if (number == i)
                pants[i].visual.SetActive(true);
            else
                pants[i].visual.SetActive(false);
    }

    public void SelectGloves(int number) {
        for (int i = 0; i < gloves.Length; i++)
            if (number == i)
                gloves[i].visual.SetActive(true);
            else
                gloves[i].visual.SetActive(false);
    }

    public void SelectBoots(int number) {
        for (int i = 0; i < boots.Length; i++)
            if (number == i)
                boots[i].visual.SetActive(true);
            else
                boots[i].visual.SetActive(false);
    }

    public Sprite[] GetHeadVisual() {
        Sprite[] spArr = new Sprite[headVisual.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = headVisual[i].sprite;

        return spArr;
    }

    public Sprite[] GetHairVisual() {
        Sprite[] spArr = new Sprite[hairStyles.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = hairStyles[i].sprite;

        return spArr;
    }

    public Sprite[] GetJacketsVisual() {
        Sprite[] spArr = new Sprite[jackets.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = jackets[i].sprite;

        return spArr;
    }

    public Sprite[] GetPantsVisual() {
        Sprite[] spArr = new Sprite[pants.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = pants[i].sprite;

        return spArr;
    }

    public Sprite[] GetGlovesVisual() {
        Sprite[] spArr = new Sprite[gloves.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = gloves[i].sprite;

        return spArr;
    }

    public Sprite[] GetBootsVisual() {
        Sprite[] spArr = new Sprite[boots.Length];
        for (int i = 0; i < spArr.Length; i++)
            spArr[i] = boots[i].sprite;

        return spArr;
    }
}

[System.Serializable]
public class VisualRef {
    public GameObject visual;
    public Sprite sprite;
}
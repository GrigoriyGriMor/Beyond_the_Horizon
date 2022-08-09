using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsModule : CharacterBase
{
    [Header("Skills")]
    [SerializeField] private List<SkillModuleParam> skills = new List<SkillModuleParam>();
        
    private void Start() {
        for (int i = 0; i < skills.Count; i++) {
            SkillParams param = skills[i].skill.Init(state, visual, _rb, playerAnim, this, null, mainCamera, _player);
            skills[i].skillImages.sprite = param.skillImage;
            skills[i].coolDown = param.skillCoolDown;

            skills[i].cooldownText.text = "";
            skills[i].currentColldown = 0;
            skills[i].coolDownSlider.gameObject.SetActive(false);

            skills[i].cooldownText.text = "";
        }
    }

    public void UseSkill(int skillNumber) {
        if (skills.Count <= skillNumber || (skills.Count > skillNumber && skills[skillNumber] == null)) return;

        if (skills[skillNumber].currentColldown <= 0) {
            skills[skillNumber].skill.UseSkill();
            StartCoroutine(SkillCoolDown(skillNumber));
        }
    }

    private IEnumerator SkillCoolDown(int skillNumber) {
        skills[skillNumber].currentColldown = skills[skillNumber].coolDown;
        skills[skillNumber].skillImageAnim.SetTrigger("In");
        skills[skillNumber].coolDownSlider.gameObject.SetActive(true);
        skills[skillNumber].coolDownSlider.value = (-1) * skills[skillNumber].currentColldown;

        while (skills[skillNumber].currentColldown > 0) {
            skills[skillNumber].currentColldown -= Time.fixedDeltaTime;
            skills[skillNumber].coolDownSlider.value = (-1) * skills[skillNumber].currentColldown;
            skills[skillNumber].cooldownText.text = Mathf.CeilToInt(skills[skillNumber].currentColldown).ToString();
            yield return new WaitForFixedUpdate();
        }

        skills[skillNumber].cooldownText.text = "";
        skills[skillNumber].currentColldown = 0;
        skills[skillNumber].skillImageAnim.SetTrigger("Out");
        skills[skillNumber].coolDownSlider.gameObject.SetActive(false); 
    }
}

[System.Serializable]
public class SkillModuleParam {
    public SkillBase skill;
    public Image skillImages;
    public TMPro.TMP_Text cooldownText;
    public TMPro.TMP_Text countText;
    public Slider coolDownSlider;

    [HideInInspector] public float currentColldown;
    [HideInInspector] public float coolDown;

    public Animator skillImageAnim;
}

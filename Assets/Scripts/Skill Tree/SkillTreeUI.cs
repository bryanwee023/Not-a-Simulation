using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeUI : MonoBehaviour
{
    public bool unlock;

    public ChooseSkill[] skillButtons;
    public GameObject noSkillUI;

    private void Start()
    {
        Time.timeScale = 0;
        PlayerController.instance.paused = true;

        RefreshSkills();

    }

    private void RefreshSkills()
    {
        Skill[] skills = this.unlock ? SkillTree.instance.Get() : SkillTree.instance.GetUpgrades();

        if (skills[0] == null)
        {
            Instantiate(noSkillUI);
            this.Close();
            return;
        }

        for (int i = 0; i < skills.Length; i++)
        {
            skillButtons[i].ParseSkill(skills[i]);
            skillButtons[i].closeWindow = () => this.Close(); 
        }
    }

    private void Close()
    {
        Time.timeScale = 1;
        //TODO: animate close
        PlayerController.instance.paused = false;
        Destroy(this.gameObject);
    }
}

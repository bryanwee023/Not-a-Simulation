using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSkill : MonoBehaviour
{
    public delegate void CloseDelegate();
    public CloseDelegate closeWindow;

    public Image sprite;
    public new Text name;
    public Text description;
    public Text stats;

    public void ParseSkill(Skill skill)
    {
        if (skill == null)
        {
            this.gameObject.SetActive(false);
        } else
        {
            this.gameObject.SetActive(true);
            this.name.text = skill.name;
            this.description.text = skill.description;

            this.sprite.sprite = Resources.Load<Sprite>(string.Format("Skill Icons/{0}", skill.name));

            this.stats.text = skill.stats();
        }
    }

    public void Unlock()
    {
        SkillTree.instance.Unlock(name.text);
        AudioManager.instance.skillButton.Play();
        this.closeWindow();
    }

    public void Upgrade()
    {
        SkillTree.instance.Upgrade(name.text);
        AudioManager.instance.skillButton.Play();
        this.closeWindow();
    }

}

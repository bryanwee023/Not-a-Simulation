using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;

public enum Weapon { BAT, YOYO }

[System.Serializable]
public class Wrapper
{
    public Skill[] skills;
}

[System.Serializable]
public class Skill
{
    public string name;
    public string description;
    public string[] dependencies;
    public string[] preclusions;

    public int type;
    public string path;

    public string[] baseStats;
    public float[] baseValues;

    public string keyword;
    public string format;

    public string upStats;
    public float[] upValues;

    public int level;

    public string stats()
    {
        if (keyword == "") return "";

        if (level == 0)
        {
            return keyword + ": " + string.Format(format, upValues[0]);
        }
        string suffix = string.Format(format, upValues[level]) + " > " + string.Format(format, upValues[level + 1]);
        return keyword + "\n" + suffix;
    }
}

public class SkillTree
{
    private Dictionary<string, Skill> skillBank = new Dictionary<string, Skill>();
    private HashSet<string> unlockableSkills = new HashSet<string>();
    private HashSet<string> upgradeableSkills = new HashSet<string>();

    public bool testMode;
    public static SkillTree instance;

    public SkillTree(Weapon weapon, bool testMode = false)
    {
        this.testMode = testMode;
        string path = (weapon == Weapon.BAT) ? "Skill Tree/bat_skills"
            : "Skill Tree/yoyo_skills";

        //Parse JSON file to retrieve skills
        string jsonString = Resources.Load<TextAsset>(path).ToString();

        foreach (Skill skill in JsonUtility.FromJson<Wrapper>(jsonString).skills)
        {
            skillBank.Add(skill.name, skill);
        }

        //ValidateBank();

        if (weapon == Weapon.BAT)
        {

            this.Unlock("Bat Swing", check_requirements: false);
            this.Unlock("Bat Spin", check_requirements: false);
        }
        else if (weapon == Weapon.YOYO)
        {
            this.Unlock("Yoyo Strike", check_requirements: false);
            this.Unlock("Grappling Hook", check_requirements: false);
        }

        this.Unlock("Dash", check_requirements: false);

        instance = this;
    }


    //Randomly retrieves 3 unlockable skills
    public Skill[] Get()
    {
        List<string> skillList = unlockableSkills.ToList();
        skillList = skillList.OrderBy( x => Random.value ).ToList( );

        Skill[] ret = new Skill[3];
        for (int i = 0; i < Mathf.Min(skillList.Count, 3); i++)
            ret[i] = skillBank[skillList[i]];

        return ret;
    }

    public void Unlock(string name, bool check_requirements = true)
    {
        if (check_requirements && !unlockableSkills.Contains(name))
        {
            Debug.LogFormat("WARNING: {0} not unlockable yet", name);
            return;
        }

        Skill unlocked = skillBank[name];
        unlocked.level = 1;

        unlockableSkills.Remove(name);
        foreach (string preclusion in unlocked.preclusions)
            unlockableSkills.Remove(preclusion);

        foreach (string dependency in unlocked.dependencies)
            unlockableSkills.Add(dependency);

        if (testMode) Debug.Log(name + " unlocked!");
        else
        {
            if (unlocked.path != "")
            {
                Dictionary<string, float> newAttributes = new Dictionary<string, float>();
                for (int i = 0; i < unlocked.baseStats.Length; i++)
                    newAttributes.Add(unlocked.baseStats[i], unlocked.baseValues[i]);
                
                PlayerController.LoadAbility(unlocked.type, unlocked.path, newAttributes);
            }
            if (unlocked.upStats != "")
                PlayerController.UpgradeAbility(unlocked.type, unlocked.upStats, unlocked.upValues[0]);
            
        }

        if (unlocked.level < unlocked.upValues.Length - 1)
            upgradeableSkills.Add(name);

    }

    //Randomly retrieves 3 upgradeable skills
    public Skill[] GetUpgrades()
    {
        List<string> skillList = upgradeableSkills.ToList();
        skillList = skillList.OrderBy( x => Random.value ).ToList( );

        Skill[] ret = new Skill[3];
        for (int i = 0; i < Mathf.Min(skillList.Count, 3); i++)
            ret[i] = skillBank[skillList[i]];

        return ret;
    }

    public void Upgrade(string name)
    {
        Skill skill = skillBank[name];

        if (skill.level >= skill.upValues.Length - 1)
        {
            Debug.LogFormat("WARNING: {0} already at max level");
            return;
        }

        if (testMode) Debug.Log(name + " upgraded to level " + skill.level + 1);
        else
        {
            PlayerController.UpgradeAbility(skill.type, skill.upStats, skill.upValues[skill.level]);
        }

        skill.level++;
        if (skill.level >= skill.upValues.Length - 1)
        {
            Debug.Log(string.Format("removing {0} level {1}, since only {2} levels", skill.name, skill.level, skill.upValues.Length - 1));
            upgradeableSkills.Remove(name);
        }
        
    }
}
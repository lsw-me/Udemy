using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pary_Skill : Skill
{


    [Header("Perry")]
    [SerializeField]private UI_SkillTreeSlot parryUnlockButton;
    public bool parryUnlocked {  get; private set; }


    [Header("Parry restore")]
    [SerializeField] private UI_SkillTreeSlot restoreUnlockButton;
    public bool restoreUnlocked {  get; private set; }

    [Header("Perry with a  mirage")]
    [SerializeField] private UI_SkillTreeSlot perryWithaMirageButtom;
    [Range(0f,1f)]
    [SerializeField] private float restoreHealthPercentage;
    public bool perryWithMirageUnlocked {  get; private set; }
    public override void UseSkill()
    {
        base.UseSkill();

        if (restoreUnlocked)
        {
            int restoreValue =Mathf.RoundToInt( player.stats.GetMaxHealthValue() * restoreHealthPercentage);
            player.stats.IncreaseHealthBy(restoreValue);

        }
    }

    protected override void Start()
    {
        base.Start();

        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParryRestore);
        perryWithaMirageButtom.GetComponent<Button>().onClick.AddListener(UnlockPerryWithMirage);
    }

    protected override void CheckUnlock()
    {
        UnlockParry();
        UnlockParryRestore();
        UnlockPerryWithMirage();
    }
    private void UnlockParry()
    {
        if(parryUnlockButton.unlocked)
            parryUnlocked = true;
    }
    private void UnlockParryRestore()
    {
        if (restoreUnlockButton.unlocked)
            restoreUnlocked = true;
    }
    private void UnlockPerryWithMirage()
    {
        if(perryWithaMirageButtom.unlocked)
            perryWithMirageUnlocked =true;
    }

    public void MakeMirageOnParry(Transform _respawnTransform)
    {
        if (perryWithaMirageButtom.unlocked)
            SkillManager.instance.clone.CreatCloneWithDelay(_respawnTransform);
    }

}

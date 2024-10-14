using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dodge_Skill : Skill
{
    [Header("Dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockDodgeButton;
    [SerializeField] private int evasionAmount;
    public bool dodgeUnlocked {  get; private set; }

    [Header("Mirage dodge")]
    [SerializeField] private UI_SkillTreeSlot unlockMirageDodgeButton;
    public bool dodgeMirageUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();

        unlockDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        unlockMirageDodgeButton.GetComponent<Button>().onClick.AddListener(UnlockMirageDodge);

    }

    protected override void CheckUnlock()
    {
        

        UnlockDodge();
        UnlockMirageDodge();
    }
    private  void UnlockDodge()
    {
        if(unlockDodgeButton.unlocked &&!dodgeUnlocked) //只生效一次，不然持续点击一直生效因为设计到给人物加evasion 如果一直生效就一直加 无敌了
        {
            player.stats.evasion.AddModifier(evasionAmount);
            Inventory.instance.UpdateStatsUI();
            dodgeUnlocked = true;
        }
    }
    private void UnlockMirageDodge()
    {
        if (unlockMirageDodgeButton.unlocked)
            dodgeMirageUnlocked = true;
    }

    public void CreateMirageOnDodge()
    {
        if (dodgeMirageUnlocked)
        {
            SkillManager.instance.clone.CreatClone(player.transform,new Vector3 (2 * player.facingDir,0));
        }
    }

}

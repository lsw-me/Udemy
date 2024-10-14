using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dash_Skill : Skill
{

    //132 合并技能树
    [Header("Dash")]
    [SerializeField] private UI_SkillTreeSlot dashUnlockButton;
    public bool dashUnlock {  get; private set; }

    [Header("Clone on dash ")]
    [SerializeField] private UI_SkillTreeSlot cloneOnDashUnlockButton;
    public bool cloneOnDashUnlock {  get; private set; }

    [Header("Clone on arrival")]
    [SerializeField] private UI_SkillTreeSlot cloneOnArrivalDashUnlockButton;
    public bool cloneOnArrivalDashUnlock { get; private set; } //利用skill tree ui 上的 button 来控制


    public override void UseSkill()
    {
        base.UseSkill();

        //Debug.Log("Create clone behind");
    }
    protected override void Start()
    {
        base.Start();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDash);
        cloneOnArrivalDashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnArrivalDash);
    }


    protected override void CheckUnlock()
    {
        UnlockDash();
        UnlockCloneOnDash();
        UnlockCloneOnArrivalDash();
    }
    private void UnlockDash()
    {

        if(dashUnlockButton.unlocked)
        {
            dashUnlock = true;
        }

    }

    private void UnlockCloneOnDash()
    {
        if (cloneOnDashUnlockButton.unlocked)
        cloneOnDashUnlock = true;

    }
    private void UnlockCloneOnArrivalDash()
    {
        if (cloneOnArrivalDashUnlockButton.unlocked)
        cloneOnArrivalDashUnlock = true;
    }

    public void CloneOnDash() //CreatCloneOnDashStart 原名
    {
        if (cloneOnDashUnlock)
        {
            SkillManager.instance.clone.CreatClone(player.transform, Vector3.zero);
        }
    }

    public void CloneOnArrival()  //CreatCloneOnDashStart  origin name
    {
        if (cloneOnArrivalDashUnlock)
        {
            SkillManager.instance.clone.CreatClone(player.transform, Vector3.zero);
        }
    }

}

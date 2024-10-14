using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Slider slider;
    [SerializeField] private Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackholeImage;
    [SerializeField] private Image flaskImage;

    private SkillManager skills;
    
    
    [Header("Soul info")]
    [SerializeField] private TextMeshProUGUI currentSouls;//当前的魂 买东西用
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate = 100;

    private void Start()
    {
        if (playerStats != null)
        {
            playerStats.onHealChanged += UpdateHealthUI;
        }


        skills = SkillManager.instance;
    }


    private void Update()
    {
        UPdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlock)  //进行技能cd ui 和冷却时间的绑定
            SetCooldownOf(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && skills.parry.parryUnlocked)
            SetCooldownOf(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && skills.crystal.crystalUnlocked)
            SetCooldownOf(crystalImage);
        if (Input.GetKeyDown(KeyCode.Mouse1) && skills.sword.swordUnlocked)
            SetCooldownOf(swordImage);
        if (Input.GetKeyDown(KeyCode.R))
            SetCooldownOf(blackholeImage);
        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.GetEquipment(EquipmentType.Flask) != null)
            SetCooldownOf(flaskImage);

        CheckCooldownOf(dashImage, skills.dash.cooldown);
        CheckCooldownOf(parryImage, skills.parry.cooldown);
        CheckCooldownOf(crystalImage, skills.crystal.cooldown);
        CheckCooldownOf(swordImage, skills.sword.cooldown);
        CheckCooldownOf(blackholeImage, skills.blackhole.cooldown);
        CheckCooldownOf(flaskImage, Inventory.instance.flaskCooldown);
    }

    private void UPdateSoulsUI()   //这个就是相当于 拿到钱之后不是直接从 100块 跳到200 块 而是随时间慢慢加到人物现在的魂的数量
    {
        if (soulsAmount < PlayerManager.instance.GetCurrentCurrency())
        {
            soulsAmount += Time.deltaTime * increaseRate;
        }
        else
        {
            soulsAmount = PlayerManager.instance.GetCurrentCurrency();
        }

        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = playerStats.GetMaxHealthValue();
        slider.value = playerStats.currentHealth;
    }

    private void SetCooldownOf(Image _image)
    {
        if(_image.fillAmount <= 0)
            _image.fillAmount = 1;
    }

    private void CheckCooldownOf(Image _image,float _cooldown)
    {
        if(_image.fillAmount > 0)
            _image.fillAmount -= 1/_cooldown * Time.deltaTime;
    }

}

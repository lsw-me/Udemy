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
    [SerializeField] private TextMeshProUGUI currentSouls;//��ǰ�Ļ� ������
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

        if (Input.GetKeyDown(KeyCode.LeftShift) && skills.dash.dashUnlock)  //���м���cd ui ����ȴʱ��İ�
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

    private void UPdateSoulsUI()   //��������൱�� �õ�Ǯ֮����ֱ�Ӵ� 100�� ����200 �� ������ʱ�������ӵ��������ڵĻ������
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

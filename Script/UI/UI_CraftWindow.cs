using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Button craftButton;


    [SerializeField] private Image[] materialImage;

    public void SetupCraftWindow(ItemData_Equipment _data)  //制作
    {

        craftButton.onClick.RemoveAllListeners(); //移除所以监听者，事件相关内容

        for (int i = 0; i < materialImage.Length; i++)
        {
            materialImage[i].color = Color.clear;
            materialImage[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.clear;
        }
        for (int i = 0; i < _data.craftingMaterials.Count; i++)   
        {
            if (_data.craftingMaterials.Count > materialImage.Length)
                Debug.Log("You have more materials amount than you hanmaterial slots in craft window");


            materialImage[i].sprite = _data.craftingMaterials[i].data.icon;
            materialImage[i].color = Color.white;

            TextMeshProUGUI materialSlotText = materialImage[i].GetComponentInChildren<TextMeshProUGUI>();

            materialSlotText.text = _data.craftingMaterials[i].stackSize.ToString();
            materialSlotText.color = Color.white;
        }

        
        itemIcon.sprite = _data.icon;                            //更新图标
        itemName.text = _data.itemName;
        itemDescription.text = _data.GetDescription();

        craftButton.onClick.AddListener(() => Inventory.instance.CanCraft(_data, _data.craftingMaterials));  //哇哦！！！***
    }
}

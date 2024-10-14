using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{

    private ItemObject myItemObject => GetComponentInParent<ItemObject>();  

    private void OnTriggerEnter2D(Collider2D other)  //������������Ʒ
    {
        if (other.GetComponent<Player>() != null)   //�ж�������other�����
        {
            if (other.GetComponent<CharacterStats>().isDead)  //�����������������Ʒbug
                return;

            Debug.Log("Picked up item");
            myItemObject.PickUpItem();
        }
    }

}

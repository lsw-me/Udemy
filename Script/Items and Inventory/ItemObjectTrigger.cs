using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{

    private ItemObject myItemObject => GetComponentInParent<ItemObject>();  

    private void OnTriggerEnter2D(Collider2D other)  //用来捡起来物品
    {
        if (other.GetComponent<Player>() != null)   //判断碰到的other是玩家
        {
            if (other.GetComponent<CharacterStats>().isDead)  //解决人物死亡捡起物品bug
                return;

            Debug.Log("Picked up item");
            myItemObject.PickUpItem();
        }
    }

}

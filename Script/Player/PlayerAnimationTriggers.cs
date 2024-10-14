using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{

    private Player player => GetComponentInParent<Player>();
    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        //������������ attackCheck Բ�п��ܻ��кܶ࣬����ȫ��ע��

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (Collider2D hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();
                if (_target != null)                           // p117 ���bug
                    player.stats.DoDamage(_target);


            /*    //hit.GetComponent<Enemy>().Damage();  //��������Ĺ�����hit�ܻ�ȡ����������������˺����� p90 ���� ���е�damage()������Ҫ�ع�
                //hit.GetComponent<CharacterStats>().TakeDamage(player.stats.damage.GetValue()); // p89 �����˺�  �����ع�Ϊһ������


                //ִ���߼� inventory  �õ�������ִ��itemЧ��
                //Inventory.instance.GetEquipment(EquipmentType.Wepon).Effect(_target.transform);  //bug ��װ����Ҳ����ã�����һ������ û��ʵ�����ı���
            */   
                
                
                ItemData_Equipment weponData = Inventory.instance.GetEquipment(EquipmentType.Wepon);  //����ϱ�bug
                if (weponData != null)
                {
                    weponData.Effect(_target.transform);
                }

            }
        }
    }





    private void ThrowSword()
    {
        SkillManager.instance.sword.CreatSword();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Ice and Fire effect", menuName = "Data/Item effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;

    [SerializeField] private float xVelocity;  //�޸��ٶ�ֻ����һ������ ������ٶ�ƥ�䲻�ϵ� ���⣨���� <-  ���������ܣ�

    public override void ExecuteEffect(Transform _respawnPosition)
    {
        Player player  =PlayerManager.instance.player;

        bool thirdAttack = player.primaryAttack.comboCounter == 2;  //�����������һ�ι�������effect

        if (thirdAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab,_respawnPosition.position, player.transform.rotation);

            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(player.facingDir * xVelocity, 0);

            Destroy(newIceAndFire, 10f);
        }

    }

}
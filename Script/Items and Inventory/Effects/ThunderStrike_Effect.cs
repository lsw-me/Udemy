using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item effect/Thunder strike")]
public class ThunderStrike_Effect : ItemEffect
{


    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExecuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = Instantiate(thunderStrikePrefab,_enemyPosition.position,Quaternion.identity);

        //set up new  thunder strike 

        Destroy(newThunderStrike,.5f);

    }
}

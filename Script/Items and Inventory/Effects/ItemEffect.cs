using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect")]   后续因为不同效果分别 继承独立写，所以不会创建这个父类
public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)  //执行效果
    {
        Debug.Log("Effect executed");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item effect")]   ������Ϊ��ͬЧ���ֱ� �̳ж���д�����Բ��ᴴ���������
public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;

    public virtual void ExecuteEffect(Transform _enemyPosition)  //ִ��Ч��
    {
        Debug.Log("Effect executed");
    }
}

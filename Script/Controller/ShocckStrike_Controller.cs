using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShocckStrike_Controller : MonoBehaviour
{
    [SerializeField]private CharacterStats targetStats;
    [SerializeField] private float speed;
    private int damage;

    private Animator anim;
    private bool triggered;


    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int _damage,CharacterStats _targetStats  )
    {
        damage = _damage;
        targetStats = _targetStats;
    }
    private void Update()
    {
        if (!targetStats)
            return;

        if(triggered)
            return;
        transform.position = Vector2.MoveTowards(transform.position, targetStats.transform.position, speed * Time.deltaTime);   //�׵��ƶ�
        transform.right = transform.position - targetStats.transform.position;                       //ת����

        if(Vector2.Distance(transform.position,targetStats.transform.position) <.1f)
        {

            anim.transform.localPosition = new Vector3(0, .5f);
            anim.transform.localRotation = Quaternion.identity;


            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);






            Invoke("DamageAndSelfDestory", .2f);
            triggered = true;
            anim.SetTrigger("Hit");
            
        }
    }


    private void DamageAndSelfDestory()
    {
        targetStats.ApplyShock(true);
        targetStats.TakeDamage(damage);
        Destroy(gameObject, .4f);
    }



}

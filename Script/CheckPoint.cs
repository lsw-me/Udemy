using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private Animator anim;

    public string Id;
    public bool activationStatus;


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        //anim = GetComponent<Animator>();
    }
    void Update()
    {
        
    }

    [ContextMenu("Generate checkpoint id")]
    private  void GenerateId() //����id ÿ�ε��ö����µ����Ե���һ��
    {
        Id = System.Guid.NewGuid().ToString();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        activationStatus = true;
        anim.SetBool("active", true);
    }
}

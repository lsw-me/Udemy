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
    private  void GenerateId() //产生id 每次调用都有新的所以调用一次
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float parallaxEffect; //视觉差效应

    private float xPosition;
    private float length;


    void Start()
    {
        cam = GameObject.Find("Main Camera");


        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x; 
    }

    // Update is called once per frame
    void Update()
    {

        float distanceMove = cam.transform.position.x * (1 - parallaxEffect);  //人物移动距离

        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);


        if(distanceMove >xPosition +length)   // 人物移动距离超出图层 ，把图层瞬时移动
            xPosition = xPosition +length;
        else if (distanceMove <xPosition -length)
            xPosition = xPosition -length;
    }
}

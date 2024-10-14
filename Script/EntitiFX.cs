using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitiFX : MonoBehaviour
{
    // ���е��Ӿ�Ч��  FX����Effects��д����ʾ��Ч

    private SpriteRenderer sr;

    [Header("after image fx")]
    [SerializeField] private GameObject afterImagePrefab;
    [SerializeField] private float colorLooseRate;
    [SerializeField] private float afterImageCooldown;
    private float afterImageCooldownTimer;


    [Header("Flash FX")]
    [SerializeField] private float flashDuration;
    [SerializeField] private Material hitMat;
    private Material origanlMat;


    [Header("Ailment  colors ")]  //����Ч����ɫ�����䣬ȼ������

    [SerializeField] private Color[] igniteColor; //������ʵ�����ֲ�ͬ��ɫ�л�������ʹ������
    [SerializeField] private Color[] chillColor;
    [SerializeField] private Color[] shockColor;
    [Header("Allment particles")]
    [SerializeField] private ParticleSystem igniteFX;
    [SerializeField] private ParticleSystem chillFX;//������Ч
    [SerializeField] private ParticleSystem shockFX;
    
    [Header("Hit fx")]
    [SerializeField] private GameObject hitFx;
    [SerializeField] private GameObject criticalhHtFx;
    [Space]
    [SerializeField] private ParticleSystem dustFX;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        origanlMat = sr.material;
    }

    private void Update()
    {
        afterImageCooldownTimer -= Time.deltaTime;
    }

    public void CreateAfterImage()
    {
        if(afterImageCooldownTimer < 0)
        {
            afterImageCooldownTimer = afterImageCooldown;
            GameObject  newAfterImage = Instantiate(afterImagePrefab,transform.position ,transform.rotation);
            newAfterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate,sr.sprite);
        }
    }


    public void MakeTransprent(bool _transprent)  //�ڶ�����ʱ����� ͬʱ����Ϊ����Gameobject���� player��͸��
    {
        if (_transprent)
            sr.color = Color.clear;
        else
            sr.color = Color.white;
    }
    private IEnumerator FlashFX()
    {
        sr.material = hitMat;
        Color currentColor = sr.color;   // ����Ϊ�˱����ܻ�ʱ���׹��ȼ��״̬ ��ɫ�л���Ӱ��  �����˴�*���Ĳ����������ܻ���Ȼ��ʾ������ɫ����ɫ

        sr.color = Color.white;  // *

        yield return new WaitForSeconds(flashDuration);

        sr.color = currentColor; //*

        sr.material = origanlMat;
    }

    private void RedColorBlink()  //��׺����
    {
        if(sr.color !=Color.white)
            sr.color = Color.white;
        else 
            sr.color = Color.red;
    }

    private void CancelColorChange()
    {
        CancelInvoke();
        sr.color = Color.white ;

        igniteFX.Stop();//ֹͣ��Ч
        chillFX.Stop();
        shockFX.Stop();
    }


    public void IgniteFxFor(float _seconds)             //****ȥѧϰ���
    {
        igniteFX.Play();

        InvokeRepeating("IgniteColorFx",0,.3f);
        Invoke("CancelColorChange", _seconds);
    }

    public void ChillFxFor(float _seconds)             //****ȥѧϰ���
    {
        chillFX.Play();
        InvokeRepeating("ChillColorFx", 0, .3f);                 //���ﲻ��Ҫ����ΪChill����������Ϊ��һ��  �ָ��� ��Ϊbug ����ɶbug
        Invoke("CancelColorChange", _seconds);
    }
    public void ShockFxFor(float _seconds)             //****ȥѧϰ���
    {
        shockFX.Play();
        InvokeRepeating("ShockColorFx", 0, .3f);
        Invoke("CancelColorChange", _seconds);
    }
    private void IgniteColorFx()
    {
        if (sr.color != igniteColor[0])
            sr.color = igniteColor[0];
        else
            sr.color = igniteColor[1];
    }
    private void ChillColorFx()
    {
        if (sr.color != chillColor[0])
            sr.color = chillColor[0];
        else
            sr.color = chillColor[1];
    }

    private void ShockColorFx() 
    {
        if (sr.color != shockColor[0])
            sr.color = shockColor[0];
        else
            sr.color = shockColor[1];
    }


    public void CreateHitFx(Transform _target,bool _critical)
    {

        float zRotation = Random.Range(-90, 90); //�����ת hitprefab
        float xPosition = Random.Range(-.5f, .5f);
        float yPosition = Random.Range(-.5f, .5f);

        Vector3 hitFxRotation = new Vector3(0, 0, zRotation);//default

        GameObject hitPrefab =hitFx;

        if(_critical)
        {
            hitPrefab = criticalhHtFx;

            float yRotation = 0;
            zRotation = Random.Range(-45, 45);

            if (GetComponent<Entity>().facingDir == -1)
                yRotation = 180;

            hitFxRotation = new Vector3(0, yRotation, zRotation);
        }
        GameObject newHitFX = Instantiate(hitPrefab, _target.position + new Vector3(xPosition, yPosition),Quaternion.identity);

        
        newHitFX.transform.Rotate(hitFxRotation);
  

        Destroy(newHitFX,.5f);
    }

    public void PlayDustFx()
    {
        if(dustFX != null)
            dustFX.Play();
    }

}

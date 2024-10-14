using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;


    public bool playBgm;
    private int bgmIndex;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Update()
    {
        if (!playBgm)
            StopAllBgm();
        else
        {
            if (!bgm[bgmIndex].isPlaying)
            {
                PlayBGM(bgmIndex);
            }
        }
    }

    public void PlaySFX(int _sfxIndex)
    {
        if(_sfxIndex <sfx.Length)
        {
            sfx[_sfxIndex].pitch = Random.Range(0.85f, 1.1f);
            sfx[_sfxIndex].Play();
        }
    }

    public void PlayRandowBGM()
    {
        bgmIndex = Random.Range(0, bgm.Length);
        PlayBGM(bgmIndex);
    }
    public void StopSFX(int _index) => sfx[_index].Stop();

    public void PlayBGM(int _index)
    {
        bgmIndex = _index;
        Debug.Log(bgmIndex);
        StopAllBgm();
        bgm[bgmIndex].Play();

    }
    public void StopAllBgm()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
}

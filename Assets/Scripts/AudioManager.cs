using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum RenshengAudio
{
    xiuzhi,
    bcgm,
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private void Awake()
    {
        instance = this;
        
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = bgm;
    }

    public AudioSource audioSource;
    public AudioSource miniAudioSource;
    public AudioClip bgm;
    public AudioClip xiuzhi;
    public AudioClip bcgm;

    public float preBGMVolume;

    public bool isPlayOther;
    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = 5;
        audioSource.Play();
    }

    public void AudioPlay(RenshengAudio m_audio)
    {
        switch (m_audio)
        {
            case RenshengAudio.xiuzhi:
                preBGMVolume = audioSource.volume;
                if (miniAudioSource.isPlaying)return;
                miniAudioSource.clip = xiuzhi;
                audioSource.volume = 0.1F;
                miniAudioSource.Play();
                isPlayOther = true;
                break;
            case RenshengAudio.bcgm:
                preBGMVolume = audioSource.volume;
                if (miniAudioSource.isPlaying)return;
                miniAudioSource.clip = bcgm;
                audioSource.volume = 0.1F;
                miniAudioSource.Play();
                isPlayOther = true;
                break;
        }
    }

    public void XiuzhiPlay()
    {
        AudioPlay(RenshengAudio.xiuzhi);
    }
    public void BcgmPlay()
    {
        AudioPlay(RenshengAudio.bcgm);
    }
    public void SwitchBGM()
    {
        if (!isPlayOther)
        {
            miniAudioSource.Stop();
        }
        if (isPlayOther)
        {
            isPlayOther = false;
            audioSource.volume = preBGMVolume;
            miniAudioSource.Stop();
        }
        
    }
}

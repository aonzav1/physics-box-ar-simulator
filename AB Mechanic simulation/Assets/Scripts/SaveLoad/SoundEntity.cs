using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEntity : MonoBehaviour
{
    public AudioSource source;
    public AudioClip[] SE;
    public bool auto_play;
    public int auto_num;
    public bool isLoop;
    public enum sound_type
    {
        se,music,voice
    }
    //public sound_type soundType = sound_type.se;
    void Start()
    {
        source.volume = SaveLoad.cur_setting.volume;
        if (auto_play)
        {
            if (!isLoop)
                Play(auto_num);
            else
                PlayLoop(auto_num);
        }
    }

    public void Play(int num)
    {
        source.PlayOneShot(SE[num]);
    }
    public void StopPlay()
    {
        source.Stop();
    }
    public void PlayLoop(int num)
    {
        source.clip = SE[num];
        source.loop = true;
        source.Play();
    }
    private void Update()
    {
        source.volume = SaveLoad.cur_setting.volume;
    }
}

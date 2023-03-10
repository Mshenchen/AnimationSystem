using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource bgMusic = null;
    private float bgValue = 1;
    private GameObject soundObj = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundValue = 1;

    public MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }

    private void Update()
    {
        for (int i = soundList.Count-1; i >= 0; i--)
        {
            if (!soundList[i].isPlaying)
            {
                GameObject.Destroy(soundList[i]);
                soundList.RemoveAt(i);
            }
        }
    }
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBGMusic(string name)
    {
        if (bgMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BgMusic";
            bgMusic = obj.AddComponent<AudioSource>();
        }
        ResMgr.Instance.LoadAsync<AudioClip>("Music/BG/"+name, (clip) =>
        {
            bgMusic.clip = clip;
            bgMusic.volume = bgValue;
            bgMusic.loop = true;
            bgMusic.Play();
        });
    }

    
    public void PauseBGMusic()
    {
        if(bgMusic==null)
            return;
        bgMusic.Pause();
    }

    
    public void StopBGMusic()
    {
        if(bgMusic==null)
            return;
        bgMusic.Stop();
    }
    public void ChangeBGValue(float v)
    {
        bgValue = v;
        if(bgMusic==null)
            return;
        bgMusic.volume = bgValue;
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name,bool isLoop,UnityAction<AudioSource> callBack = null)
    {
        if (soundObj == null)
        {
            soundObj = new GameObject();
            soundObj.name = "Sound";
        }

        ResMgr.Instance.LoadAsync<AudioClip>("Music/Sound/"+name, (clip) =>
        {
            AudioSource source = soundObj.AddComponent<AudioSource>();
            bgMusic.clip = clip;
            source.loop = isLoop;
            bgMusic.volume = soundValue;
            bgMusic.Play();
            soundList.Add(source);
            if (callBack != null)
                callBack(source);
        });
    }
    /// <summary>
    /// 改变音效声音大小
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundValue(float value)
    {
        soundValue = value;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = value;
        }
    }
    public void StopSound(AudioSource source)
    {
        if (soundList.Contains(source))
        {
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }
}

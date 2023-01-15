using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClip
{
    public int clipId = 0;
    public ESoundPlayType playType = ESoundPlayType.None;
    public string clipName = string.Empty;
    public string clipPath = string.Empty;
    public float maxVolume = 1.0f;
    public bool isLoop = false;
    public float[] checkTime = new float[0];
    public float[] setTime = new float[0];

    private AudioClip clip = null;
    public int currentLoop = 0;
    public float pitch = 1.0f;
    public float dopplerLevel = 1.0f;
    public AudioRolloffMode rollOffMode = AudioRolloffMode.Logarithmic;
    public float minDist = 10000.0f;
    public float maxDist = 50000.0f;
    public float spatialBlend = 1.0f;

    public float fadeTime1 = 0.0f;
    public float fadeTime2 = 0.0f;
    public Interpolate.Function interpolateFunc;
    public bool isFadeIn = false;
    public bool isFadeOut = false;

    public bool HasLoop { get { return checkTime.Length > 0; } }

    public SoundClip() { }
    public SoundClip(string p_clipPath, string p_clipName)
    {
        clipPath = p_clipPath;
        clipName = p_clipName;
    }

    public void PreLoad()
    {
        if (clip == null)
        {
            string t_fullPath = clipPath + clipName;
            clip = ResourceManager.Load(t_fullPath) as AudioClip;
        }
    }

    public void AddLoop()
    {
        checkTime = ArrayHelper.Add(0.0f, checkTime);
        setTime = ArrayHelper.Add(0.0f, setTime);
    }

    public void RemoveLoop(int p_idx)
    {
        checkTime = ArrayHelper.Remove(p_idx, checkTime);
        setTime = ArrayHelper.Remove(p_idx, setTime);
    }

    public AudioClip GetClip()
    {
        if (clip == null) PreLoad();
        if (clip == null && clipName != string.Empty)
        {
            Debug.LogWarning($"Can not load audio clip resources {clipName}");
            return null;
        }
        return clip;
    }

    public void ReleaseClip()
    {
        if (clip == null) return;

        clip = null;
    }

    public void NextLoop()
    {
        currentLoop++;
        if (currentLoop >= checkTime.Length) currentLoop = 0;
    }

    public void CheckLoop(AudioSource p_source)
    {
        if (HasLoop && p_source.time >= checkTime[currentLoop])
        {
            p_source.time = setTime[currentLoop];
            NextLoop();
        }
    }

    public void FadeIn(float p_time, Interpolate.EaseType p_easeType)
    {
        isFadeIn = true;
        isFadeOut = false;
        fadeTime1 = 0.0f;
        fadeTime2 = p_time;
        interpolateFunc = Interpolate.Ease(p_easeType);
    }

    public void FadeOut(float p_time, Interpolate.EaseType p_easeType)
    {
        isFadeIn = false;
        isFadeOut = true;
        fadeTime1 = 0.0f;
        fadeTime2 = p_time;
        interpolateFunc = Interpolate.Ease(p_easeType);
    }

    public void DoFade(float p_time, AudioSource p_audio)
    {
        if (isFadeIn)
        {
            fadeTime1 += p_time;
            p_audio.volume = Interpolate.Ease(interpolateFunc, 0, maxVolume, fadeTime1, fadeTime2);
            if (fadeTime1 >= fadeTime2) isFadeIn = false;
        }
        else if (isFadeOut)
        {
            fadeTime1 += p_time;
            p_audio.volume = Interpolate.Ease(interpolateFunc, maxVolume, 0, fadeTime1, fadeTime2);
            if (fadeTime1 >= fadeTime2)
            {
                isFadeOut = false;
                p_audio.Stop();
            }
        }
    }
}

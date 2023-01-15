using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingletonMonobehaviour<SoundManager>
{
    public const string masterGroupName = "Master";
    public const string effectGroupName = "Effect";
    public const string bgmGroupName = "BGM";
    public const string uiGroupName = "UI";
    public const string mixerName = "AudioMixer";
    public const string containerName = "SoundContainer";
    public const string fadeA = "FadeA";
    public const string fadeB = "FadeB";
    public const string ui = "UI";
    public const string effectVolumeParam = "Volume_Effect";
    public const string bgmVolumeParam = "Volume_BGM";
    public const string uiVolumeParam = "Volume_UI";

    public enum EMusicPlayingType
    { 
        None = 0,
        SourceA = 1,
        SourceB = 2,
        AtoB = 3,
        BtoA = 4,
    }

    public AudioMixer mixer = null;
    public Transform audioRoot = null;
    public AudioSource fadeAudioA = null;
    public AudioSource fadeAudioB = null;
    public AudioSource[] effectAudios = null;
    public AudioSource uiAudio = null;

    public float[] effectPlayStartTime = null;
    private int numEffectChannel = 5;
    private EMusicPlayingType curPlayingType = EMusicPlayingType.None;
    private bool isTicking = false;
    private SoundClip curSound = null;
    private SoundClip lastSound = null;
    private float minVolume = -80.0f;
    private float maxVolume = 0.0f;

    public float BGMVolume
    {
        set
        {
            float t_volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(value));
            mixer.SetFloat(bgmVolumeParam, t_volume);
            PlayerPrefs.SetFloat(bgmVolumeParam, t_volume);
        }
        get 
        {
            if (PlayerPrefs.HasKey(bgmVolumeParam)) return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(bgmVolumeParam));

            return maxVolume;
        }
    }

    public float EffectVolume
    {
        set
        {
            float t_volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(value));
            mixer.SetFloat(effectVolumeParam, t_volume);
            PlayerPrefs.SetFloat(effectVolumeParam, t_volume);
        }
        get
        {
            if (PlayerPrefs.HasKey(effectVolumeParam)) return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(effectVolumeParam));

            return maxVolume;
        }
    }

    public float UIVolume
    {
        set
        {
            float t_volume = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(value));
            mixer.SetFloat(uiVolumeParam, t_volume);
            PlayerPrefs.SetFloat(uiVolumeParam, t_volume);
        }
        get
        {
            if (PlayerPrefs.HasKey(uiVolumeParam)) return Mathf.Lerp(minVolume, maxVolume, PlayerPrefs.GetFloat(uiVolumeParam));

            return maxVolume;
        }
    }

    public bool IsPlaying { get { return (int)curPlayingType > 0; } }
    
    private void Start()
    {
        if (mixer == null) mixer = Resources.Load(mixerName) as AudioMixer;
        if (audioRoot == null)
        {
            audioRoot = new GameObject(containerName).transform;
            audioRoot.SetParent(transform);
            audioRoot.localPosition = Vector3.zero;
        }
        if (fadeAudioA == null)
        {
            GameObject t_fadeA = new GameObject(fadeA, typeof(AudioSource));
            t_fadeA.transform.SetParent(audioRoot);
            fadeAudioA = t_fadeA.GetComponent<AudioSource>();
            fadeAudioA.playOnAwake = false;
        }
        if (fadeAudioB == null)
        {
            GameObject t_fadeB = new GameObject(fadeB, typeof(AudioSource));
            t_fadeB.transform.SetParent(audioRoot);
            fadeAudioB = t_fadeB.GetComponent<AudioSource>();
            fadeAudioB.playOnAwake = false;
        }
        if (uiAudio == null)
        {
            GameObject t_ui = new GameObject(ui, typeof(AudioSource));
            t_ui.transform.SetParent(audioRoot);
            uiAudio = t_ui.GetComponent<AudioSource>();
            uiAudio.playOnAwake = false;
        }
        if (effectAudios == null || effectAudios.Length <= 0)
        {
            effectPlayStartTime = new float[numEffectChannel];
            effectAudios = new AudioSource[numEffectChannel];

            for (int i = 0; i < numEffectChannel; i++)
            {
                effectPlayStartTime[i] = 0.0f;
                GameObject t_effectAudio = new GameObject("Effect " + i.ToString(), typeof(AudioSource));
                t_effectAudio.transform.SetParent(audioRoot);
                effectAudios[i] = t_effectAudio.GetComponent<AudioSource>();
                effectAudios[i].playOnAwake = false;
            }
        }
        if (mixer != null)
        {
            fadeAudioA.outputAudioMixerGroup = mixer.FindMatchingGroups(bgmGroupName)[0];
            fadeAudioB.outputAudioMixerGroup = mixer.FindMatchingGroups(bgmGroupName)[0];
            
            uiAudio.outputAudioMixerGroup = mixer.FindMatchingGroups(uiGroupName)[0];
        
            for (int i = 0; i < effectAudios.Length; i++)
                effectAudios[i].outputAudioMixerGroup = mixer.FindMatchingGroups(effectGroupName)[0];
        }

        InitVolume();
    }

    private void Update()
    {
        if (curSound == null) return;

        if (curPlayingType == EMusicPlayingType.SourceA)
            curSound.DoFade(Time.deltaTime, fadeAudioA);
        else if (curPlayingType == EMusicPlayingType.SourceB)
            curSound.DoFade(Time.deltaTime, fadeAudioB);
        else if (curPlayingType == EMusicPlayingType.AtoB)
        {
            lastSound.DoFade(Time.deltaTime, fadeAudioA);
            curSound.DoFade(Time.deltaTime, fadeAudioB);
        }
        else if (curPlayingType == EMusicPlayingType.BtoA)
        {
            lastSound.DoFade(Time.deltaTime, fadeAudioB);
            curSound.DoFade(Time.deltaTime, fadeAudioA);
        }

        if (fadeAudioA.isPlaying && !fadeAudioB.isPlaying)
            curPlayingType = EMusicPlayingType.SourceA;
        else if (fadeAudioB.isPlaying && !fadeAudioA.isPlaying)
            curPlayingType = EMusicPlayingType.SourceB;
        else if (!fadeAudioA.isPlaying && !fadeAudioB.isPlaying)
            curPlayingType = EMusicPlayingType.None;
    }

    private void InitVolume()
    {
        if (mixer != null) return;

        mixer.SetFloat(bgmVolumeParam, BGMVolume);
        mixer.SetFloat(effectVolumeParam, EffectVolume);
        mixer.SetFloat(uiVolumeParam, UIVolume);
    }

    private void PlayAudioSource(AudioSource p_source, SoundClip p_clip, float p_volume)
    {
        if (p_source == null || p_clip == null) return;

        p_source.Stop();

        p_source.clip = p_clip.GetClip();
        p_source.volume = p_volume;
        p_source.loop = p_clip.isLoop;
        p_source.pitch = p_clip.pitch;
        p_source.dopplerLevel = p_clip.dopplerLevel;
        p_source.rolloffMode = p_clip.rollOffMode;
        p_source.minDistance = p_clip.minDist;
        p_source.maxDistance = p_clip.maxDist;
        p_source.spatialBlend = p_clip.spatialBlend;

        p_source.Play();
    }

    private void PlayAudioSourceAtPoint(SoundClip p_clip, Vector3 p_pos, float p_volume)
    {
        AudioSource.PlayClipAtPoint(p_clip.GetClip(), p_pos, p_volume);
    }

    public bool CheckDifferentSound(SoundClip p_clip)
    {
        if (p_clip == null) return false;
        if (curSound != null && curSound.clipId == p_clip.clipId && IsPlaying && !curSound.isFadeOut) return false;
        return true;
    }

    public void DoCheck() => StartCoroutine(CheckProcess());

    private IEnumerator CheckProcess()
    {
        while (isTicking && IsPlaying)
        {
            yield return new WaitForSeconds(0.05f);
            if (curSound.HasLoop)
            {
                if (curPlayingType == EMusicPlayingType.SourceA)
                    curSound.CheckLoop(fadeAudioA);
                else if (curPlayingType == EMusicPlayingType.SourceB)
                    curSound.CheckLoop(fadeAudioB);
                else if (curPlayingType == EMusicPlayingType.AtoB)
                {
                    lastSound.CheckLoop(fadeAudioA);
                    curSound.CheckLoop(fadeAudioB);
                }
                else if (curPlayingType == EMusicPlayingType.BtoA)
                {
                    lastSound.CheckLoop(fadeAudioB);
                    curSound.CheckLoop(fadeAudioA);
                }
            }
        }
    }

    public void FadeIn(SoundClip p_clip, float p_time, Interpolate.EaseType p_easeType)
    {
        if (CheckDifferentSound(p_clip))
        {
            fadeAudioA.Stop();
            fadeAudioB.Stop();
            lastSound = curSound;
            curSound = p_clip;
            
            PlayAudioSource(fadeAudioA, curSound, 0.0f);
            
            curSound.FadeIn(p_time, p_easeType);
            curPlayingType = EMusicPlayingType.SourceA;

            if (curSound.HasLoop)
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeIn(int p_idx, float p_time, Interpolate.EaseType p_easeType) => FadeIn(DataManager.SoundData.GetCopyClip(p_idx), p_time, p_easeType);

    public void FadeOut(float p_time, Interpolate.EaseType p_easeType)
    {
        if (curSound == null) return;

        curSound.FadeOut(p_time, p_easeType);
    }

    public void FadeTo(SoundClip p_clip, float p_time, Interpolate.EaseType p_ease)
    {
        if (curPlayingType == EMusicPlayingType.None) FadeIn(p_clip, p_time, p_ease);
        else if (CheckDifferentSound(p_clip))
        {
            if (curPlayingType == EMusicPlayingType.AtoB)
            {
                fadeAudioA.Stop();
                curPlayingType = EMusicPlayingType.SourceB;
            }
            else if (curPlayingType == EMusicPlayingType.BtoA)
            {
                fadeAudioB.Stop();
                curPlayingType = EMusicPlayingType.SourceA;
            }

            lastSound = curSound;
            curSound = p_clip;

            lastSound.FadeOut(p_time, p_ease);
            curSound.FadeIn(p_time, p_ease);

            if (curPlayingType == EMusicPlayingType.SourceA)
            {
                PlayAudioSource(fadeAudioB, curSound, 0.0f);
                curPlayingType = EMusicPlayingType.AtoB;
            }
            else if (curPlayingType == EMusicPlayingType.SourceB)
            {
                PlayAudioSource(fadeAudioA, curSound, 0.0f);
                curPlayingType = EMusicPlayingType.BtoA;
            }

            if (curSound.HasLoop)
            {
                isTicking = true;
                DoCheck();
            }
        }
    }

    public void FadeTo(int p_idx, float p_time, Interpolate.EaseType p_ease) => FadeTo(DataManager.SoundData.GetCopyClip(p_idx), p_time, p_ease);

    public void PlayBGM(SoundClip p_clip)
    {
        if (!CheckDifferentSound(p_clip)) return;

        fadeAudioB.Stop();
        lastSound = curSound;
        curSound = p_clip;

        PlayAudioSource(fadeAudioA, p_clip, p_clip.maxVolume);

        if (curSound.HasLoop)
        {
            isTicking = true;
            DoCheck();
        }
    }

    public void PlayBGM(int p_idx) => PlayBGM(DataManager.SoundData.GetCopyClip(p_idx));

    public void PlayUISound(SoundClip p_clip) => PlayAudioSource(uiAudio, p_clip, p_clip.maxVolume);
    public void PlayEffectSound(SoundClip p_clip)
    {
        for (int i = 0; i < numEffectChannel; i++)
        {
            if (!effectAudios[i].isPlaying)
            {
                PlayAudioSource(effectAudios[i], p_clip, p_clip.maxVolume);
                effectPlayStartTime[i] = Time.realtimeSinceStartup;
                return;
            }
            else if (effectAudios[i].clip == p_clip.GetClip())
            {
                effectAudios[i].Stop();
                PlayAudioSource(effectAudios[i], p_clip, p_clip.maxVolume);
                effectPlayStartTime[i] = Time.realtimeSinceStartup;
                return;
            }
        }

        float t_maxTime = 0.0f;
        int t_selectIdx = 0;

        for (int i = 0; i < numEffectChannel; i++)
        {
            if (effectPlayStartTime[i] > t_maxTime)
            {
                t_maxTime = effectPlayStartTime[i];
                t_selectIdx = i;
            }
        }

        PlayAudioSource(effectAudios[t_selectIdx], p_clip, p_clip.maxVolume);
    }

    public void PlayEffectSound(SoundClip p_clip, Vector3 p_pos, float p_volume)
    {
        for (int i = 0; i < numEffectChannel; i++)
        {
            if (!effectAudios[i].isPlaying)
            {
                PlayAudioSourceAtPoint(p_clip, p_pos, p_clip.maxVolume);
                effectPlayStartTime[i] = Time.realtimeSinceStartup;
                return;
            }
            else if (effectAudios[i].clip == p_clip.GetClip())
            {
                effectAudios[i].Stop();
                PlayAudioSourceAtPoint(p_clip, p_pos, p_clip.maxVolume);
                effectPlayStartTime[i] = Time.realtimeSinceStartup;
                return;
            }
        }

        PlayAudioSourceAtPoint(p_clip, p_pos, p_clip.maxVolume);
    }

    public void PlayOneShotEffect(int p_idx, Vector3 p_pos, float p_volume)
    {
        if (p_idx == (int)SoundList.None) return;

        SoundClip t_clip = DataManager.SoundData.GetCopyClip(p_idx);
        
        if (t_clip == null) return;

        PlayEffectSound(t_clip, p_pos, p_volume);
    }

    public void PlayOnShot(SoundClip p_clip)
    {
        if (p_clip == null) return;

        switch (p_clip.playType)
        {
            case ESoundPlayType.EFFECT:
                PlayEffectSound(p_clip);
                break;
            case ESoundPlayType.BGM:
                PlayBGM(p_clip);
                break;
            case ESoundPlayType.UI:
                PlayUISound(p_clip);
                break;
        }
    }

    public void Stop(bool p_isAllStop = false)
    {
        if (p_isAllStop)
        {
            fadeAudioA.Stop();
            fadeAudioB.Stop();
        }

        FadeOut(0.5f, Interpolate.EaseType.Linear);
        curPlayingType = EMusicPlayingType.None;
        StopAllCoroutines();
    }

    public void PlayShotSound(string p_classID, Vector3 p_pos, float p_volume)
    {
        SoundList t_sound = (SoundList)Enum.Parse(typeof(SoundList), p_classID.ToLower());
        PlayOneShotEffect((int)t_sound, p_pos, p_volume);
    }
}

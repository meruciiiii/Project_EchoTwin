using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum SoundType
{
    // BGM 계열
    BGM_Main,
    BGM_Boss,

    // SFX 계열
    SFX_Dash,
    SFX_MonsterHit,
    SFX_PlayerHit,
    SFX_Coin,
    SFX_Heart,
    SFX_Crystal,
    SFX_Portal,
    SFX_Chest,
    SFX_UI_Interaction,
    SFX_UI_Cloud,

    SFX_SwordAttack1,
    SFX_SwordAttack2,
    SFX_DaggerAttack1,
    SFX_DaggerAttack2,
    SFX_DaggerThrow,
    SFX_HammerAttack1,
    SFX_HammerAttack2,
    SFX_AxeAttack1,
    SFX_AxeAttack2,
    SFX_SpearAttack1,
    SFX_SpearAttack2,

    SFX_Mushroom,
    SFX_Rat,
    SFX_Goblin,
    SFX_Slime,
    SFX_Skeleton,
    SFX_Crab,
    SFX_FlyingEye,
    SFX_Bat,
    SFX_Skul,
    SFX_Golem1,
    SFX_Golem2,
    SFX_Bringer1,
    SFX_Bringer2,
    SFX_Sentinel1,
    SFX_Sentinel2,
    SFX_Sentinel3,
    SFX_RedDragon1,
    SFX_RedDragon2,
    SFX_RedDragon3,

    SFX_MonsterDie,
    SFX_PlayerDie,

}

public class SoundManager : MonoBehaviour
{
    public static Action<SoundType> OnPlaySound;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [System.Serializable]
    public struct SoundData
    {
        public SoundType type;
        public AudioClip clip;
    }

    public List<SoundData> soundList;
    private Dictionary<SoundType, AudioClip> soundDict = new Dictionary<SoundType, AudioClip>();
    void Start()
    {
        SendEvent(SoundType.BGM_Main);
    }
    void Awake()
    {
        foreach (var data in soundList) soundDict[data.type] = data.clip;
        UpdateVolumes();
    }

    private void OnValidate()
    {
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    private void OnEnable() { OnPlaySound += PlaySound; }
    private void OnDisable() { OnPlaySound -= PlaySound; }

    private void PlaySound(SoundType type)
    {
        if (!soundDict.ContainsKey(type)) return;
        AudioClip clip = soundDict[type];

        if (type.ToString().StartsWith("BGM"))
        {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    public static void SendEvent(SoundType type) => OnPlaySound?.Invoke(type);
}
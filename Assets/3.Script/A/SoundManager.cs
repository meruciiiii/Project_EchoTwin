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
        [Range(0f, 1f)] public float volume;
    }

    public List<SoundData> soundList;
    private Dictionary<SoundType, SoundData> soundDict = new Dictionary<SoundType, SoundData>();
    
    void Start()
    {
        SendEvent(SoundType.BGM_Main);
    }
    void Awake()
    {
        soundDict.Clear();
        foreach (var data in soundList)
        {
            if (!soundDict.ContainsKey(data.type))
                soundDict[data.type] = data;
        }
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

        SoundData data = soundDict[type];
        if (data.clip == null) return;

        // 개별 볼륨이 0일 경우 소리가 안 들리므로, 기본적으로 1(100%)로 취급하게 방어 코드 추가
        float individualVol = data.volume <= 0.001f ? 1f : data.volume;

        if (type.ToString().StartsWith("BGM"))
        {
            bgmSource.clip = data.clip;
            bgmSource.volume = bgmVolume * individualVol;
            bgmSource.Play();
        }
        else
        {
            // 최종 볼륨 = 마스터 SFX 볼륨 * 해당 소리의 개별 볼륨
            sfxSource.PlayOneShot(data.clip, sfxVolume * individualVol);
        }
    }

    public static void SendEvent(SoundType type) => OnPlaySound?.Invoke(type);
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

public enum SoundType
{
    // BGM 계열
    BGM_Main, BGM_Boss,

    // SFX 계열
    SFX_Dash, SFX_MonsterHit, SFX_PlayerHit, SFX_Coin, SFX_Heart,
    SFX_Crystal, SFX_Portal, SFX_Chest, SFX_UI_Interaction, SFX_UI_Cloud,
    SFX_SwordAttack1, SFX_SwordAttack2, SFX_DaggerAttack1, SFX_DaggerAttack2, SFX_DaggerThrow,
    SFX_HammerAttack1, SFX_HammerAttack2, SFX_AxeAttack1, SFX_AxeAttack2, SFX_SpearAttack1, SFX_SpearAttack2,
    SFX_Mushroom, SFX_Rat, SFX_Goblin, SFX_Slime, SFX_Skeleton, SFX_Crab, SFX_FlyingEye,
    SFX_Bat, SFX_Skul, SFX_Golem1, SFX_Golem2, SFX_Bringer1, SFX_Bringer2,
    SFX_Sentinel1, SFX_Sentinel2, SFX_Sentinel3, SFX_RedDragon1, SFX_RedDragon2, SFX_RedDragon3,
    SFX_MonsterDie, SFX_PlayerDie,
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
    public struct SoundData // 반드시 클래스 내부에 있어야 데이터가 복구됩니다.
    {
        public SoundType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume; // 볼륨 필드만 추가
    }

    // [데이터 복구 핵심] 원본 이름인 soundList를 그대로 사용합니다.
    // 만약 제가 실수로 알려드린 sfxSettings로 이름을 바꿔서 저장하셨더라도 아래 코드가 데이터를 찾아옵니다.
    [FormerlySerializedAs("sfxSettings")]
    public List<SoundData> soundList;

    private Dictionary<SoundType, SoundData> soundDict = new Dictionary<SoundType, SoundData>();

    void Awake()
    {
        // 딕셔너리 구성 (개별 볼륨 데이터 포함)
        soundDict.Clear();
        foreach (var data in soundList)
        {
            if (!soundDict.ContainsKey(data.type))
                soundDict[data.type] = data;
        }
        UpdateVolumes();
    }

    void Start()
    {
        SendEvent(SoundType.BGM_Main);
    }

    private void OnValidate()
    {
        UpdateVolumes();
    }

    private void UpdateVolumes()
    {
        if (bgmSource != null) bgmSource.volume = bgmVolume;
    }

    private void OnEnable() { OnPlaySound += PlaySound; }
    private void OnDisable() { OnPlaySound -= PlaySound; }

    private void PlaySound(SoundType type)
    {
        if (!soundDict.ContainsKey(type)) return;
        SoundData data = soundDict[type];
        if (data.clip == null) return;

        if (type.ToString().StartsWith("BGM"))
        {
            bgmSource.clip = data.clip;
            // 개별 볼륨이 0일 경우를 대비해 1로 계산하는 방어 로직
            float individualVol = data.volume <= 0.001f ? 1f : data.volume;
            bgmSource.volume = bgmVolume * individualVol;
            bgmSource.Play();
        }
        else
        {
            // SFX 재생: 마스터 볼륨 * 개별 볼륨
            float individualVol = data.volume <= 0.001f ? 1f : data.volume;
            sfxSource.PlayOneShot(data.clip, sfxVolume * individualVol);
        }
    }

    public static void SendEvent(SoundType type) => OnPlaySound?.Invoke(type);
}
using System;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BubbleTransition,
    CharacterAttack,
    CharcterGetHit,
    CharacterCharge,
    CharacterDash,
    RangeCharacterAttack,
    RangeCharacterHit,
    MeleeCharacterAttack,
    MeleeCharacterHit,
    BubbleCapture,
}

[Serializable]
public class GameSound
{
    public SoundType key;
    public bool isMultiple;
    public List<AudioClip> clips;
    public AudioSource externalAudioSource;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Count == 0) return null;
        return clips[UnityEngine.Random.Range(0, clips.Count)];
    }
}

public class SoundManager : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private AudioSource mainAudioSource;

    [SerializeField] private List<GameSound> gameSounds = new();

    public static SoundManager Instance;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayOneShotSound(SoundType key)
    {
        var gameSound = gameSounds.Find(x => x.key == key);
        if (gameSound == null) return;

        AudioClip clip = gameSound.isMultiple ? gameSound.GetRandomClip() : gameSound.clips[0];
        if (clip == null) return;

        if (gameSound.externalAudioSource != null)
        {
            gameSound.externalAudioSource.PlayOneShot(clip);
        }
        else
        {
            mainAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayOneShotSound(SoundType key, float volume)
    {
        var gameSound = gameSounds.Find(x => x.key == key);
        if (gameSound == null) return;

        AudioClip clip = gameSound.isMultiple ? gameSound.GetRandomClip() : gameSound.clips[0];
        if (clip == null) return;

        if (gameSound.externalAudioSource != null)
        {
            if (gameSound.externalAudioSource.isPlaying) return;
            gameSound.externalAudioSource.PlayOneShot(clip, volume);
        }
        else
        {
            mainAudioSource.PlayOneShot(clip, volume);
        }
    }
}
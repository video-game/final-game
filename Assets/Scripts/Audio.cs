using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Audio
{
    public string name;

    public AudioClip clip;

    private AudioSource source;
    public AudioSource Source { get { return source; } }

    [Range(0f, 1f)]
    public float volume;

    [Range(0.3f, 3f)]
    public float pitch;

    public bool loop;

    public void Init(AudioSource s)
    {
        source = s;
        UpdateAudio();
    }

    public void UpdateAudio()
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
    }
}
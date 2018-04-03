using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonMB<AudioManager>
{
    public bool playOnStart;

    //lists to hold the audio/music clips
    public List<Audio> audioClip;
    public List<Audio> musicClip;

    //reference to current music playing (used to stop it)
    Audio musicPlaying;
    //reference to musicPlaying's position in the musicClip array
    private int musicPlayingIndex;

    //bools to set if the musicList or current music should loop;
    public bool loopMusicList;
    public bool loopCurrentMusic;

    private AudioSource MusicSource;

    private void Start()
    {
        MusicSource = gameObject.AddComponent<AudioSource>();

        if (playOnStart)
        {
            ToggleMusic();
        }
    }

    //reference to the playMusic coroutine function (used to stop it)
    Coroutine playMusicCoroutine = null;

    //a coroutine function that plays from the musicClip array.
    public IEnumerator PlayMusic(int index = 0)
    {
        //if index not outofbounds it's safe to run
        if (!(index > musicClip.Count - 1 || index < 0))
        {
            musicPlayingIndex = index;
            do
            {
                musicPlaying = musicClip[musicPlayingIndex];
                MusicSource.clip = musicPlaying.clip;
                MusicSource.volume = musicPlaying.volume;
                MusicSource.Play();
                //wait til the end of the clip
                yield return new WaitForSeconds(musicPlaying.clip.length);

                //if not looping current music, get next musicClip index
                if (!loopCurrentMusic)
                {
                    musicPlayingIndex = musicPlayingIndex == musicClip.Count - 1 ? 0 : musicPlayingIndex + 1;
                }
            }
            while (loopMusicList);
        }
    }

    //a function to stop the music playing, and the looping coroutine
    public void StopMusic()
    {
        MusicSource.Stop();
        if(playMusicCoroutine != null)
            StopCoroutine(playMusicCoroutine);
        playMusicCoroutine = null;
    }

    //a function to toggle the music on and off.
    public void ToggleMusic()
    {
        if (playMusicCoroutine != null)
        {
            StopMusic();
        }
        else
        {
            playMusicCoroutine = StartCoroutine(PlayMusic(musicPlayingIndex));
        }
    }

    //a function to get an audio clip by it's name.
    private Audio AudioClipByName(string name)
    {
        Audio a = Array.Find(audioClip.ToArray(), sound => sound.name == name);
        if (a == null)
        {
            Debug.LogWarning("Can't find sound with name: " + name);
        }
        return a;
    }

    //a function to get a music clip by it's name.
    private Audio MusicClipByName(string name)
    {
        Audio a = Array.Find(musicClip.ToArray(), sound => sound.name == name);
        if (a == null)
        {
            Debug.LogWarning("Can't find music with name: " + name);
        }
        return a;
    }

    //function that starts playing a clip by name
    public void PlayAudioClip(string name, float pitchVariancePercent = 0, PitchDirection pitchD = PitchDirection.Both)
    {
        Audio a = AudioClipByName(name);
        if(a == null)
        {
            return;
        }
        float pitchVariance = pitchVariancePercent != 0 ? (pitchVariancePercent / 100) : 0;
        StartCoroutine(PlayOneShot(a, pitchVariance, pitchD));
    }

    //function that starts playing music by name
    public void PlayMusicClip(string name)
    {
        Audio m = MusicClipByName(name);
        if (m == null)
        {
            return;
        }
        musicPlayingIndex = musicClip.IndexOf(m);
        StopMusic();
        ToggleMusic();
    }

    public IEnumerator PlayOneShot(Audio a, float pitchVariance, PitchDirection direction)
    {
        AudioSource temp = gameObject.AddComponent<AudioSource>();
        a.AttachAudio(temp);
        float maxPitchDifference = pitchVariance * temp.pitch;
        float variance = UnityEngine.Random.Range(direction != PitchDirection.Down ? -maxPitchDifference : 0, direction != PitchDirection.Up ? +maxPitchDifference : 0);
        temp.pitch = Mathf.Clamp(temp.pitch + variance, .3f, 3f);
        temp.Play();
        yield return new WaitForSeconds(a.clip.length);
        Destroy(temp);
    }

}

public enum PitchDirection
{
    Up, Down, Both
}

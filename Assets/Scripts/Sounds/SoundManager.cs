using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public bool loop;
    [Range(0, 256)]
    public int priority;
    [Range(0f, 1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;
    [Range(-1f, 1f)]
    public float stereoPan;
    [Range(0, 1f)]
    public float spatialBlend;
    public AudioSource audioSource;
}

public class SoundManager : MonoBehaviour
{
    [SerializeField] private Sound[] specialMusicList = new Sound[0];
    [SerializeField] private Sound[] bgMusicList = new Sound[0];
    [SerializeField] private Sound[] soundEffectList = new Sound[0];

    private Sound currentMusic;
    private Sound nextMusic;

    private Dictionary<string, Sound> specialMusicD = new Dictionary<string, Sound>();
    private List<Sound> bgMusicL = new List<Sound>();
    private Dictionary<string, Sound> soundEffectD = new Dictionary<string, Sound>();
    private float musicDelay = 0;
    static private SoundManager instance;

    private List<AudioSource> toUnPause = new List<AudioSource>();

    private void Awake()
    {
        instance = this;

        foreach (Sound s in specialMusicList)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            AudioSourceSetValues(temp, s);

            specialMusicD.Add(s.name, s);
        }

        foreach (Sound s in bgMusicList)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            AudioSourceSetValues(temp, s);

            bgMusicL.Add(s);
        }

        foreach (Sound s in soundEffectList)
        {
            AudioSource temp = gameObject.AddComponent<AudioSource>();
            AudioSourceSetValues(temp, s);

            soundEffectD.Add(s.name, s);
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            foreach (GameObject temp in GameObject.FindGameObjectsWithTag("UISound"))
            {
                if (!temp.transform.GetComponent<AudioSource>().isPlaying) Destroy(temp);
            }
        }
    }

    private void AudioSourceSetValues(AudioSource audioSource, Sound sound)
    {
        audioSource.clip = sound.clip;
        audioSource.loop = sound.loop;
        audioSource.priority = sound.priority;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.panStereo = sound.stereoPan;
        audioSource.spatialBlend = sound.spatialBlend;
        sound.audioSource = audioSource;
    }

    static public void PlayBGMusic(float delay = 0)
    {
        instance.musicDelay = delay;
        instance.nextMusic = instance.bgMusicL[Random.Range(0, instance.bgMusicL.Count)];
        instance.AudioSourceSetValues(instance.nextMusic.audioSource, instance.nextMusic);

        instance.ChangeMusic();
    }

    static public void PlaySpecialMusic(string musicName, float delay = 0)
    {
        instance.musicDelay = delay;
        instance.nextMusic = instance.specialMusicD[musicName];
        instance.AudioSourceSetValues(instance.nextMusic.audioSource, instance.nextMusic);

        instance.ChangeMusic();
    }

    private void ChangeMusic()
    {
        StartCoroutine("ChangeMusicCoroutine");
    }

    private IEnumerator ChangeMusicCoroutine()
    {
        if (currentMusic != null && currentMusic.audioSource.isPlaying)
        {
            float volume = currentMusic.audioSource.volume;
            while (currentMusic.audioSource.volume > 0)
            {
                volume -= Time.deltaTime;
                if (volume < 0) volume = 0;
                currentMusic.audioSource.volume = volume;
                yield return null;
            }
        }
        currentMusic = nextMusic;
        currentMusic.audioSource.Stop();
        currentMusic.audioSource.PlayDelayed(instance.musicDelay);
    }

    static public Sound GetSound(string name)
    {
        instance.soundEffectD.TryGetValue(name, out Sound temp);
        return temp;
    }

    static public void OneShotSound(AudioClip clip, Vector3 pos, float volume = 1f, float pitch = 1f,
        float stereoPan = 0f, float spatialBlend = 0f, int priority = 128, string tag = "")
    {
        GameObject tempSound = new GameObject("One Shot Sound");
        tempSound.transform.position = pos;
        if (tag != "") tempSound.tag = tag;

        AudioSource tempAS = tempSound.AddComponent<AudioSource>();
        tempAS.clip = clip;
        tempAS.volume = volume;
        tempAS.pitch = pitch;
        tempAS.panStereo = stereoPan;
        tempAS.spatialBlend = spatialBlend;
        tempAS.priority = priority;
        tempAS.Play();

        Destroy(tempSound, clip.length);
    }

    static public void PauseMusic(bool pause = true)
    {
        if (instance.currentMusic != null)
        {
            if (pause) instance.currentMusic.audioSource.Pause();
            else if (instance.currentMusic != null) instance.currentMusic.audioSource.UnPause();
        }
    }

    static public void PauseAllSounds(bool pause = true)
    {
        if (pause)
        {
            AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource s in allAudioSources)
            {
                if (s.isPlaying)
                {
                    s.Pause();
                    instance.toUnPause.Add(s);
                }
            }
        }
        else
        {
            foreach (AudioSource s in instance.toUnPause)
            {
                if (s) s.UnPause();
            }

            instance.toUnPause.Clear();
        }
    }
}

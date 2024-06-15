using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance
    {
        get
        {
            if (_instance is null)
                Debug.LogError("Audio manager is NULL");
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public List<Sound> musicSounds, sfxSounds, typingSounds;

    public AudioSource musicSource, sfxSource, typingSource;

    public bool StillSpeaking = false;


    public void PlayMusic(string name)
    {
        Sound s = musicSounds.Find(x => x.name == name);
        if(s != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound s = sfxSounds.Find(x => x.name == name);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void PlaySFXOnLoop()
    {
        StartCoroutine(PlaySFXOnLoopRoutine());
    }

    private IEnumerator PlaySFXOnLoopRoutine()
    {
        StillSpeaking = true;
        int r = Random.Range(0, 3);
        while (StillSpeaking)
        {
            typingSource.Stop();

            typingSource.clip = typingSounds[r].clip;

            //typingSource.PlayOneShot(typingSounds[r].clip);
            // yield return new WaitForSeconds(0.17f);
            typingSource.Play();
            while(typingSource.isPlaying)
            {
                yield return null;
            }
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] public AudioSource m_BackgroundMusicSource, m_effectSource;
    [SerializeField] List<AudioClip> m_audios;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgorundMusic()
    {
        m_BackgroundMusicSource.Play();
    }

    public void StopBackgorundMusic()
    {
        m_BackgroundMusicSource.Stop();
    }

    public void PlaySound(int i)
    {
        m_effectSource.PlayOneShot(m_audios[i]);
    }

    public void StopSound(int i)
    {
        m_effectSource.Stop();
    }
}

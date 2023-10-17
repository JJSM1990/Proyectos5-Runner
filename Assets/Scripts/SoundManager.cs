using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] public  AudioSource m_musicSource, m_effectSource;
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

    public void PlaySound(int i)
    {
        m_effectSource.PlayOneShot(m_audios[i]);

        if(i == 3)
        {
            m_musicSource.loop = true;
        }
    }
}

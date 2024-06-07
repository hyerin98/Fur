using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public bool isSound = true;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SFXFallingPlay(string sfxName, AudioClip[] clips)
    {
        if (isSound)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip clip = clips[randomIndex];

            GameObject fallingSound = new GameObject(sfxName + "Sound");
            AudioSource audioSource = fallingSound.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();

            Destroy(fallingSound, clip.length);
        }
    }

    public void SFXMovePlay(string sfxName, AudioClip[] clips)
    {
        if (isSound)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip clip = clips[randomIndex];

            GameObject moveSound = new GameObject(sfxName + "Sound");
            AudioSource audioSource = moveSound.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.Play();

            Destroy(moveSound, clip.length);
        }
    }
}

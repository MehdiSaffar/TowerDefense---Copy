using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    public static SoundManager instance = null;

    public AudioSource fxSource;
    public AudioSource musicSource;

    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    void Awake() {
        if (instance == null)
        {
            instance = this;
            if (Application.isPlaying)
            {
                if (transform.root == transform)  DontDestroyOnLoad(gameObject);
            }
        }
        else if (instance != this)
        {
            Debug.LogError("You cannot place multiple sound managers at once!");
            Destroy(gameObject);
            return;
        }
    }

    public static void PlaySingleFx(AudioClip fx)
    {
        instance.fxSource.clip = fx;
        instance.fxSource.Play();
    }
	public static void RandomizeFx(params AudioClip[] fxs)
    {
        int randomIndex = Random.Range(0, fxs.Length);
        instance.fxSource.clip = fxs[randomIndex];
        instance.fxSource.pitch = Random.Range(instance.lowPitchRange, instance.highPitchRange);
        instance.fxSource.PlayOneShot(fxs[randomIndex]);
    }
    public static void SetMusic(AudioClip music)
    {
        instance.musicSource.clip = music;
    }
    public static void PlayMusic(float volume = 1.0f, bool loop = true)
    {
        instance.musicSource.volume = volume;
        instance.musicSource.loop = loop;
        instance.musicSource.Play();
    }
    public static void StopMusic()
    {
        instance.musicSource.Stop();
    }
}

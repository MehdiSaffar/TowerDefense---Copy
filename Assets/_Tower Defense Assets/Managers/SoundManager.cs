using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {
    private SoundManager instance = null;

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

    public void PlaySingleFx(AudioClip fx)
    {
        fxSource.clip = fx;
        fxSource.Play();
    }
	public void RandomizeFx(params AudioClip[] fxs)
    {
        int randomIndex = Random.Range(0, fxs.Length);
        fxSource.clip = fxs[randomIndex];
        fxSource.pitch = Random.Range(lowPitchRange, highPitchRange);
        fxSource.PlayOneShot(fxs[randomIndex]);
    }
    public void SetMusic(AudioClip music)
    {
        musicSource.clip = music;
    }
    public void PlayMusic(float volume = 1.0f, bool loop = true)
    {
        musicSource.volume = volume;
        musicSource.loop = loop;
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
}

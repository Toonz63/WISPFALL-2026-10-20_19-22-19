using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager I;

    public AudioClip swing, hit, hurt, shrine, lose, win, music;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.35f;

    AudioSource sfxSource, musicSource;

    void Awake()
    {
        I = this;
        sfxSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.ignoreListenerPause = true;
    }

    void Start()
    {
        if (music != null) { musicSource.clip = music; musicSource.Play(); }
    }

    static void P(AudioClip c)
    {
        if (I != null && c != null) I.sfxSource.PlayOneShot(c, I.sfxVolume);
    }

    public static void Swing()  { if (I != null) P(I.swing); }
    public static void Hit()    { if (I != null) P(I.hit); }
    public static void Hurt()   { if (I != null) P(I.hurt); }
    public static void Shrine() { if (I != null) P(I.shrine); }
    public static void Lose()   { if (I != null) P(I.lose); }
    public static void Win()    { if (I != null) P(I.win); }
}
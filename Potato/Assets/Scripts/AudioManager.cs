using UnityEngine;

using System.Collections;

public enum EAudioPlayType { SFXPotatoSpawn, SFXPotatoHit, BGM }
public class AudioManager : MonoBehaviour
{
    private AudioSource sfxspawn;
    private AudioSource sfxhit;
    private AudioSource bgm;

    private bool sfxspawnbool;
    private bool sfxhitbool;
    private bool bgmbool;

    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("AudioManager", typeof(AudioManager)).GetComponent<AudioManager>();
            }
            return instance;
        }
    }

    public void Start()
    {
        if (!sfxspawn)
        {
            sfxspawn = gameObject.AddComponent<AudioSource>();
        }

        if (!sfxspawn)
        {
            sfxhit = gameObject.AddComponent<AudioSource>();
        }

        if (!bgm)
        {
            bgm = gameObject.AddComponent<AudioSource>();
        }

        transform.parent = Camera.main.transform;
        transform.localPosition = Vector3.zero;
    }

    public void PlaySound(EAudioPlayType audioPlayType, AudioClip clip)
    {
        if (!sfxhit)
        {
            sfxhit = gameObject.AddComponent<AudioSource>();
        }

        if (!sfxspawn)
        {
            sfxhit = gameObject.AddComponent<AudioSource>();
        }

        if (!bgm)
        {
            bgm = gameObject.AddComponent<AudioSource>();
        }

        switch (audioPlayType)
        {
            case EAudioPlayType.SFXPotatoSpawn:
                if (!sfxspawnbool)
                {
                    sfxspawnbool = !sfxspawnbool;
                    sfxspawn.clip = clip;
                    sfxspawn.Play();
                    StartCoroutine(WaitTill(audioPlayType, clip.length));
                }
                break;
            case EAudioPlayType.SFXPotatoHit:
                //if (!sfxhitbool)
                //{
                    //sfxhitbool = !sfxhitbool;
                    sfxhit.clip = clip;
                    sfxhit.Play();
                    //StartCoroutine(WaitTill(audioPlayType, clip.length));
                //}
                break;
            case EAudioPlayType.BGM:
                if (!bgmbool)
                {
                    bgmbool = !bgmbool;
                    bgm.clip = clip;
                    bgm.Play();
                    StartCoroutine(WaitTill(audioPlayType, clip.length));
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator WaitTill( EAudioPlayType audioType, float time )
    {
        yield return new WaitForSeconds(time);

        switch (audioType)
        {
            case EAudioPlayType.SFXPotatoSpawn:
                sfxspawnbool = !sfxspawnbool;
                break;
            case EAudioPlayType.SFXPotatoHit:
                sfxhitbool = !sfxhitbool;
                break;
            case EAudioPlayType.BGM:
                bgmbool = !bgmbool;
                break;
            default:
                break;
        }
    }
}

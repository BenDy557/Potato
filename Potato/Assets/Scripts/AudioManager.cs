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

    public void PlaySound(EAudioPlayType audioPlayType, AudioClip clip)
    {
        if (!sfxspawn)
        {
            sfxspawn = gameObject.AddComponent<AudioSource>();
            sfxspawn.volume = 0.20f;
        }

        if (!sfxhit)
        {
            sfxhit = gameObject.AddComponent<AudioSource>();
            sfxhit.volume = 0.10f;
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
                    //sfxspawn.Play();
                    StartCoroutine(WaitTill(audioPlayType, 2));
                }
                break;
            case EAudioPlayType.SFXPotatoHit:
                if (!sfxhitbool)
                {
                    sfxhitbool = !sfxhitbool;
                    sfxhit.clip = clip;
                    sfxhit.Play();
                    StartCoroutine(WaitTill(audioPlayType, 0.1f));
                }
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
                sfxhitbool = false;
                break;
            case EAudioPlayType.BGM:
                bgmbool = !bgmbool;
                break;
            default:
                break;
        }
    }
}

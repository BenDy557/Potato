using UnityEngine;
using System.Collections;

public class PotatoDeactivation : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    private bool canValidate = false;
    private bool canPlay = true;

    [SerializeField] private AudioClip[] hitClips;
    [SerializeField] private AudioClip[] spawnClips;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(ToggleBool(1));
    }
	
    private void Update ()
    {
        if (canValidate)
        {
            if (myRigidbody.velocity.magnitude < 0.01f )
            {
                Destroy(myRigidbody);
                Destroy(this);
            }
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlaySound(EAudioPlayType.SFXPotatoHit);
    }

    public void PlaySound(EAudioPlayType audioType)
    {
        AudioClip clip = null;

        switch (audioType)
        {
            case EAudioPlayType.SFXPotatoSpawn:
                clip = spawnClips[ UnityEngine.Random.Range(0, spawnClips.Length - 1)];

                break;
            case EAudioPlayType.SFXPotatoHit:
                clip = hitClips[UnityEngine.Random.Range(0, hitClips.Length - 1)];
                break;
            default:
                return;
        }

        AudioManager.Instance.PlaySound(audioType, clip);
    }

    private IEnumerator ToggleBool(float t)
    {
        yield return new WaitForSeconds(t);
        canValidate = !canValidate;
    }

    private IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
        canPlay = !canPlay;
    }
}

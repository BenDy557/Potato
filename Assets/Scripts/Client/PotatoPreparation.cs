using UnityEngine;
using System.Collections;
using System.Net;

public class PotatoPreparation : MonoBehaviour {

    public GameObject m_UnpeeledPotato;
    public GameObject m_PeeledPotato;
    public GameObject m_Light;

    //private bool m_ReadyToPeel;
    private bool m_ReadyToSend;

    public  Animator m_PeelAnimator;
    public AnimationCurve m_SpawnCurve;
    private bool m_IsSpawning;
    public float m_SpawnTimer;
    private float m_SpawnTimerCurrent;

    private bool m_SendingPotato;
    public float m_TransitionSpeed;
	public ClientSQLComm m_clientComm;

	private AudioSource audioSource = null;


	[SerializeField] private AudioClip[] spawnSounds;
	[SerializeField] private AudioClip[] peelSounds;
	[SerializeField] private AudioClip[] sendSounds;

	private bool isMoving = false;
	private bool sendAnimation = false;
	private bool touchBeenReset = false;

	// Use this for initialization
	void Start () {

        //m_ReadyToPeel = false;
        m_ReadyToSend = false;

        m_SendingPotato = false;
        //m_PeelAnimator = m_UnpeeledPotato.GetComponent<Animator>();
        m_PeelAnimator.speed = 0;

		m_clientComm = GameObject.FindGameObjectWithTag ("Client").GetComponent<ClientSQLComm>();

		m_PeelAnimator.GetBehaviour<TriggerEndOfPeel>().script = this;

		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = spawnSounds[ Random.Range(0, spawnSounds.Length-1)];
		audioSource.Play(   );

        m_SpawnTimerCurrent = m_SpawnTimer;
        m_IsSpawning = true;
        transform.localScale = Vector3.zero;
	}
	
	// Update is called once per frame
	private void Update () 
	{
        if (!m_IsSpawning)
        {

            GetTouchMove();

            if (m_SendingPotato && Input.touches.Length == 0)
            {
                touchBeenReset = true;
            }

            if (!m_SendingPotato)
            {
                m_PeelAnimator.SetBool("RestartPeeling", false);


                if (isMoving)
                {
                    m_PeelAnimator.speed = 1;

                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = peelSounds[Random.Range(0, peelSounds.Length - 1)];
                        audioSource.Play();
                    }
                }
            }
            else
            {

                //if(isGracePeriodFinished)
                //{
                if (touchBeenReset)
                {
                    if (isMoving)
                    {
                        sendAnimation = true;
                    }

                    if (sendAnimation)
                    {
                        if (m_ReadyToSend)
                        {
                            m_ReadyToSend = false;


                            audioSource.clip = sendSounds[Random.Range(0, sendSounds.Length - 1)];
                            audioSource.Play();
                        }

                        transform.position += new Vector3(0.0f, m_TransitionSpeed * Time.deltaTime, 0.0f);

                        if (transform.position.y > 20.0f)
                        {
                            m_SendingPotato = false;
                            m_PeelAnimator.SetBool("RestartPeeling", true);
                            m_PeelAnimator.SetBool("StartPeeling", true);

                            m_clientComm.CreatePotato();

                            Destroy(gameObject);
                        }
                    }
                }
                //}
            }
        }
        else
        {
            float temp = m_SpawnCurve.Evaluate(1-(float)(m_SpawnTimer/m_SpawnTimerCurrent));
            transform.localScale = new Vector3(temp,temp,temp);


            m_SpawnTimer -= Time.deltaTime;

            if (m_SpawnTimer <= 0.0f)
            {
                m_IsSpawning = false;
            }


        }
	}

	public void GetTouchMove()
	{
		Touch[] touches = Input.touches;
		
		isMoving = false;
		
		for (int i = 0; i < touches.Length; i++) 
		{
			if (touches[i].phase == TouchPhase.Moved && touches[i].deltaPosition.magnitude > 2f) 
			{
				isMoving = true;
				break;
			}
		}
	}

	// Called From The Animator State Behaviour
	public void SendPotato()
	{
		m_SendingPotato = true;
		m_ReadyToSend = true;

		m_PeelAnimator.SetBool("StartPeeling", false);

		m_UnpeeledPotato.SetActive(false);
	}
}

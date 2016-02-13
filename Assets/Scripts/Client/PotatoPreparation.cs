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

    private bool m_SendingPotato;
    public float m_TransitionSpeed;
	public ClientSQLComm m_clientComm;

	public float gracePeriod = 0;
	private bool isGracePeriodFinished = false;

	// Use this for initialization
	void Start () {

        //m_ReadyToPeel = false;
        m_ReadyToSend = false;

        m_SendingPotato = false;
        //m_PeelAnimator = m_UnpeeledPotato.GetComponent<Animator>();
        m_PeelAnimator.speed = 0;

		m_clientComm = GameObject.FindGameObjectWithTag ("Client").GetComponent<ClientSQLComm>();
	}
	
	// Update is called once per frame
	private void Update () 
	{	
		if (!m_SendingPotato) 
		{
			m_PeelAnimator.SetBool("RestartPeeling", false);

			Touch[] touches = Input.touches;

			bool isMoving = false;

			for (int i = 0; i < touches.Length; i++) 
			{
				if (touches[i].phase == TouchPhase.Moved && touches[i].deltaPosition.magnitude > 0.1f) 
				{
					isMoving = true;
					break;
				}
			}

			if (isMoving) 
			{
				m_PeelAnimator.speed = 1;



				if (m_PeelAnimator.GetCurrentAnimatorStateInfo (0).IsName ("Peel9") && m_PeelAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.90) 
				{
					m_SendingPotato = true;
					m_ReadyToSend = true;

					m_PeelAnimator.SetBool("StartPeeling", false);

					StartCoroutine(WaitForGracePeriod());

					m_UnpeeledPotato.SetActive(false);
				}
			} else 
			{
				m_PeelAnimator.speed = 0;
			}
		} else 
		{
			if(isGracePeriodFinished)
			{
				if(m_ReadyToSend)
				{
					m_ReadyToSend = false;

					m_clientComm.CreatePotato();
				}
				
				transform.position += new Vector3(0.0f, m_TransitionSpeed * Time.deltaTime, 0.0f);
				
				if (transform.position.y > 20.0f)
				{
					m_SendingPotato = false;
					m_PeelAnimator.SetBool("RestartPeeling", true);
					m_PeelAnimator.SetBool("StartPeeling", true);

					Destroy(gameObject);
				}
			}
		}
	}

	private IEnumerator WaitForGracePeriod()
	{
		yield return new WaitForSeconds (gracePeriod);
		isGracePeriodFinished = true;
	}
}

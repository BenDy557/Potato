using UnityEngine;
using System.Collections;

public class PotatoPreparation : MonoBehaviour {

    public GameObject m_UnpeeledPotato;
    public GameObject m_PeeledPotato;
    public GameObject m_Light;

    public SocketComm m_Socket;

    private bool m_ReadyToPeel;
    private bool m_ReadyToSend;

    private Animator m_PeelAnimator;

    private bool m_SendingPotato;
    private float m_TransitionSpeed;
    public float m_TransitionSpeedMax;
	// Use this for initialization
	void Start () {

        m_ReadyToPeel = false;
        m_ReadyToSend = false;

        m_SendingPotato = false;

        m_TransitionSpeed = 0.0f;

        m_Socket = GameObject.FindGameObjectWithTag("Client").GetComponent<SocketComm>();

        m_PeelAnimator = m_UnpeeledPotato.GetComponent<Animator>();
        m_PeelAnimator.speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
        Touch[] touches = Input.touches;

        if (touches.Length != 0)
        {
            if (touches[0].phase == TouchPhase.Moved)
            {
                if (m_UnpeeledPotato)
                {
                    m_PeelAnimator.speed=1;
                    //m_ReadyToPeel = true;
                    if (m_PeelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Peel9") && m_PeelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                    {
                        m_PeelAnimator.speed = 0;
                    }
                }
                
                if (m_PeeledPotato&&!m_UnpeeledPotato)
                {
                    m_ReadyToSend = true;
                }
            }

            //m_PeelAnimator.set
            
             
            if(touches[0].phase == TouchPhase.Ended || touches[0].phase == TouchPhase.Canceled)
            {
                if (m_UnpeeledPotato)
                {
                    m_PeelAnimator.speed = 0;
                }

                if (m_PeeledPotato && m_ReadyToSend)
                {
                    m_Socket.RequestPotatoCreation();
                    m_SendingPotato = true;
                }

                if (m_PeelAnimator)
                {

                    if (m_PeelAnimator.GetCurrentAnimatorStateInfo(0).IsName("Peel9"))
                    {
                        if (m_PeelAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                        {
                            Destroy(m_UnpeeledPotato);
                        }
                    }
                }
            }
        }
        else if (m_PeelAnimator)
        {
            m_PeelAnimator.speed = 0;
        }


       
            /*
        else if (m_UnpeeledPotato)
        {
            m_PeelAnimator.speed = 0;
        }*/

        if (m_SendingPotato)
        {
            m_ReadyToSend = false;
            m_TransitionSpeed += (100.0f * Time.deltaTime);
            if (m_TransitionSpeed > m_TransitionSpeedMax)
            {
                m_TransitionSpeed = m_TransitionSpeedMax;
            }

            transform.position += new Vector3(0.0f, m_TransitionSpeed*Time.deltaTime, 0.0f);

            if (transform.position.y > 20.0f)
            {
                m_SendingPotato = false;
                Destroy(gameObject);
            }
        }
        

	}
}

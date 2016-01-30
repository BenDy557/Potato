using UnityEngine;
using System.Collections;

public class PotatoPreparation : MonoBehaviour {

    public GameObject m_UnpeeledPotato;
    public GameObject m_PeeledPotato;
    public GameObject m_Light;

    public SocketComm m_Socket;

    private bool m_ReadyToPeel;
    private bool m_ReadyToSend;
	// Use this for initialization
	void Start () {

        m_ReadyToPeel = false;
        m_ReadyToSend = false;
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
                    m_ReadyToPeel = true;
                }
                else if (m_PeeledPotato)
                {
                    m_ReadyToSend = true;
                }
            }
             
            if(touches[0].phase == TouchPhase.Ended || touches[0].phase == TouchPhase.Canceled)
            {
                 if (m_UnpeeledPotato && m_ReadyToPeel)
                 {
                    Destroy(m_UnpeeledPotato);
                    //SendPotato
                    //m_Socket.RequestPotatoCreation();
                 }

                 if (m_PeeledPotato && m_ReadyToSend)
                 {
                     Destroy(m_PeeledPotato);
                     //SendPotato
                     m_Socket.RequestPotatoCreation();
                     Destroy(gameObject);
                 }
            }

        }

	}
}

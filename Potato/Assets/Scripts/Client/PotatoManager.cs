using UnityEngine;
using System.Collections;

public class PotatoManager : MonoBehaviour {

    public GameObject m_PotatoPrefab;
    public GameObject m_Potato;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (!m_Potato)
        {
            m_Potato = Instantiate<GameObject>(m_PotatoPrefab);
        }


        
	}
}

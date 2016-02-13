using UnityEngine;
using System.Net;
using System.Collections;

public class ClientSQLComm : MonoBehaviour 
{
	private bool spawn = false;

	private void Start()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
	}

	private void Update()
	{
		if(spawn)
		{
			StartCoroutine (IncrementPotato ());
			spawn = false;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.Quit();
		}
	}

	public void CreatePotato()
	{
		spawn = true;
	}

	private IEnumerator IncrementPotato()
	{
		HttpWebRequest webRequestSend = (HttpWebRequest)WebRequest.Create(@"http://www.potato.azurelit.com/IncrementPotato.php");
		
		webRequestSend.GetResponse();
		
		yield return new WaitForEndOfFrame();
	}
}

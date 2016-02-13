using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;

public class ServerSQLComm : MonoBehaviour 
{
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log ("Incrementing");
			HttpWebRequest webRequestSend = (HttpWebRequest)WebRequest.Create(@"http://www.potato.azurelit.com/IncrementPotato.php");

			webRequestSend.GetResponse().GetResponseStream();
			//if(reader.ReadToEnd() != "")
			//{
			//	Debug.Log("Unkown Return Value: " + reader.ReadToEnd());
			//}
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			Debug.Log ("Checking Value");
			HttpWebRequest webRequestCheck = (HttpWebRequest)WebRequest.Create(@"http://www.potato.azurelit.com/GetPotatoCount.php");

			StreamReader reader = new StreamReader(webRequestCheck.GetResponse().GetResponseStream());  
			Debug.Log(reader.ReadToEnd());
		}

		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			Debug.Log ("Reseting");
			HttpWebRequest webRequestReset = (HttpWebRequest)WebRequest.Create(@"http://www.potato.azurelit.com/ResetPotatoCount.php");

			StreamReader reader = new StreamReader(webRequestReset.GetResponse().GetResponseStream());  
			if(reader.ReadToEnd() != "")
			{
				Debug.Log("Unknown Return Value: " + reader.ReadToEnd());
			}
		}
	}

	//private IEnumerator GetStuff();
}
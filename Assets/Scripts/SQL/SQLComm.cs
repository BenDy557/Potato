using UnityEngine;

using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public enum EConnectionBehaviour { None, Server, Client }

public class SQLComm : MonoBehaviour
{
	[Header("Server Variables")] // Server Variables
	public GameObject[] potatoPrefab;
	public Gradient potatoGradient;
	public AudioClip audioClip;
	private int currentPotatoCount = 0;
	private Vector3 limits;

	private int webPotatoCount = 0;
	
	/// <summary>
	/// Socket Initialization Place Any Other Code Before the Socket Initialization
	/// </summary>
	private void Start()
	{
		limits.x = -4;
		limits.y =  8;
		limits.z =  4;
		
		AudioManager.Instance.PlaySound(EAudioPlayType.BGM, audioClip);

		StartCoroutine(UpdatePotatoCounter());
	}
	
	/// <summary>
	/// Logic Code Here
	/// </summary>
	private void Update()
	{
			if (currentPotatoCount < webPotatoCount)
			{
				Vector3 position = new Vector3(UnityEngine.Random.Range(limits.x, limits.z), limits.y, 0);
				Quaternion startRotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(0, 360)));

				GameObject obj = ((GameObject)Instantiate(potatoPrefab[ UnityEngine.Random.Range(0, potatoPrefab.Length-1) ], position, startRotation));

				obj.GetComponent<SpriteRenderer>().color = potatoGradient.Evaluate(UnityEngine.Random.Range(0f,1f));

				obj.GetComponent<PotatoDeactivation>().PlaySound(EAudioPlayType.SFXPotatoSpawn);

				float scale = UnityEngine.Random.Range(0.18f, 0.30f);
				obj.GetComponent<Transform>().localScale = new Vector3(scale, scale, scale);

				currentPotatoCount++;
			}
	}

	public void UpdateSpawnLimits( float y )
	{
		limits.y = y;
	}

	private IEnumerator UpdatePotatoCounter()
	{
		WaitForSeconds waitTimer = new WaitForSeconds(0.5f);
		while(true)
		{
			HttpWebRequest webRequestCheck = (HttpWebRequest)WebRequest.Create(@"http://www.potato.azurelit.com/GetPotatoCount.php");
			
			StreamReader reader = new StreamReader(webRequestCheck.GetResponse().GetResponseStream());  
			webPotatoCount = int.Parse(reader.ReadToEnd());

			yield return waitTimer;
		}
	}
}

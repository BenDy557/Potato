using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class ConnectionChecker : MonoBehaviour 
{
	[SerializeField] private Image image;

	[SerializeField] private Sprite successfulConnection; 
	[SerializeField] private Sprite failedConnection; 

	private void Start () 
	{
		StartCoroutine(CheckConnection());
	}

	private IEnumerator CheckConnection()
	{
		NetworkReachability status;
		while(true)
		{
			status = Application.internetReachability;

			if(status == NetworkReachability.ReachableViaLocalAreaNetwork || status == NetworkReachability.ReachableViaCarrierDataNetwork)
			{
				image.sprite = successfulConnection;
			}
			else
			{
				image.sprite = failedConnection;
			}

			yield return new WaitForSeconds(3);
		}
	}
}

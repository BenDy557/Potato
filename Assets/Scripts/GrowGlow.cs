using UnityEngine;
using System.Collections;

public class GrowGlow : MonoBehaviour 
{
	public Vector3 offsetGrowth;
	public float sinSpeed; 

	private void Update () 
	{
		float sinTime = Mathf.Sin (Time.time * sinSpeed);
		transform.localScale += sinTime * offsetGrowth;
	}
}

using UnityEngine;
using System.Collections;

public class PotatoDeactivation : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    private bool canValidate = false;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(ToggleBool(1));
    }
	
    private void Update ()
    {
        if (canValidate)
        {
            if (myRigidbody.velocity.magnitude < 0.01f )
            {
                Destroy(myRigidbody);
                Destroy(this);
            }
        }
	}

    private IEnumerator ToggleBool(float t)
    {
        yield return new WaitForSeconds(t);
        canValidate = !canValidate;
    }
}

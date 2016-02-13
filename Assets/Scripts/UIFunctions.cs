using UnityEngine;
using System.Collections;

public class UIFunctions : MonoBehaviour
{
    [SerializeField]
    GameObject splashScreen;
    [SerializeField]
    GameObject splashtogglebutton;

    public void ToggleIngameSplashScreenTrue()
    {
        splashScreen.SetActive(true);
        splashtogglebutton.SetActive(false);
    }

    public void ToggleIngameSplashScreenFalse()
    {
        splashScreen.SetActive(false);
        splashtogglebutton.SetActive(true);
    }
}

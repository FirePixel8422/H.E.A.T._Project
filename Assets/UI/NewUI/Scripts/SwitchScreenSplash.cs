using System.Collections;
using UnityEngine;

public class SwitchScreenSplash : MonoBehaviour
{
    public float splashScreenTime;
    void Start()
    {
        StartCoroutine(WaitForSplash(splashScreenTime));
    }
    IEnumerator WaitForSplash(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("Setup Screen");
    }
}

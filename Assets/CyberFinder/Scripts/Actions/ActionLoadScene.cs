using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActionLoadScene : MonoBehaviour
{
    [Tooltip("Name of the scene to load")]
    public string sceneName;

    [Tooltip("Use fade for transition")]
    public bool useFade;

    public void DoLoadScene()
    {
        if (useFade)
        {
            // Begin the fade-out
            OVRScreenFade.instance.FadeOut();

            // Start loading scene after fade time
            StartCoroutine(LoadAfterTime(OVRScreenFade.instance.fadeTime));
        }
        else
        {
            // Do instant transition
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(sceneName);
    }
}

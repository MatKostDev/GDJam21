using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    public float delay = 1.0f;
    public Animator transition;

	public void StartGameScene() 
    {
		StartCoroutine(LoadGameSceneAsync());
        transition.SetTrigger("Start");
	}

	private static IEnumerator LoadGameSceneAsync() 
    {
        yield return new WaitForSeconds(1.0f);

        var asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

		while (!asyncLoad.isDone) 
        {
			yield return null;

			StatTracker.ResetStats();
		}
	}
	
	public void QuitApplication() 
    {
		Application.Quit();
	}
}

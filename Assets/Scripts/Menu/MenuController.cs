using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {
    // Start is called before the first frame update

    public float delay = 1.0f;
    public Animator transition;
	private void Start() {
    }

	public void StartGameScene() {
		StartCoroutine(LoadGameSceneAsync());
        transition.SetTrigger("Start");
	}

	private static IEnumerator LoadGameSceneAsync() {

        yield return new WaitForSeconds(1.0f);

        var asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);

		while (!asyncLoad.isDone) {
			yield return null;

			StatTracker.ResetStats();
		}
	}
	
	public void QuitApplication() {
		Application.Quit();
	}
}

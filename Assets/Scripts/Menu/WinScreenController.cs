using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreenController : MonoBehaviour 
{
	[SerializeField]
	TMP_Text timeText;

    [SerializeField]
    TMP_Text deathsText;

    [SerializeField]
    TMP_Text killsText;

	public float delay = 1.0f;
    public Animator transition;
	private void Start() 
    {
        int minutes = Mathf.FloorToInt(StatTracker.GetPlayTime() / 60f);
        int seconds = Mathf.FloorToInt(StatTracker.GetPlayTime() - minutes * 60);
        string timeString = string.Format("{0:0}:{1:00}", minutes, seconds);
        timeText.text = timeString;

        deathsText.text = StatTracker.PlayerDeaths.ToString();
        killsText.text  = StatTracker.EnemiesKilled.ToString();
    }

	public void StartMenuScene() 
    {
		StartCoroutine(LoadMenuSceneAsync());
        transition.SetTrigger("Start");
	}

	private static IEnumerator LoadMenuSceneAsync() 
    {

        yield return new WaitForSeconds(1.0f);

        var asyncLoad = SceneManager.LoadSceneAsync("Menu");


		while (!asyncLoad.isDone) 
        {
			yield return null;
		}
	}
	
	public void QuitApplication()
    {
		Application.Quit();
	}
}

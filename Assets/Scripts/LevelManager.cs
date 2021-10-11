using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    public Scene winScene;

    public GameObject pauseMenu;
	
	public UnityEvent onPreLevelLoadEvent;
	public UnityEvent onPostLevelLoadEvent;
	public UnityEvent onLevelStartEvent;

	LevelTracker m_levelTracker;

	public int NumLevelsPassed { get; private set; } = 0;

	private void Start() 
    {
        m_levelTracker = FindObjectOfType<LevelTracker>();
    }

	private void OnDisable() 
    {
		Time.timeScale = 1;
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(true);

                Time.timeScale = 0;
            }
            else
            {
                ResumeButtonClickedEvent();
            }
        }
    }

    public void LoadNextLevel(bool reset = false) 
    {
		if (!reset)
        {
			NumLevelsPassed++;
        }

		onPreLevelLoadEvent?.Invoke();
		
		//Destroy(m_currentLevel);

		//if (reset)
		//{
		//	m_currentLevel = Instantiate(levels[m_currentLevelIndex]);
		//}
		//else
		//{
		//	if (levels.Count <= 1)
        //  {
		//		m_currentLevelIndex = 0;
		//  }
		//	else 
		//	{
		//		int newLevelIndex = m_currentLevelIndex;
		//		while (newLevelIndex == m_currentLevelIndex)
		//			newLevelIndex = Random.Range(1, levels.Count);

		//		m_currentLevelIndex = newLevelIndex;
		//	}

		//	m_currentLevel = Instantiate(levels[m_currentLevelIndex]);
		//}

		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);

		onPostLevelLoadEvent?.Invoke();
		onPostLevelLoadEvent?.RemoveAllListeners();
	}

	public void MenuButtonClickedEvent() 
    { 
		StartCoroutine(LoadSceneAsync("Menu"));
	}

	public void ResumeButtonClickedEvent() 
    {
        pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }

	private IEnumerator LoadSceneAsync(string scene) 
    {
		var asyncLoad = SceneManager.LoadSceneAsync(scene);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}

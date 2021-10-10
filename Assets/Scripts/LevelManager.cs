using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour { 
	public List<Scene> levels;
	public int startingLevelIndex = 0;

	public GameObject pauseMenu;
	
	public UnityEvent onPreLevelLoadEvent;
	public UnityEvent onPostLevelLoadEvent;
	public UnityEvent onLevelStartEvent;

	private int m_currentLevelIndex;
	private GameObject m_currentLevel;

	public int NumLevelsPassed { get; private set; } = 0;

	private void Start() 
    {
		LoadLevelPrefabs("Prefabs/Levels");
		
		// Call GenerateNewLevel() with reset to avoid having to rewrite the function in Start()
		m_currentLevelIndex = startingLevelIndex;
		//GenerateNewLevel(reset: true);
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

    private void GenerateNewLevel(bool reset) 
    {
		if (!reset)
        {
			NumLevelsPassed++;
        }

		onPreLevelLoadEvent?.Invoke();
		
		//Destroy(m_currentLevel);

		//if (reset) {
		//	m_currentLevel = Instantiate(levels[m_currentLevelIndex]);
		//} else {
		//	if (levels.Count <= 1)
		//		m_currentLevelIndex = 0;
		//	else 
		//	{
		//		int newLevelIndex = m_currentLevelIndex;
		//		while (newLevelIndex == m_currentLevelIndex)
		//			newLevelIndex = Random.Range(1, levels.Count);

		//		m_currentLevelIndex = newLevelIndex;
		//	}

		//	m_currentLevel = Instantiate(levels[m_currentLevelIndex]);
		//}

        SceneManager.LoadScene(levels[NumLevelsPassed].buildIndex, LoadSceneMode.Single);

		onPostLevelLoadEvent?.Invoke();
		onPostLevelLoadEvent?.RemoveAllListeners();
	}

	private void PlayerInfectedEvent() 
    {
		GenerateNewLevel(reset: true);
	}
	
	private void PlayerSurvivedEvent() 
    {
		GenerateNewLevel(reset: false);
	}

	public void RetryButtonClickedEvent() 
    {
		StartCoroutine(LoadSceneAsync("Gameplay"));
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

	private void LoadLevelPrefabs(string path) 
    {
		//var res = Resources.LoadAll<GameObject>(path);

		//foreach (var obj in res) {
		//	levels.Add(obj);
		//}
	}
}

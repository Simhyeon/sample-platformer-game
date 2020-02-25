using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	// SerializeField
	[SerializeField] GameObject levelEndScreen;
	[SerializeField] GameObject levelClearScreen;

	public enum LoadType
	{
		Next,
		Start,
		Reload
	}

	public static LevelManager levelManager;

	void Awake()
	{
		levelManager = this;
	}

	public void LoadScene(bool loadNext = true, LoadType loadType = LoadType.Next)
	{
		switch (loadType)
		{
			case LoadType.Next:
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
				break;

			case LoadType.Start:
				SceneManager.LoadScene(0);
				break;

			case LoadType.Reload:
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
				break;

			default:
				break;
		}
	}

	public void LoadNextScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void PlayerDeath()
	{
		StartCoroutine(DisplayUIScreen(levelEndScreen, 1f));
	}

	public void LevelClear()
	{
		StartCoroutine(DisplayUIScreen(levelClearScreen, 0.3f));
	}
	
	public void QuitGame()
	{
		Application.Quit();
	}

	IEnumerator DisplayUIScreen(GameObject ui, float delay)
	{
		yield return new WaitForSeconds(delay);
		ui.SetActive(true);
	}
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	// SerializeField
	[SerializeField] GameObject levelEndScreen;
	[SerializeField] GameObject levelClearScreen;
	[SerializeField] GameObject levelSelectionScreen;

	public static LevelManager levelManager;

	void Awake()
	{
		levelManager = this;
	}

	public void LoadScene(int sceneNumber)
	{
		SceneManager.LoadScene(sceneNumber);
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

	public void LoadMainMenu()
	{
		SceneManager.LoadScene(0);
	}

	public void LoadLevelSelection()
	{
		levelSelectionScreen.SetActive(true);
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

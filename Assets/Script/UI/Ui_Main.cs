using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ui_Main : MonoBehaviour
{
	private bool gamePaused;
	public static bool isGamePaused = false;


	[SerializeField] private GameObject mainMenu;

	private void Start()
	{
		SwitchMenuTo(mainMenu);
		Time.timeScale = 1;
	}

	public void SwitchMenuTo(GameObject uiMenu)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}

		uiMenu.SetActive(true);
	}

	public void StartGameButton() => GameManager.instance.UnlockPlayer();

	public void PauseGameButton()
	{
		if (gamePaused)
		{
			Time.timeScale = 1;
			gamePaused = false;
			GameManager.instance.IsMenuOpen = false;
		}
		else
		{
			Time.timeScale = 0;
			gamePaused = true;
			GameManager.instance.IsMenuOpen = true;
		}
	}

	public void RestartGameButton() => GameManager.instance.RestartLevel();

	public void ChangeScene(string sceneName)
	{
		Time.timeScale = 1; // tránh trường hợp bị pause khi chuyển scene
		SceneManager.LoadScene(sceneName);
	}
}


   

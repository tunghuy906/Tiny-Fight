using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Ui_Main : MonoBehaviour
{
	private bool gamePaused;
	private bool gameMuted;
	public static bool isGamePaused = false;


	[SerializeField] private GameObject mainMenu;

	[Header("Volume sliders")]
	[SerializeField] private UI_VolumeSlider[] slider;
	[SerializeField] private Image muteIcon;
	[SerializeField] private Image inGameMuteIcon;

	private void Start()
	{
		for(int i = 0; i< slider.Length;  i++)
		{
			slider[i].SetupSlider();
		}

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

		AudioManager.instance.PlaySfx(2);
	}
	public void MuteButton()
	{
		gameMuted = !gameMuted;
		if(gamePaused)
		{
			AudioListener.volume = 0;
		}
		else
		{
			AudioListener.volume = 1;
		}
	}
	public void StartGameButton()
	{
		GameManager.instance.UnlockPlayer();
	}
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


   

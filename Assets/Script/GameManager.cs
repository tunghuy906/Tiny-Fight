using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public PlayerController player;
	public static GameManager instance;
	public Ui_User uiUser;

	[Header("SkillPoint info")]
	public int skillPoints;

	[Header("UI Elements")]
	public GameObject deathPanel;

	private void Awake()
	{
		instance = this;
	}

	public void UnlockPlayer() => player.playerUnlocked = true;
	public void RestartLevel()
	{
		DontDestroy[] donts = FindObjectsOfType<DontDestroy>();
		foreach (var d in donts)
		{
			Destroy(d.gameObject);
		}
		SaveInfo();
		SceneManager.LoadScene(0);
	}

	public void SaveInfo()
	{
		int savedSkillPoint = PlayerPrefs.GetInt("SkillPoints");

		PlayerPrefs.SetInt("SkillPoint", savedSkillPoint + skillPoints);
	}
	public void ShowDeathUI()
	{
		deathPanel.SetActive(true);
	}
	public void AddSkillPoint(int amount)
	{
		skillPoints += amount;
		uiUser.IncreaseSkillPoint(amount);
	}
}

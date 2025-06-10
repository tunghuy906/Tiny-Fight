using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
	public void RestartCurrentMap()
	{
		// Lấy tên của scene hiện tại
		string currentSceneName = SceneManager.GetActiveScene().name;

		// Tải lại scene đó
		SceneManager.LoadScene(currentSceneName);
	}
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
	public void PlayGame()
	{
		SceneManager.LoadScene("ChoseMap"); // Thay bằng tên scene thật
	}

	public void OpenSettings()
	{
		SceneManager.LoadScene("Tutorial"); // Tùy chọn
	}

	public void ExitGame()
	{
		Application.Quit();
		Debug.Log("Game exited");
	}
}

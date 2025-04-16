using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChoseMap : MonoBehaviour
{
	public void Map1()
	{
		SceneManager.LoadScene("Map 1");
	}

	public void Map2()
	{
		// Kiểm tra nếu đã hoàn thành Map 1
		if (PlayerPrefs.GetInt("Map1Completed", 0) == 1)
		{
			SceneManager.LoadScene("Map 2");
		}
		else
		{
			Debug.Log("Bạn phải hoàn thành Map 1 trước!");
			// Optional: Hiện UI báo lỗi ở đây
		}
	}

	public void Map3()
	{
		if (PlayerPrefs.GetInt("Map2Completed", 0) == 1)
		{
			SceneManager.LoadScene("Map 3");
		}
		else
		{
			Debug.Log("Bạn phải hoàn thành Map 2 trước!");
		}
	}

	public void Menu()
	{
		SceneManager.LoadScene("Menu");
	}
}

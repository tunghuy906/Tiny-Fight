using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
	public void Menu()
	{
		SceneManager.LoadScene("Menu");

		if (DontDestroy.Instance != null)
		{
			Destroy(DontDestroy.Instance.gameObject);
			DontDestroy.Instance = null;
		}
	}
}

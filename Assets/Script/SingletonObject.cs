using UnityEngine;

public class SingletonObject : MonoBehaviour
{
	private static bool exists = false;

	void Awake()
	{
		if (!exists)
		{
			DontDestroyOnLoad(gameObject);
			exists = true;
		}
		else
		{
			Destroy(gameObject);
		}
	}
}

using UnityEngine;

public class DontDestroy : MonoBehaviour
{
	public static DontDestroy Instance;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject); // Giữ lại toàn bộ GameManager
		}
		else
		{
			Destroy(gameObject); // Tránh tạo thêm GameManager mới khi load lại scene
		}
	}
}


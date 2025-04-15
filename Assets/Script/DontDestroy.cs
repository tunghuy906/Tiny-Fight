using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class DontDestroy : MonoBehaviour
{
	public static DontDestroy Instance;

	public int totalItemsToCollect = 3;
	private int itemsCollected = 0;

	public GameObject winPanel;
	public GameObject player; // Gán trong Inspector hoặc tìm bằng tag

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void CollectItem()
	{
		itemsCollected++;
		Debug.Log("Đã thu thập: " + itemsCollected + "/" + totalItemsToCollect);

		if (itemsCollected >= totalItemsToCollect)
		{
			LevelComplete();
		}
	}

	private void LevelComplete()
	{
		Debug.Log("Hiện UI chiến thắng!");
		if (winPanel != null)
		{
			winPanel.SetActive(true);
		}
	}

	public void ResetGame()
	{
		itemsCollected = 0;
		if (winPanel != null)
			winPanel.SetActive(false);
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (winPanel != null && winPanel.activeSelf)
		{
			winPanel.SetActive(false);
		}

		itemsCollected = 0;

		MovePlayerToSpawnPoint();

		ResetPlayerStats();

		// Xử lý EventSystem trùng
		EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
		if (eventSystems.Length > 1)
		{
			foreach (var sys in eventSystems)
			{
				if (sys != EventSystem.current)
				{
					Debug.Log("Destroyed extra EventSystem: " + sys.name);
					Destroy(sys.gameObject);
				}
			}
		}
	}

	void MovePlayerToSpawnPoint()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			if (player == null)
			{
				Debug.LogWarning("Không tìm thấy Player trong scene mới.");
				return;
			}
		}

		GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");
		if (spawnPoint != null)
		{
			player.transform.position = spawnPoint.transform.position;
			Debug.Log("Đặt lại vị trí player tại điểm spawn.");
		}
		else
		{
			Debug.LogWarning("Không tìm thấy PlayerSpawnPoint trong scene mới.");
		}
	}
	private void ResetPlayerStats()
	{
		if (player == null)
		{
			player = GameObject.FindGameObjectWithTag("Player");
			if (player == null)
			{
				Debug.LogWarning("Không tìm thấy Player để reset máu/mana.");
				return;
			}
		}

		var damageable = player.GetComponent<Damageable>();
		if (damageable != null)
		{
			damageable.ResetHealth();
		}

		var manaBar = FindObjectOfType<ManaBar>(); // Giả sử có 1 mana bar duy nhất
		if (manaBar != null)
		{
			manaBar.ResetMana();
		}
	}

}

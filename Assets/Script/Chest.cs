using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
	public GameObject[] itemPrefabs;  // Mảng chứa nhiều vật phẩm
	public Transform[] spawnPoints;   // Danh sách các điểm xuất hiện vật phẩm
	private bool isOpened = false;
	private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>(); // Lấy Animator (nếu có)
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player") && !isOpened)
		{
			AudioManager.instance.PlaySfx(6);
			OpenChest();
		}
	}

	void OpenChest()
	{
		isOpened = true;

		if (animator != null)
		{
			animator.SetTrigger("Open");  // Nếu có Animator, phát animation mở
		}

		SpawnItems();  // Gọi hàm sinh vật phẩm
	}

	void SpawnItems()
	{
		if (itemPrefabs.Length > 0 && spawnPoints.Length > 0)
		{
			for (int i = 0; i < itemPrefabs.Length; i++)
			{
				Transform spawnPosition = spawnPoints[i % spawnPoints.Length]; // Chọn vị trí tuần tự
				Instantiate(itemPrefabs[i], spawnPosition.position, Quaternion.identity);
			}
		}
	}
}

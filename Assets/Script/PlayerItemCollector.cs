using UnityEngine;
using TMPro;

public class PlayerItemCollector : MonoBehaviour
{
	[Header("Fruit Settings")]
	public int fruitCount = 0;

	[Header("UI Reference")]
	public TMP_Text fruitCounterText; // Kéo object có TMP_Text vào đây trong Inspector

	private void Start()
	{
		UpdateFruitText(); // Hiển thị "x0" lúc đầu
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Fruit"))
		{
			fruitCount++;
			UpdateFruitText();
			Destroy(collision.gameObject);

			if (fruitCount >= 3)
			{
				Debug.Log("Đã ăn đủ 3 quả xanh! Qua màn.");
				// Ví dụ chuyển màn:
				// SceneManager.LoadScene("NextSceneName");
			}
		}
	}

	private void UpdateFruitText()
	{
		if (fruitCounterText != null)
		{
			fruitCounterText.text = "x" + fruitCount;
		}
	}
}

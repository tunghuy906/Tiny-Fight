using UnityEngine;

public class ItemBuff : MonoBehaviour
{
	public float buffDuration = 5f; // Thời gian hiệu ứng tồn tại
	public float manaRegenMultiplier = 0.5f; // Giảm thời gian hồi mana (càng nhỏ hồi càng nhanh)
	public float speedBoostMultiplier = 1.5f; // Hệ số tăng tốc độ di chuyển

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player")) // Nếu Player nhặt vật phẩm
		{
			PlayerController playerController = other.GetComponent<PlayerController>();
			ManaBar manaBar = FindObjectOfType<ManaBar>();
			AudioManager.instance.PlaySfx(3);
			if (playerController != null && manaBar != null)
			{
				playerController.ActivateSpeedBoost(buffDuration, speedBoostMultiplier);
				manaBar.ActivateManaRegenBoost(buffDuration, manaRegenMultiplier);
			}

			Destroy(gameObject); // Hủy vật phẩm sau khi nhặt
		}
	}
}

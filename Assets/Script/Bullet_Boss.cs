using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float lifetime = 3f; // Thời gian tồn tại của đạn
	public int damage = 10; // Sát thương của đạn
	public Vector2 knockbackForce = new Vector2(2, 2); // Lực đẩy lùi khi trúng đạn

	void Start()
	{
		Destroy(gameObject, lifetime); // Tự hủy sau thời gian tồn tại
	}

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player")) // Kiểm tra nếu trúng Player
		{
			Damageable playerDamageable = collider.GetComponent<Damageable>();
			if (playerDamageable != null) // Nếu Player có Damageable.cs
			{
				playerDamageable.Hit(damage, knockbackForce); // Gây sát thương
			}

			Destroy(gameObject); // Hủy viên đạn sau khi gây sát thương
		}
	}
}

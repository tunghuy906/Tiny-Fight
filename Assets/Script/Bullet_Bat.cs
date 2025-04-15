using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	public float lifetime = 5f;
	public int damage = 10;
	public Vector2 knockbackForce = new Vector2(2, 2);

	private void Start()
	{
		Destroy(gameObject, lifetime);
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

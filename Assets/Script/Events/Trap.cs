using UnityEngine;

public class Trap : MonoBehaviour
{
	public int damage = 10; // Sát thương khi chạm vào gai
	public Vector2 knockbackForce = new Vector2(0, 5); // Lực đẩy ngược lại khi va chạm

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player")) // Kiểm tra nếu đối tượng va chạm là Player
		{
			Damageable damageable = other.GetComponent<Damageable>();
			if (damageable != null)
			{
				bool wasHit = damageable.Hit(damage, knockbackForce); // Trừ máu và đẩy lùi
				if (wasHit)
				{
					Debug.Log("Player bị gai đâm! -10 HP");
				}
			}
		}
	}
}

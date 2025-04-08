using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(10f, 0);
	public Vector2 knockback = new Vector2(0, 0);
    public float maxDistance = 30f; // Khoảng cách tối đa viên đạn có thể di chuyển
    private Vector2 startPosition;

    Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		// Gán tag "Projectile" cho viên đạn
		gameObject.tag = "Projectile";
	}

	// Start is called before the first frame update
	private void Start()
    {
		startPosition = transform.position;
		rb.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }

	private void Update()
	{
		// Hủy viên đạn nếu vượt quá khoảng cách tối đa
		if (Vector2.Distance(startPosition, transform.position) >= maxDistance)
		{
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		Damageable damageable = collision.GetComponent<Damageable>();

		if (damageable != null)
		{
			Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

			bool gotHit = damageable.Hit(damage, deliveredKnockback);
			if (gotHit)
			{
				Debug.Log(collision.name + "hit for " + damage);
				Destroy(gameObject);
			}
		}
	}
}

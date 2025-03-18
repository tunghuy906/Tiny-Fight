using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float speed = 10f;
	public float lifetime = 2f;
	private int direction;
	private Vector3 bulletScale = new Vector3(0.1f, 0.1f, 1f); // Kích thước nhỏ hơn

	public void SetDirection(int dir)
	{
		direction = dir;
		transform.localScale = new Vector3(bulletScale.x * dir, bulletScale.y, bulletScale.z); // Lật đạn nếu bắn trái nhưng vẫn giữ kích thước nhỏ
	}

	private void Start()
	{
		Destroy(gameObject, lifetime);
	}

	private void Update()
	{
		transform.position += Vector3.right * direction * speed * Time.deltaTime;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Enemy")) // Nếu đạn chạm quái
		{
			Destroy(collision.gameObject); // Tiêu diệt quái
			Destroy(gameObject); // Xóa đạn
		}
	}
}

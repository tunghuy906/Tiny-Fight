using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			AudioManager.instance.PlaySfx(3);
			DontDestroy.Instance.CollectItem();
			Destroy(gameObject);
		}
	}
}

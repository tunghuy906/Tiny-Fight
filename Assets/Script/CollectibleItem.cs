using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			DontDestroy.Instance.CollectItem();
			Destroy(gameObject);
		}
	}
}

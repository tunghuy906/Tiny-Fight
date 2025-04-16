using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
	public Transform[] waypoints; // Các vị trí cố định
	public float moveSpeed = 3f; // Tốc độ di chuyển
	private int currentIndex = 0;

	void Update()
	{
		if (waypoints.Length == 0) return;

		Transform targetPoint = waypoints[currentIndex];
		transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

		if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
		{
			currentIndex = (currentIndex + 1) % waypoints.Length; // Di chuyển sang điểm tiếp theo
		}
	}
}

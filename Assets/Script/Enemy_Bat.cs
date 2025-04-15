using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat_Enemy : MonoBehaviour
{
	public float flightSpeed = 2f;
	public float waypointReachedDistance;
	public List<Transform> waypoints;

	public GameObject bulletPrefab;        
	public Transform firePoint;             
	public float shootInterval = 3f;       

	private Animator animator;
	private Rigidbody2D rb;
	private Damageable damageable;

	private Transform nextWaypoint;
	private int waypointNum = 0;

	private Transform player;

	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);
		}
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		damageable = GetComponent<Damageable>();
	}

	private void Start()
	{
		nextWaypoint = waypoints[waypointNum];

		player = GameObject.FindGameObjectWithTag("Player")?.transform;

		StartCoroutine(ShootRoutine());
	}

	private void FixedUpdate()
	{
		if (damageable.IsAlive)
		{
			if (CanMove)
			{
				Flight();
			}
			else
			{
				rb.velocity = Vector2.zero;
			}
		}
		else
		{
			rb.gravityScale = 2f;
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	private void Flight()
	{
		Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

		float distance = Vector2.Distance(nextWaypoint.position, transform.position);

		rb.velocity = directionToWaypoint * flightSpeed;

		if (distance <= waypointReachedDistance)
		{
			waypointNum++;

			if (waypointNum >= waypoints.Count)
			{
				waypointNum = 0;
			}

			nextWaypoint = waypoints[waypointNum];
		}
	}

	private IEnumerator ShootRoutine()
	{
		while (true)
		{
			yield return new WaitForSeconds(shootInterval);

			if (player != null && damageable.IsAlive)
			{
				animator.SetTrigger("Attack"); // Phát animation tấn công

				// Tính hướng tới player
				Vector2 direction = (player.position - firePoint.position).normalized;

				// Tạo viên đạn và gán vận tốc
				GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
				Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
				if (bulletRb != null)
				{
					bulletRb.velocity = direction * 5f;
				}
			}
		}
	}
}

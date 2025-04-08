using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBat_Enemy : MonoBehaviour
{
	public float flightSpeed = 2f;
	public float waypointReachedDistance;
	public List<Transform> waypoints;
	public GameObject bulletPrefab;
	public Transform firePoint;
	public float bulletSpeed = 5f;
	public float shootInterval = 3f;
	public int bulletsPerWave = 12;
	public float switchModeTime = 5f;

	private Animator animator;
	private Rigidbody2D rb;
	private Damageable damageable;
	private Transform nextWaypoint;
	private int waypointNum = 0;
	private bool isCircularShooting = true;
	private Transform player;

	public bool CanMove
	{
		get { return animator.GetBool(AnimationStrings.canMove); }
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
		player = GameObject.FindGameObjectWithTag("Player").transform;
		StartCoroutine(ShootingPattern());
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
				rb.velocity = Vector3.zero;
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

	private IEnumerator ShootingPattern()
	{
		while (true)
		{
			animator.SetTrigger("Attack"); // Kích hoạt animation tấn công

			if (isCircularShooting)
			{
				ShootCircular();
			}
			else
			{
				ShootStraight();
			}

			yield return new WaitForSeconds(shootInterval);
		}
	}

	private void ShootCircular()
	{
		float angleStep = 360f / bulletsPerWave;
		for (int i = 0; i < bulletsPerWave; i++)
		{
			float angle = i * angleStep;
			Vector2 shootDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
			ShootBullet(shootDirection);
		}
		StartCoroutine(SwitchShootingMode());
	}

	private void ShootStraight()
	{
		if (player == null) return;
		Vector2 direction = (player.position - transform.position).normalized;

		// Số lượng viên đạn trong một loạt bắn
		int bulletsInRow = 5;
		float delayBetweenBullets = 0.1f; // Độ trễ giữa từng viên đạn

		StartCoroutine(ShootBulletRow(direction, bulletsInRow, delayBetweenBullets));

		StartCoroutine(SwitchShootingMode());
	}

	// Coroutine bắn nhiều viên đạn liên tiếp
	private IEnumerator ShootBulletRow(Vector2 direction, int count, float delay)
	{
		for (int i = 0; i < count; i++)
		{
			ShootBullet(direction);
			yield return new WaitForSeconds(delay);
		}
	}

	private void ShootBullet(Vector2 direction)
	{
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
		Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
		rbBullet.velocity = direction * bulletSpeed;


	}

	private IEnumerator SwitchShootingMode()
	{
		yield return new WaitForSeconds(switchModeTime);
		isCircularShooting = !isCircularShooting;
	}
}

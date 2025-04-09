using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBat_Enemy : MonoBehaviour
{
	public float flightSpeed = 2f;
	public float waypointReachedDistance;
	public GameObject bulletPrefab;
	public Transform firePoint;
	public float bulletSpeed = 5f;
	public float shootInterval = 3f;
	public int bulletsPerWave = 12;
	public float switchModeTime = 5f;
	public float chaseDistance = 10f; // Khoảng cách để bắt đầu đuổi theo người chơi
	public float evadeDistance = 3f; // Khoảng cách để né đạn
	public float chaseDuration = 5f; // Thời gian tối đa boss đuổi theo người chơi
	public float safeDistance = 3f; // Khoảng cách an toàn tối thiểu giữa boss và người chơi
	public float bossZoneRadius = 15f; // Bán kính vùng boss có thể di chuyển
	public float randomMoveInterval = 2f; // Thời gian giữa các lần chọn vị trí di chuyển ngẫu nhiên
	private Vector2 randomTargetPosition;
	private float randomMoveTimer = 0f;
	private float chaseTimer = 0f;
	private Vector2 initialPosition;
	private float bulletDetectionCooldown = 0.2f; // Thời gian giữa các lần kiểm tra đạn
	private float bulletDetectionTimer = 0f;
	private float evadeCooldown = 0.5f; // Thời gian chờ giữa các lần né
	private float evadeTimer = 0f;

	private Animator animator;
	private Rigidbody2D rb;
	private Damageable damageable;
	private bool isCircularShooting = true;
	private Transform player;
	private SpriteRenderer spriteRenderer; // To flip the boss sprite

	public bool CanMove
	{
		get { return animator.GetBool(AnimationStrings.canMove); }
	}

	private void Awake()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		damageable = GetComponent<Damageable>();
		spriteRenderer = GetComponent<SpriteRenderer>(); // Initialize sprite renderer
	}

	private void Start()
	{
		initialPosition = transform.position;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		SetRandomTargetPosition(); // Đặt vị trí ngẫu nhiên ban đầu
		StartCoroutine(ShootingPattern());
	}

	private void FixedUpdate()
	{
		if (damageable.IsAlive)
		{
			if (CanMove)
			{
				evadeTimer += Time.fixedDeltaTime;
				bulletDetectionTimer += Time.fixedDeltaTime;
				randomMoveTimer += Time.fixedDeltaTime;

				if (IsPlayerInBossZone() && IsPlayerInChaseRange() && chaseTimer < chaseDuration)
				{
					ChasePlayer();
					chaseTimer += Time.fixedDeltaTime;
				}
				else if (IsPlayerInBossZone() && // Check if player is in the boss zone
				         evadeTimer >= evadeCooldown && 
				         bulletDetectionTimer >= bulletDetectionCooldown && 
				         IsBulletNearby(out Vector2 evadeDirection))
				{
					evadeTimer = 0f; // Reset evade timer
					bulletDetectionTimer = 0f; // Reset bullet detection timer
					EvadeBullet(evadeDirection);
				}
				else
				{
					chaseTimer = 0f; // Reset timer khi không đuổi theo
					DynamicMovement();
				}

				FlipSpriteBasedOnPlayer(); // Flip sprite based on player's position
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

	private bool IsPlayerInChaseRange()
	{
		if (player == null) return false;
		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
		return distanceToPlayer <= chaseDistance;
	}

	private bool IsPlayerInBossZone()
	{
		if (player == null) return false;
		float distanceToInitial = Vector2.Distance(initialPosition, player.position);
		return distanceToInitial <= bossZoneRadius;
	}

	private void ChasePlayer()
	{
		if (player == null) return;

		float distanceToPlayer = Vector2.Distance(transform.position, player.position);
		if (distanceToPlayer > safeDistance)
		{
			Vector2 directionToPlayer = (player.position - transform.position).normalized;
			rb.velocity = directionToPlayer * flightSpeed;
		}
		else
		{
			// Nếu trong khoảng cách an toàn, di chuyển ngẫu nhiên để tránh dính người chơi
			DynamicMovement();
		}
	}

	private bool IsBulletNearby(out Vector2 evadeDirection)
	{
		evadeDirection = Vector2.zero;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, evadeDistance);
		foreach (var collider in colliders)
		{
			// Kiểm tra nếu collider có tag "Projectile"
			if (collider.CompareTag("Projectile"))
			{
				Debug.Log("Bullet detected: " + collider.name);
				// Tính toán hướng né
				Rigidbody2D bulletRb = collider.GetComponent<Rigidbody2D>();
				if (bulletRb != null)
				{
					Vector2 bulletDirection = bulletRb.velocity.normalized;
					float randomAngle = Random.Range(-30f, 30f); // Thêm góc ngẫu nhiên từ -30 đến 30 độ
					evadeDirection = Quaternion.Euler(0, 0, randomAngle) * Vector2.Perpendicular(bulletDirection).normalized;
					return true;
				}
			}
		}
		Debug.Log("No bullets detected");
		return false;
	}

	private void EvadeBullet(Vector2 evadeDirection)
	{
		rb.velocity = evadeDirection * flightSpeed;
	}

	private void DynamicMovement()
	{
		if (randomMoveTimer >= randomMoveInterval)
		{
			SetRandomTargetPosition();
			randomMoveTimer = 0f;
		}

		Vector2 directionToTarget = (randomTargetPosition - (Vector2)transform.position).normalized;
		rb.velocity = directionToTarget * flightSpeed;

		// Nếu đã đến gần vị trí mục tiêu, chọn vị trí mới
		if (Vector2.Distance(transform.position, randomTargetPosition) <= waypointReachedDistance)
		{
			SetRandomTargetPosition();
		}
	}

	private void SetRandomTargetPosition()
	{
		Vector2 randomOffset = Random.insideUnitCircle * bossZoneRadius;
		randomTargetPosition = initialPosition + randomOffset;
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

	private void FlipSpriteBasedOnPlayer()
	{
		if (player == null) return;

		// Flip sprite if player is on the opposite side
		if ((player.position.x < transform.position.x && spriteRenderer.flipX) ||
			(player.position.x > transform.position.x && !spriteRenderer.flipX))
		{
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
	}

	private void OnDrawGizmosSelected()
	{
		// Vẽ phạm vi đuổi theo người chơi
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, chaseDistance);

		// Vẽ phạm vi né đạn
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, evadeDistance);

		// Vẽ khoảng cách an toàn
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, safeDistance);

		// Vẽ vùng boss có thể di chuyển
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(initialPosition, bossZoneRadius);
	}
}

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Slinger_Enemy : MonoBehaviour
{
	public float walkSpeed = 3f;
	public float walkStopRate = 0.05f;
	public GameObject projectilePrefab;
	public Transform shootPoint;
	public float shootCooldown = 2f;

	private float nextShootTime = 0f;
	private bool _hasTarget = false;

	public DirectionZone attackZone;
	public DirectionZone cliffDetectionZone;

	Rigidbody2D rb;
	TouchingDirections touchingDirections;
	Animator animator;
	Damageable damageable;

	public enum WalkableDirection { Right, Left }

	private WalkableDirection _walkDirection;
	private Vector2 walkDirectionVector = Vector2.right;

	public WalkableDirection WalkDirection
	{
		get => _walkDirection;
		set
		{
			if (_walkDirection != value)
			{
				transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
				walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
			}
			_walkDirection = value;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		touchingDirections = GetComponent<TouchingDirections>();
		animator = GetComponent<Animator>();
		damageable = GetComponent<Damageable>();
	}

	private void Update()
	{
		HasTarget = attackZone.detectedColliders.Count > 0;

		if (AttackCooldown > 0)
		{
			AttackCooldown -= Time.deltaTime;
		}

		// Bắt đầu bắn nếu đủ thời gian cooldown
		if (HasTarget && Time.time >= nextShootTime && !damageable.LockVelocity)
		{
			StartAttack();
		}
	}

	private void FixedUpdate()
	{
		if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
		{
			FlipDirection();
		}

		if (!damageable.LockVelocity)
		{
			if (CanMove && touchingDirections.IsGrounded)
			{
				rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
			}
			else
			{
				rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
			}
		}
	}

	private void FlipDirection()
	{
		WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkDirection = WalkableDirection.Right;
	}

	public bool CanMove => animator.GetBool(AnimationStrings.canMove);

	public float AttackCooldown
	{
		get => animator.GetFloat(AnimationStrings.attackCooldown);
		private set => animator.SetFloat(AnimationStrings.attackCooldown, MathF.Max(value, 0));
	}

	public bool HasTarget
	{
		get => _hasTarget;
		private set
		{
			_hasTarget = value;
			animator.SetBool(AnimationStrings.hasTarget, value);
		}
	}

	private void StartAttack()
	{
		if (animator != null && projectilePrefab != null && shootPoint != null)
		{
			animator.SetTrigger("Attack");
			damageable.LockVelocity = true;
			AttackCooldown = shootCooldown;
			nextShootTime = Time.time + shootCooldown;
			StartCoroutine(PerformShoot());
		}
	}

	private IEnumerator PerformShoot()
	{
		yield return new WaitForSeconds(0.3f); // Delay đạn để đồng bộ với animation

		if (projectilePrefab != null && shootPoint != null)
		{
			GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
			if (bulletRb != null)
			{
				bulletRb.velocity = walkDirectionVector * 10f;
			}
		}
	}

	// Gọi bằng animation event cuối animation Attack
	public void UnlockVelocity()
	{
		damageable.LockVelocity = false;
	}

	public void OnHit(int damage, Vector2 knockback)
	{
		rb.velocity = Vector2.zero;
		rb.AddForce(new Vector2(knockback.x * -Mathf.Sign(transform.localScale.x), knockback.y), ForceMode2D.Impulse);
	}

	public void OnCliffDetected()
	{
		if (touchingDirections.IsGrounded)
		{
			FlipDirection();
		}
	}
}

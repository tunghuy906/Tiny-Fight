using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Slinger_Enemy : MonoBehaviour
{
	public float walkSpeed = 3f;
	public float walkStopRate = 0.05f;
	public GameObject projectilePrefab;  // Prefab đạn
	public Transform shootPoint;         // Vị trí bắn đạn
	public float shootCooldown = 2f;     // Thời gian chờ giữa các lần bắn
	private float nextShootTime = 0f;    // Thời gian bắn tiếp theo

	public DirectionZone cliffDetectionZone;
	Damageable damageable;

	Rigidbody2D rb;
	TouchingDirections touchingDirections;
	Animator animator;

	public enum WalkableDirection { Right, Left }

	private WalkableDirection _walkDirection;
	private Vector2 walkDirectionVector = Vector2.right;

	public WalkableDirection WalkDirection
	{
		get { return _walkDirection; }
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

		// Kiểm tra thời gian và thực hiện bắn đạn
		if (Time.time >= nextShootTime)
		{
			TriggerAttack();
			nextShootTime = Time.time + shootCooldown;
		}
	}

	private void FlipDirection()
	{
		WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
	}

	public bool CanMove
	{
		get { return animator.GetBool(AnimationStrings.canMove); }
	}

	public float AttackCooldown
	{
		get { return animator.GetFloat(AnimationStrings.attackCooldown); }
		private set { animator.SetFloat(AnimationStrings.attackCooldown, MathF.Max(value, 0)); }
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

	// Gọi animation bắn đạn
	private void TriggerAttack()
	{
		animator.SetTrigger("Attack"); // Kích hoạt animation bắn
		StartCoroutine(PerformShoot()); // Đợi animation rồi bắn
	}

	// Thực hiện bắn đạn sau animation
	private IEnumerator PerformShoot()
	{
		yield return new WaitForSeconds(0.3f); // Chờ 0.3s để animation diễn ra

		if (projectilePrefab != null && shootPoint != null)
		{
			GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
			if (bulletRb != null)
			{
				bulletRb.velocity = walkDirectionVector * 10f; // Tốc độ đạn
			}
		}
	}
}

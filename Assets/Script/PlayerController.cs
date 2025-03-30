using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
	public float walkSpeed = 5f;
	public float dashSpeed = 15f;
	public float airWalkSpeed = 3f;
	public float dashTime = 0.2f;
	public float dashCooldown = 1f;

	private Vector2 moveInput;
	private bool _isMoving = false;
	private bool isDashing = false;
	private bool canDash = true;

	public float jumpInpulse = 10f;

	public int maxJumps = 2; // Số lần nhảy tối đa
	public int jumpCount = 0; // Đếm số lần nhảy

	TouchingDirections touchingDirections;
	Damageable damageable;

	private Rigidbody2D rb;
	private Animator animator;

	public float CurrentMoveSpeed
	{
		get {
			if (CanMove)
			{
				if (IsMoving && !touchingDirections.IsOnWall)
				{
					if (touchingDirections.IsGrounded)
					{
						if (isDashing)
						{
							return dashSpeed;
						}
						else
						{
							return walkSpeed;
						}
					}
					else
					{
						return airWalkSpeed;
					}
				}
				else
				{
					return 0;
				}
			}
			else
			{
				return 0;
			}
		}
	}

	public bool IsAlive
	{
		get
		{
			return animator.GetBool(AnimationStrings.isAlive);
		}
	}

	public bool IsMoving
	{
		get { return _isMoving; }
		private set
		{
			_isMoving = value;
			animator.SetBool(AnimationStrings.isMoving, value);
		}
	}

	public bool CanMove
	{
		get
		{
			return animator.GetBool(AnimationStrings.canMove);

		}
	}
	private bool _isFacingRight = true;

	public bool IsFacingRight
	{
		get { return _isFacingRight; }
		private set
		{
			if (_isFacingRight != value)
			{
				transform.localScale *= new Vector2(-1, 1);
			}
			_isFacingRight = value;
		}
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		touchingDirections = GetComponent<TouchingDirections>();
		damageable = GetComponent<Damageable>();
	}

	private void FixedUpdate()
	{
		if (touchingDirections.IsGrounded)
		{
			jumpCount = 0;
		}

		if (!isDashing && !damageable.LockVelocity)
		{
			rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
		}

		animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
		if (IsAlive)
		{
			IsMoving = moveInput != Vector2.zero;
			SetFacingDirection(moveInput);
		}
		else
		{
			IsMoving = false;
		}
		
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (context.performed && canDash)
		{
			StartCoroutine(Dash());
		}
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started && CanMove && (touchingDirections.IsGrounded || jumpCount < maxJumps))
		{
			animator.SetTrigger(AnimationStrings.jumpTrigger);
			rb.velocity = new Vector2(rb.velocity.x, jumpInpulse);
			jumpCount++;
		}
	}

	public void OnRangedAttack(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
		}
	}

	private IEnumerator Dash()
	{
		canDash = false;
		isDashing = true;
		animator.SetBool(AnimationStrings.isDashing, true);

		float originalGravity = rb.gravityScale;
		rb.gravityScale = 0;
		rb.velocity = new Vector2(IsFacingRight ? dashSpeed : -dashSpeed, 0);

		yield return new WaitForSeconds(dashTime);

		rb.gravityScale = originalGravity;
		isDashing = false;
		animator.SetBool(AnimationStrings.isDashing, false);

		yield return new WaitForSeconds(dashCooldown);
		canDash = true;
	}

	private void SetFacingDirection(Vector2 moveInput)
	{
		if (moveInput.x > 0 && !IsFacingRight)
		{
			IsFacingRight = true;
		}
		else if (moveInput.x < 0 && IsFacingRight)
		{
			IsFacingRight = false;
		}
	}

	public void OnHit(int damage, Vector2 knockback)
	{
		rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
	}
}

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

	[SerializeField] private float climbSpeed = 3f;
	[SerializeField] private LayerMask ladderLayer;
	private bool isClimbing = false;
	private bool wasOnLadder = false;

	private Vector2 moveInput;
	private bool _isMoving = false;
	private bool isDashing = false;
	private bool canDash = true;

	public float jumpInpulse = 10f;
	[HideInInspector] public bool playerUnlocked;

	public int manaCostPerShot = 10; 
	private ManaBar manaBar; 

	public int maxJumps = 2; 
	public int jumpCount = 0; 

	TouchingDirections touchingDirections;
	Damageable damageable;

	private Rigidbody2D rb;
	private Animator animator;

	public int skillPoints;

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

	private bool IsOnLadder()
	{
		return Physics2D.OverlapCircle(transform.position, 0.5f, ladderLayer) != null;
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
		manaBar = FindObjectOfType<ManaBar>();
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

		if (isClimbing)
		{
			rb.velocity = new Vector2(0, moveInput.y * climbSpeed); // Chỉ di chuyển theo trục Y
		}

		// Nếu vừa rời khỏi thang, bật lại trọng lực
		if (wasOnLadder && !isClimbing)
		{
			rb.gravityScale = 1;
		}

		wasOnLadder = isClimbing;

	}

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
		if (IsAlive)
		{
			if (isClimbing)
			{
				rb.velocity = new Vector2(0, moveInput.y * climbSpeed);
			}
			else
			{
				IsMoving = moveInput != Vector2.zero;
				SetFacingDirection(moveInput);
			}
		}
		else
		{
			IsMoving = false;
		}
		
	}

	public void OnClimb(InputAction.CallbackContext context)
	{
		if (context.started)
		{
			if (IsOnLadder())
			{
				isClimbing = !isClimbing;
				rb.gravityScale = isClimbing ? 0 : 1; // Tắt trọng lực khi leo
				rb.velocity = Vector2.zero; // Reset vận tốc để tránh bị trượt
				animator.SetBool(AnimationStrings.isClimbing, isClimbing);
			}
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
		if (context.started && IsAlive && manaBar != null)
		{
			if (manaBar.currentMana >= manaCostPerShot) // Kiểm tra đủ mana không
			{
				manaBar.UseMana(manaCostPerShot); // Trừ mana trước khi bắn
				animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
			}
			else
			{
				Debug.Log("Không đủ mana để bắn!");
			}
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

		if (damageable.Health <= 0) // Kiểm tra nếu máu = 0
		{
			GameManager.instance.ShowDeathUI(); // Gọi hàm hiển thị UI
		}
	}

	public void ActivateSpeedBoost(float duration, float multiplier)
	{
		StartCoroutine(SpeedBoostCoroutine(duration, multiplier));
	}

	private IEnumerator SpeedBoostCoroutine(float duration, float multiplier)
	{
		walkSpeed *= multiplier;
		airWalkSpeed *= multiplier;
		dashSpeed *= multiplier;

		Debug.Log("Speed Buff Activated!");

		yield return new WaitForSeconds(duration);

		walkSpeed /= multiplier;
		airWalkSpeed /= multiplier;
		dashSpeed /= multiplier;

		Debug.Log("Speed Buff Expired.");
	}

}

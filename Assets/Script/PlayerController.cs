using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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
	[SerializeField] private LayerMask groundLayer; 
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
	private Collider2D playerCollider;

	public int skillPoints;

	private Ladder currentLadder; 

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
		manaBar = FindObjectOfType<ManaBar>();
		playerCollider = GetComponent<Collider2D>();
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
				// Di chuyển theo trục Y khi trèo nếu có input
				if (moveInput.y != 0)
				{
					rb.velocity = new Vector2(0, moveInput.y * climbSpeed);
				}
				else
				{
					// Giữ nguyên vị trí khi không có input
					rb.velocity = Vector2.zero;
				}

				// Đảm bảo trọng lực luôn bằng 0 khi trèo
				rb.gravityScale = 0;

				// Tạm thời vô hiệu hóa va chạm với ground khi trèo
				Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);

				animator.SetBool(AnimationStrings.isClimbing, true);

				// Chỉ dừng trèo khi rời khỏi thang
				if (currentLadder == null || (!IsOnLadder() && moveInput.y == 0))
				{
					SetClimbingState(false);
				}
			}
			else
			{
				// Đảm bảo tắt trạng thái trèo khi không trèo
				animator.SetBool(AnimationStrings.isClimbing, false);

				// Bật lại va chạm với ground khi không trèo
				Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
			}

		if (wasOnLadder && !isClimbing)
		{
			rb.gravityScale = 3; 
		}

		wasOnLadder = isClimbing;
	}
	void Start()
	{
		SetToSpawnPoint();
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
		if (context.started && currentLadder != null)
		{
			// Căn chỉnh nhân vật vào giữa thang
			transform.position = new Vector2(currentLadder.transform.position.x, transform.position.y);

			SetClimbingState(true);
		}
		else if (context.canceled && isClimbing)
		{
			// Ngừng trèo
			if (!IsOnLadder())
			{
				SetClimbingState(false);
			}
		}
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (context.performed && canDash)
		{
			AudioManager.instance.PlaySfx(0);
			StartCoroutine(Dash());
		}
	}
	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.started && CanMove && (touchingDirections.IsGrounded || jumpCount < maxJumps))
		{
			AudioManager.instance.PlaySfx(1);
			animator.SetTrigger(AnimationStrings.jumpTrigger);
			rb.velocity = new Vector2(rb.velocity.x, jumpInpulse);
			jumpCount++;
		}
	}

	public void OnRangedAttack(InputAction.CallbackContext context)
	{
		if (context.started && IsAlive && manaBar != null)
		{
			// ❌ Nếu đang pause => không bắn
			if (Ui_Main.isGamePaused) return;

			// ❌ Nếu đang ấn vào bất kỳ UI nào => không bắn
			if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
			{
				Debug.Log("Click vào UI nên không bắn");
				return;
			}

			// ✅ Điều kiện đủ để bắn
			if (manaBar.currentMana >= manaCostPerShot)
			{
				AudioManager.instance.PlaySfx(4);
				manaBar.UseMana(manaCostPerShot);
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

	public void SetClimbingState(bool state)
	{
		isClimbing = state;
		rb.gravityScale = state ? 0 : 3; 
		rb.velocity = Vector2.zero; 
		animator.SetBool(AnimationStrings.isClimbing, state); 
		// Vô hiệu hóa hoặc bật lại va chạm với ground
		Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), state);
	}

	public void SetNearLadder(Ladder ladder)
	{
		currentLadder = ladder;
		if (ladder != null)
		{
			Debug.Log("Player is near a ladder.");
		}
		else
		{
			Debug.Log("Player left the ladder area.");
		}
	}

	private bool IsOnLadder()
	{
		// Tìm các collider trong bán kính xung quanh nhân vật
		Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
		foreach (var collider in colliders)
		{
			// Kiểm tra nếu collider có tag "Ladder"
			if (collider.CompareTag("Ladder"))
			{
				return true;
			}
		}
		return false;
	}
	void SetToSpawnPoint()
	{
		GameObject spawn = GameObject.Find("PlayerSpawnPoint");
		if (spawn != null)
		{
			transform.position = spawn.transform.position;
		}
		else
		{
			Debug.LogWarning("Không tìm thấy PlayerSpawnPoint trong scene!");
		}
	}

}

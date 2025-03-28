using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
	public float walkSpeed = 5f;
	public float dashSpeed = 15f;
	public float dashTime = 0.2f;
	public float dashCooldown = 1f;

	private Vector2 moveInput;
	private bool _isMoving = false;
	private bool isDashing = false;
	private bool canDash = true;

	public float jumpInputlse = 10f;

	TouchingDirections touchingDirections; 

	private Rigidbody2D rb;
	private Animator animator;

	public bool IsMoving
	{
		get { return _isMoving; }
		private set
		{
			_isMoving = value;
			animator.SetBool(AnimationStrings.isMoving, value);
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
	}

	private void FixedUpdate()
	{
		if (!isDashing)
		{
			rb.velocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);
		}

		animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
		IsMoving = moveInput != Vector2.zero;
		SetFacingDirection(moveInput);
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
		if(context.started && touchingDirections.IsGrounded)
		{
			animator.SetTrigger(AnimationStrings.jump);
			rb.velocity = new Vector2(rb.velocity.x, jumpInputlse);
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
}

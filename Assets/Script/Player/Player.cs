using UnityEngine;

public class Player : Entity
{

	[Header("Move info")]
	public float moveSpeed = 12f;

	[Header("Jump Info")]
	public float jumpForce;
	public int maxJumps = 2;
	public int jumpCount = 0;

	[Header("Dash info")]
	[SerializeField] private float dashCooldown;
	private float dashUsageTimer;
	public float dashSpeed;
	public float dashDuration;
	public float dashDir {  get; private set; }

	

	[Header("Shooting Info")]
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private Transform firePoint;
	public bool isShooting;



	#region States
	public PlayerStateMachine stateMachine {  get; private set; }

	public PlayerIdleState idleState { get; private set; }
	public PlayerMoveState moveState { get; private set; }
	public PlayerJumpState jumpState { get; private set; }
	public PlayerAirState airState { get; private set; }
	public PlayerDashState dashState { get; private set; }
	public PlayerDoubleJumpState doubleJumpState { get; private set; }
	public PlayerShootingState shootingState { get; private set; }
	#endregion

	protected override void Awake()
	{
		base.Awake();
		stateMachine = new PlayerStateMachine();

		idleState = new PlayerIdleState(stateMachine, this, "Idle");
		moveState = new PlayerMoveState(stateMachine, this, "Move");
		jumpState = new PlayerJumpState(stateMachine, this, "Jump");
		airState = new PlayerAirState(stateMachine, this, "Jump");
		dashState = new PlayerDashState(stateMachine, this, "Dash");
		doubleJumpState = new PlayerDoubleJumpState(stateMachine, this, "DoubleJump");
		shootingState = new PlayerShootingState(stateMachine, this, "Shoot");
	}

	protected override void Start()
	{

		base.Start();
		
		stateMachine.Initialize(idleState);
	}

	private float timer;
	private float cooldown;

	protected override void Update()
	{
		base.Update();
		stateMachine.currentState.Update();

		CheckForDashInput();
		HandleJumpInput();	
		HandleShootingInput();
	}
	private void HandleShootingInput()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0)) 
		{
			isShooting = true;
			anim.SetBool("Shoot", true);
			stateMachine.ChangeState(shootingState);
			ShootBullet();
		}
		else if (Input.GetKeyUp(KeyCode.Mouse0)) 
		{
			isShooting = false;
			anim.SetBool("Shoot", false);
		}
	}
	private void ShootBullet()
	{
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
		bullet.GetComponent<Bullet>().SetDirection(facingDir);
	}
	private void HandleJumpInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (IsGroundDetected() || jumpCount < maxJumps)
			{
				if (jumpCount == 0)
				{
					stateMachine.ChangeState(jumpState);
				}
				else if (jumpCount == 1)
				{
					stateMachine.ChangeState(doubleJumpState);
				}
			}
		}
	}
	private void CheckForDashInput()
	{
		dashUsageTimer -= Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.LeftShift) && dashUsageTimer < 0)
		{
			dashUsageTimer = dashCooldown;
			dashDir = Input.GetAxisRaw("Horizontal");

			if(dashDir == 0)
			{
				dashDir = facingDir;
			}

			stateMachine.ChangeState(dashState);
		}
	} 
	public bool IsGroundDetected()
	{
		bool grounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
		if (grounded)
		{
			jumpCount = 0;
			anim.SetBool("Jump", false);
			anim.SetBool("DoubleJump", false);
		}
		return grounded;
	}


}

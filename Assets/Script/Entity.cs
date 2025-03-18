using UnityEngine;

public class Entity : MonoBehaviour
{


	#region Components
	public Animator anim { get; private set; }
	public Rigidbody2D rb { get; private set; }

	#endregion

	[Header("Collision info")]
	[SerializeField] protected Transform groundCheck;
	[SerializeField] protected float groundCheckDistance;
	[SerializeField] protected Transform wallCheck;
	[SerializeField] protected float wallCheckDistance;
	[SerializeField] protected LayerMask whatIsGround;

	public int facingDir { get; private set; } = 1;
	protected bool facingRight = true;
	protected virtual void Awake()
    {

    }

    protected virtual void Start()
    {
		anim = GetComponentInChildren<Animator>();
		rb = GetComponent<Rigidbody2D>();
	}

    protected virtual void Update()
    {

    }


	#region Collision
	public virtual bool IsGroundDetected2() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

	public virtual bool IsWallDectected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
		Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
	}
	#endregion

	#region Flip
	public virtual void Flip()
	{
		facingDir = facingDir * -1;
		facingRight = !facingRight;
		transform.Rotate(0, 180, 0);
	}

	public virtual void FlipController(float _x)
	{
		if (_x > 0 && !facingRight)
		{
			Flip();
		}
		else if (_x < 0 && facingRight)
		{
			Flip();
		}
	}
	#endregion

	#region Velocity
	public void SetVelocity(float _xVelocity, float _yVelocity)
	{
		rb.linearVelocity = new Vector2(_xVelocity, _yVelocity);
		FlipController(_xVelocity);
	}
	#endregion

}

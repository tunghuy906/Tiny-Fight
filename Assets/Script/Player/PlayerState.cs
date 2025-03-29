using UnityEngine;

public class PlayerState
{
	protected PlayerStateMachine stateMachine;
	protected Player player;

	protected Rigidbody2D rb;

	protected float xInput;
	private string animaBoolName;

	protected float stateTimer;
	public PlayerState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName)
	{
		this.stateMachine = _stateMachine;
		this.player = _player;
		this.animaBoolName = _animaBoolName;
	}

	public virtual void Enter()
	{
		player.anim.SetBool(animaBoolName, true);
		rb = player.rb;
	}

	public virtual void Update()
	{
		stateTimer -= Time.deltaTime;

		xInput = Input.GetAxisRaw("Horizontal");

		player.anim.SetFloat("yVelocity", rb.velocity.y);
	}

	public virtual void Exit()
	{
		player.anim.SetBool(animaBoolName, false);
	}
}

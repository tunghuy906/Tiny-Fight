using UnityEngine;

public class PlayerAirState : PlayerGroundedState
{
	public PlayerAirState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName) : base(_stateMachine, _player, _animaBoolName)
	{
	}

	public override void Enter()
	{
		base.Enter();
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		float moveInput = Input.GetAxisRaw("Horizontal");
		player.SetVelocity(moveInput * player.moveSpeed, rb.linearVelocity.y);

		if (player.IsGroundDetected())
		{
			stateMachine.ChangeState(player.idleState);
		}
	}
}

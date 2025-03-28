using UnityEngine;

public class PlayerDashState : PlayerState
{
	public PlayerDashState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName) : base(_stateMachine, _player, _animaBoolName)
	{
	}

	public override void Enter()
	{
		base.Enter();

		stateTimer = player.dashDuration;
	}

	public override void Exit()
	{
		base.Exit();

		player.SetVelocity(0, rb.velocity.y);
	}

	public override void Update()
	{
		base.Update();

		player.SetVelocity(player.dashSpeed * player.dashDir, 0);

		if(stateTimer < 0)
		{
			stateMachine.ChangeState(player.idleState);
		}
	}
}

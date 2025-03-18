using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
	public PlayerMoveState(PlayerStateMachine stateMachine, Player player, string animaBoolName) : base(stateMachine, player, animaBoolName)
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

		player.SetVelocity(xInput * player.moveSpeed, player.rb.linearVelocity.y);

		if (xInput == 0)
		{
			stateMachine.ChangeState(player.idleState);
		}
	}
}

using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
	public PlayerIdleState(PlayerStateMachine stateMachine, Player player, string animaBoolName) : base(stateMachine, player, animaBoolName)
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

		if (xInput != 0)
		{
			stateMachine.ChangeState(player.moveState);
		}
	}
}

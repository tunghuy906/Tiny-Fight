using UnityEngine;

public class PlayerGroundedState : PlayerState
{
	public PlayerGroundedState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName) : base(_stateMachine, _player, _animaBoolName)
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

		if (!player.IsGroundDetected())
		{
			stateMachine.ChangeState(player.airState);
		}

		if(Input.GetKeyDown(KeyCode.Space) && player.IsGroundDetected())
		{
			stateMachine.ChangeState(player.jumpState);
		}
	}
}

using UnityEngine;
using UnityEngine.Playables;

public class PlayerDoubleJumpState : PlayerGroundedState
{
	public PlayerDoubleJumpState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName) : base(_stateMachine, _player, _animaBoolName)
	{
	}

	public override void Enter()
	{
		base.Enter();

		if (player.jumpCount < player.maxJumps)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.jumpForce);
			player.jumpCount++;

			player.anim.SetBool("Jump", false);
			player.anim.SetBool("DoubleJump", true);
		}
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();
		if (rb.linearVelocity.y < 0)
		{
			stateMachine.ChangeState(player.airState);
		}
	}
}

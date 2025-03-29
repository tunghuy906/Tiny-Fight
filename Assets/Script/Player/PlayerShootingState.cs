using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerShootingState : PlayerState
{
	private float nextFireTime;
	private float fireRate = 0.5f; 
	private Transform firePoint;
	private GameObject bulletPrefab;

	public PlayerShootingState(PlayerStateMachine _stateMachine, Player _player, string _animaBoolName)
		: base(_stateMachine, _player, _animaBoolName)
	{
	}

	public override void Enter()
	{
		base.Enter();
		FireBullet();
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		if (Input.GetKey(KeyCode.Mouse0)) 
		{
			if (Time.time >= nextFireTime) 
			{
				FireBullet();
			}
		}
		else
		{
			stateMachine.ChangeState(player.idleState); 
		}
	}

	private void FireBullet()
	{
		nextFireTime = Time.time + fireRate;

		if (firePoint != null && bulletPrefab != null)
		{
			GameObject bullet = Object.Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
			Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

			if (rb != null)
			{
				rb.velocity = new Vector2(player.facingDir * 10f, 0); 
				bullet.transform.localScale = new Vector3(player.facingDir, 1, 1);
			}
		}
	}
}

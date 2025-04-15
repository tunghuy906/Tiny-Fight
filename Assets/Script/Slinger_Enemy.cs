using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class Slinger_Enemy : MonoBehaviour
{
    public float walkSpeed = 1.5f;
    public float walkStopRate = 0.05f;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootCooldown = 2f;
    private float nextShootTime = 0f;

    public float detectionRange = 5f; 
    public DirectionZone cliffDetectionZone;
    Damageable damageable;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;

    public enum WalkableDirection { Right, Left }

    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                walkDirectionVector = (value == WalkableDirection.Right) ? Vector2.right : Vector2.left;
            }
            _walkDirection = value;
        }
    }

    private Transform player;

    private bool _hasTarget = false;
    
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // Subscribe to the damageable OnDeath event
        damageable.healthChanged.AddListener(OnHealthChanged);
    }
    
    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        // Check if the enemy has died
        if (currentHealth <= 0 && gameObject != null)
        {
            // Trigger death animation
            animator.SetBool(AnimationStrings.isAlive, false);
            
        }
    }


    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            // Update HasTarget based on player detection
            HasTarget = IsPlayerInRange();
            
            if (HasTarget)
            {
                // Stop moving and set to idle animation when detecting player
                rb.velocity = Vector2.zero;
                animator.SetBool(AnimationStrings.isMoving, false);
                
                // Face the player
                if (player.position.x > transform.position.x && WalkDirection == WalkableDirection.Left)
                {
                    FlipDirection();
                }
                else if (player.position.x < transform.position.x && WalkDirection == WalkableDirection.Right)
                {
                    FlipDirection();
                }
                
                if (Time.time >= nextShootTime)
                {
                    TriggerAttack();
                    nextShootTime = Time.time + shootCooldown;
                }
            }
            else
            {
                if (!damageable.LockVelocity && touchingDirections.IsGrounded)
                {
                    rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
                    animator.SetBool(AnimationStrings.isMoving, true); 
                }
                else if (damageable.LockVelocity)
                {
                    // If velocity is locked (during hit), we wait until it's unlocked
                    animator.SetBool(AnimationStrings.isMoving, false);
                }
                else
                {
                    rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
                }
            }

            if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
            {
                FlipDirection();
            }
        }
    }

    private bool IsPlayerInRange()
    {
        if (player == null) return false;
        
        // Check if player is horizontally in range
        float horizontalDistance = Mathf.Abs(transform.position.x - player.position.x);
        
        // Check if player is roughly at the same height (within a tolerance)
        float verticalDistance = Mathf.Abs(transform.position.y - player.position.y);
        float verticalTolerance = 2f; // Adjust this value based on your game's scale
        
        return horizontalDistance <= detectionRange && verticalDistance <= verticalTolerance;
    }

    private void FlipDirection()
    {
        WalkDirection = (WalkDirection == WalkableDirection.Right) ? WalkableDirection.Left : WalkableDirection.Right;
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
    }

    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, MathF.Max(value, 0)); }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        // Hủy bỏ coroutine trước đó nếu có
        StopAllCoroutines();
        
        rb.velocity = new Vector2(knockback.x * -Mathf.Sign(transform.localScale.x), rb.velocity.y + knockback.y);
        animator.SetBool(AnimationStrings.isMoving, false);
        StartCoroutine(ResumeMovementAfterHit());
    }
    
    private IEnumerator ResumeMovementAfterHit()
    {
        yield return new WaitForSeconds(0.01f);
        
        // Đợi cho đến khi không còn LockVelocity
        float timeout = 1.5f; 
        float timer = 0f;
        
        while (damageable.LockVelocity && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (timer >= timeout || damageable.LockVelocity)
        {
            damageable.LockVelocity = false;
        }
        
        if (damageable.IsHit)
        {
            animator.SetBool(AnimationStrings.isHit, false);
        }
        
        // Sau khi phục hồi, kiểm tra xem người chơi có trong tầm không
        HasTarget = IsPlayerInRange();
        
        // Cài đặt trạng thái moving dựa vào HasTarget
        if (!HasTarget && touchingDirections.IsGrounded)
        {
            animator.SetBool(AnimationStrings.isMoving, true);
            
            // QUAN TRỌNG: Đảm bảo vận tốc được cài đặt ngay lập tức
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
        }

    }

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

    private void TriggerAttack()
    {
        animator.SetTrigger("attack");
        StartCoroutine(PerformShoot());
    }

    private IEnumerator PerformShoot()
    {
        yield return new WaitForSeconds(0.01f);

        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
    
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.velocity = walkDirectionVector * 10f;
                bulletRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
    }
}

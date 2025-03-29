using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFiler;
    public float groundDistance = 0.05f;

    CapsuleCollider2D touchingCol;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded;

    public bool IsGrounded { get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

	private void Awake()
	{
		touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();    
	}

	private void FixedUpdate()
	{
        IsGrounded = touchingCol.Cast(Vector2.down, castFiler, groundHits, groundDistance) > 0;
	}
}
 
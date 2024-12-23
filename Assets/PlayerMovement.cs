using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;

    bool IsFacingRight = true;
    
    // Movement
    public float MovementSpeed = 5f;
    float HorizontalMovement;

    // Jumping
    public float JumpPower = 5f;
    public int MaxJumps = 2;
    private int JumpsRemaining;

    // Ground Check
    public Transform GroundCheckPosition;
    public UnityEngine.Vector2 GroundCheckSize = new UnityEngine.Vector2(0.5f, 0.05f);

    public LayerMask GroundLayer;
    public bool IsGrounded;

    // Wall Check
    public Transform WallCheckPosition;
    public UnityEngine.Vector2 WallCheckSize = new UnityEngine.Vector2(0.5f, 0.05f);
    public LayerMask WallLayer;

    // Gravity
    public float BaseGravity = 2;
    public float MaxFallSpeed = 10f;
    public float FallSpeedMultiplier = 2f;

    // Wall Movement
    public float WallSlideSpeed = 2;
    bool IsWallSliding;
    bool IsWallJumping;
    float WallJumpingDirection;
    float WallJumpTime = 0.5f;
    float WallJumpTimer;
    public UnityEngine.Vector2 WallJumpPower = new UnityEngine.Vector2(5f, 10f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GroundCheck();
        Gravity();
        Flip();
        ProcessWallSlide();
        ProcessWallJump();

        if(!IsWallJumping)
        {
            rb.linearVelocity = new UnityEngine.Vector2(HorizontalMovement * MovementSpeed, rb.linearVelocityY);
            Flip();
        }
    }

    public void Gravity()
    {
        if (rb.linearVelocityY < 0) 
        {
            rb.gravityScale = BaseGravity * FallSpeedMultiplier; 
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -MaxFallSpeed));
        }

        else{
            rb.gravityScale = BaseGravity;
        }
    }
    

    public void Move(InputAction.CallbackContext context) 
    {
        HorizontalMovement = context.ReadValue<UnityEngine.Vector2>().x; 
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (JumpsRemaining > 0)
        {
            if(context.performed) // hold
            {
                rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocityX, JumpPower);
                JumpsRemaining--;
            }

            else if (context.canceled) // tap
            {
                rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
                JumpsRemaining--;
            }
        }

        if(context.performed && WallJumpTimer > 0f)
        {
            IsWallJumping = true;
            rb.linearVelocity = new UnityEngine.Vector2(WallJumpingDirection * WallJumpPower.x, WallJumpPower.y);
            WallJumpTimer = 0;

            if (transform.localScale.x != WallJumpingDirection)
            {
                IsFacingRight = !IsFacingRight;
                UnityEngine.Vector3 ls = transform.localScale;
                ls.x *= -1;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), WallJumpTime + 0.1f);
        }
    }

    private void GroundCheck() 
    {
        if (Physics2D.OverlapBox(GroundCheckPosition.position, GroundCheckSize, 0, GroundLayer))
        {
            JumpsRemaining = MaxJumps;
            IsGrounded = true;
        }
        
        else
        {
            IsGrounded = false;
        }
    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(WallCheckPosition.position, WallCheckSize, 0, WallLayer);
    }

    private void ProcessWallSlide()
    {
        if (!IsGrounded && WallCheck() && HorizontalMovement != 0)
        {
            IsWallSliding = true;
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocityX, Mathf.Max(rb.linearVelocityY, -WallSlideSpeed));
        }

        else 
        {
            IsWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if(IsWallSliding)
        {
            IsWallJumping = false;
            WallJumpingDirection = -transform.localScale.x;
            WallJumpTimer = WallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }

        else if(WallJumpTimer > 0f)
        {
            WallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        IsWallJumping = false;
    }

    private void Flip()
    {
        if(IsFacingRight && HorizontalMovement < 0 || !IsFacingRight && HorizontalMovement > 0)
        {
            IsFacingRight = !IsFacingRight;
            UnityEngine.Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(GroundCheckPosition.position, GroundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WallCheckPosition.position, WallCheckSize);
    }
}

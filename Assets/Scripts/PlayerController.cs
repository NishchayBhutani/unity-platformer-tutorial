using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    Vector2 playerInput;
    Rigidbody2D playerRigidBody;
    bool isFacingRight = true;
    bool isWallSliding = false;
    bool isWallJumping = false;
    bool isDashing = false;
    float wallJumpDirection;
    TrailRenderer trailRenderer;
    Animator animator;
    
    public float jumpCutoff = 0.5f;
    public float moveSpeed = 6f;
    public float jumpSpeed = 9f;
    public float wallSlideSpeed = 2f;
    public float dashSpeed = 15f;
    public Vector2 wallJumpSpeed = new Vector2(5f, 5f);
    public float wallJumpTime = 0.5f;
    public float dashTime = 0.7f;
    public float playerGroundRadius = 0.2f;
    public float playerVisionRadius = 0.2f;
    public GameObject playerGround;
    public GameObject playerVision;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;

    void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.emitting = false;
        animator = GetComponent<Animator>();
    }

    void Update() {
        WallSlide();
        
        if(!isWallJumping && ((isFacingRight && playerInput.x < 0) || (!isFacingRight && playerInput.x > 0))) {
            Flip();
        }
        
        animator.SetFloat("XVelocity", playerRigidBody.velocity.x);
    }

    void FixedUpdate() {
        if(!isWallJumping && !isDashing) {
            playerRigidBody.velocity = new Vector2(playerInput.x * moveSpeed, playerRigidBody.velocity.y);
        }
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    public void OnMove(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context) {
        if(context.performed) {
            if(CheckGrounded()) {
                Debug.Log("player jumped from ground");
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
            }
        } else if(context.canceled) {
            if(CheckGrounded()) {
                Debug.Log("player jumped canceled from ground");
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed * jumpCutoff);
            }
        }
        if(isWallSliding && !isWallJumping) {
            isWallJumping = true;
            wallJumpDirection = isFacingRight ? -1 : 1;
            playerRigidBody.velocity = new Vector2(wallJumpDirection * wallJumpSpeed.x, wallJumpSpeed.y);
            if(wallJumpDirection == 1 && isFacingRight == false || wallJumpDirection == -1 && isFacingRight == true) {
                Flip();
            }
            Invoke(nameof(CancelWallJump), wallJumpTime);
        }
    }

    void CancelWallJump() {
        isWallJumping = false;
    }

    public void OnDash() {
        isDashing = true;
        trailRenderer.emitting = true;
        /*
        Disabling gravity to make sure the player travels equal dash distance irrespective of direction(upward direction would be more affected by gravity and hence the player would travel less distance in the upward direction due to gravity)
        */
        playerRigidBody.gravityScale = 0f; 
        Vector2 normalizedPlayerInput = playerInput.normalized;
        playerRigidBody.velocity = new Vector2(normalizedPlayerInput.x * dashSpeed, normalizedPlayerInput.y * dashSpeed);
        Invoke(nameof(CancelDash), dashTime);
    }

    void CancelDash() {
        isDashing = false;
        trailRenderer.emitting = false;
        playerRigidBody.gravityScale = 3f; // turning on gravity after dash
    }

    void WallSlide() {
        if(CheckWalled() && !CheckGrounded() && playerInput.x != 0f) {
            isWallSliding = true;
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }

    bool CheckGrounded() {
        return Physics2D.OverlapCircle(playerGround.transform.position, playerGroundRadius, groundLayerMask);
    }

    bool CheckWalled() {
        return Physics2D.OverlapCircle(playerVision.transform.position, playerVisionRadius, wallLayerMask);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerGround.transform.position, playerGroundRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerVision.transform.position, playerVisionRadius);
    }
}

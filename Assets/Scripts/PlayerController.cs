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
    float wallJumpTimer;
    float wallJumpDirection;
    
    public float moveSpeed = 6f;
    public float jumpSpeed = 9f;
    public float wallSlideSpeed = 2f;
    public float wallJumpXSpeed = 4f;
    public float wallJumpYSpeed = 2f;
    public float wallJumpTime = 0.5f;
    public float playerGroundRadius = 0.2f;
    public float playerVisionRadius = 0.2f;
    public GameObject playerGround;
    public GameObject playerVision;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;

    void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update() {
        WallSlide();
        WallJump();
        
        if((isFacingRight && playerInput.x < 0) || (!isFacingRight && playerInput.x > 0)) {
            Flip();
        }
    }

    void FixedUpdate() {
        if(!isWallJumping) {
            playerRigidBody.velocity = new Vector2(playerInput.x * moveSpeed, playerRigidBody.velocity.y);
        }
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    void OnMove(InputValue inputValue) {
        playerInput = inputValue.Get<Vector2>();
    }

    void OnJump() {
        if(CheckGrounded()) {
            Debug.Log("player jumped from ground");
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
        }
        
        // wall jump
        if(wallJumpTimer > 0f) {
            isWallJumping = true;
            playerRigidBody.velocity = new Vector2(wallJumpXSpeed * wallJumpDirection, wallJumpYSpeed);
            wallJumpTimer = 0;
            if (isFacingRight != (wallJumpDirection > 0)) {
                Flip();
            }
            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    void WallSlide() {
        if(CheckWalled() && !CheckGrounded() && playerInput.x != 0f) {
            isWallSliding = true;
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Clamp(playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else {
            isWallSliding = false;
        }
    }

    void WallJump() {
        if(isWallSliding) {
            isWallJumping = false;
            wallJumpDirection = isFacingRight ? -1 : 1;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        } else if(wallJumpTimer > 0f) {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump() {
        isWallJumping = false;
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

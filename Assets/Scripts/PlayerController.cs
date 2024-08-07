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
    float currWallJumpTimer;
    
    public float moveSpeed = 6f;
    public float wallSlideSpeed = 6f;
    public float jumpSpeed = 9f;
    public float wallJumpSpeed = 9f;
    public float playerGroundRadius = 0.2f;
    public float playerVisionRadius = 0.2f;
    public float wallJumpCoolDown = 1f;
    public GameObject playerGround;
    public GameObject playerVision;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;

    void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
        currWallJumpTimer = wallJumpCoolDown;
    }

    void Update() {
        if((isFacingRight && playerInput.x < 0) || (!isFacingRight && playerInput.x > 0)) {
            Flip();
        }

        if(currWallJumpTimer <= 0) {
            isWallJumping = false;
            currWallJumpTimer = wallJumpCoolDown;
        } else {
            currWallJumpTimer -= Time.deltaTime;
        }

        WallSlide();
    }

    void WallSlide() {
        if (!CheckGrounded() && CheckWalled() && playerInput.x != 0) {
                isWallSliding = true;
            } else {
                isWallSliding = false;
            }
    }

    void FixedUpdate() {
        if (isWallSliding) {
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, Mathf.Clamp(
                playerRigidBody.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else if(!isWallJumping){
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
        if (CheckGrounded()) {
            Debug.Log("player jumped from ground");
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
        } else if(CheckWalled()) {
            Debug.Log("player walljumped");
            if(isWallJumping) {
                return;
            }
            isWallJumping = true;
            currWallJumpTimer = wallJumpCoolDown;
            float jumpDirection = isFacingRight ? -1 : 1;
            playerRigidBody.velocity = new Vector2(jumpDirection * wallJumpSpeed, jumpSpeed);
            Debug.Log(jumpDirection * wallJumpSpeed);
            Flip();
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

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

    public float moveSpeed = 6f;
    public float jumpSpeed = 9f;
    public float wallJumpSpeed = 9f;
    public float playerGroundRadius = 0.2f;
    public float playerVisionRadius = 0.2f;
    public GameObject playerGround;
    public GameObject playerVision;
    public LayerMask groundLayerMask;
    public LayerMask wallLayerMask;

    void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        playerRigidBody.velocity = new Vector2(playerInput.x * moveSpeed, playerRigidBody.velocity.y);
    }

    void Flip() {
        isFacingRight = !isFacingRight;
        transform.Rotate(0, 180, 0);
    }

    void OnMove(InputValue inputValue) {
        playerInput = inputValue.Get<Vector2>();
        if((isFacingRight && playerInput.x < 0) || (!isFacingRight && playerInput.x > 0)) {
            Flip();
        }
    }

    void OnJump() {
        if (IsGrounded()) {
            Debug.Log("player jumped from ground");
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
        } else if(isWalled()) {
            Debug.Log("player walljumped");
            float jumpDirection = isFacingRight ? -1 : 1;
            playerRigidBody.velocity = new Vector2(jumpDirection * wallJumpSpeed, jumpSpeed);
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(playerGround.transform.position, playerGroundRadius, groundLayerMask);
    }

    bool isWalled() {
        return Physics2D.OverlapCircle(playerVision.transform.position, playerVisionRadius, wallLayerMask);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerGround.transform.position, playerGroundRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerVision.transform.position, playerVisionRadius);
    }
}

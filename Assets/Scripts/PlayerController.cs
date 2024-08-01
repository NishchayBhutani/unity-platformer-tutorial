using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    Vector2 playerInput;
    Vector2 moveDirection;
    Rigidbody2D playerRigidBody;

    public float moveSpeed = 6f;
    public float jumpSpeed = 9f;
    public float playerGroundRadius = 0.2f;
    public GameObject playerGround;
    public LayerMask groundLayerMask;

    void Awake() {
        playerRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (!Mathf.Approximately(playerInput.x, 0f) || !Mathf.Approximately(playerInput.y, 0f)) {
            moveDirection.Set(playerInput.x, playerInput.y);
            moveDirection.Normalize();
        }
    }

    void FixedUpdate() {
        playerRigidBody.velocity = new Vector2(playerInput.x * moveSpeed, playerRigidBody.velocity.y);
    }

    void OnMove(InputValue inputValue) {
        playerInput = inputValue.Get<Vector2>();
    }

    void OnJump() {
        if (IsGrounded()) {
            Debug.Log("player jumped");
            // playerRigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, jumpSpeed);
        }
    }

    bool IsGrounded() {
        return Physics2D.OverlapCircle(playerGround.transform.position, playerGroundRadius, groundLayerMask);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerGround.transform.position, playerGroundRadius);
    }
}

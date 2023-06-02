using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;
    Vector2 vecMove;
    private BoxCollider2D coll;

    [Header("----Animation----")]
    private Animator anim;
    private enum MovementState { idling, running, jumping, falling, sliding }

    [Header("----Jump System----")]
    [SerializeField] float jumpTime;
    [SerializeField] float jumpPower = 12f;
    [SerializeField] float fallMultiplier;
    [SerializeField] float jumpMultiplier;

    public Transform groundCheck;
    public LayerMask groundLayer;
    Vector2 vecGravity;

    bool isJumping = false;
    float jumpCounter;



    private void Start()
    {
        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    public void Jump(InputAction.CallbackContext value)
    {
        Debug.Log(isGrounded());
        if (value.started && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isJumping = true;
            jumpCounter = 0;
        }
        if (value.canceled)
        {
            isJumping = false;
            jumpCounter = 0;

            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.6f);
            }
        }
    }

    public void Movement(InputAction.CallbackContext value)
    {
        vecMove = value.ReadValue<Vector2>();
        Flip();
    }

    private void Update()
    {
        rb.velocity = new Vector2(vecMove.x * speed, rb.velocity.y);

        if (rb.velocity.y < 0)
        {
            rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
        }

        if (rb.velocity.y > 0 && isJumping)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter > jumpTime) isJumping = false;

            float t = jumpCounter / jumpTime;
            float currentJumpM = jumpMultiplier;

            if (t > 0.5f)
            {
                currentJumpM = jumpMultiplier * (1 - t);
            }

            rb.velocity += vecGravity * currentJumpM * Time.deltaTime;
        }

        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        MovementState state;

        //change animation state
        if (vecMove.x > 0f) //running right
        {
            state = MovementState.running;
        }
        else if (vecMove.x < 0f) //running left
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idling;
        }

        if (rb.velocity.y > 0.01f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("movement_state", (int)state);
    }
    private void Flip()
    {
        if (vecMove.x < -0.01f) transform.localScale = new Vector3(-1, 1, 1);
        if (vecMove.x > 0.01f) transform.localScale = new Vector3(1, 1, 1);
    }

    //private bool isGrounded()
    //{
    //    return Physics2D.OverlapCapsule(groundCheck.position, new Vector2(0.9f, 0.1f), CapsuleDirection2D.Horizontal, 0, groundLayer);
    //}
    private bool isGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down; //check on ground so direction down
        float distance = .5f;
        RaycastHit2D hit = Physics2D.BoxCast(position, coll.bounds.size, 0f, direction, distance, groundLayer);
        return hit;
    }
}

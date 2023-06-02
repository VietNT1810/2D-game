using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float dirX;
    private BoxCollider2D coll;
    AudioManager audioManager;

    //wall slide
    private bool facingRight;
    private bool isWallSliding = false;

    //Jump
    [SerializeField] private float xWallForce;
    [SerializeField] private float yWallForce;
    [SerializeField] private float wallJumpTime;

    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private LayerMask jumpablePlatform;
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private Transform frontCheck;
    [SerializeField] private float wallSlidingSpeed = 2f;
    private bool doubleJump;
    private bool isDoubleJumping;

    //wall jump
    private bool isWallJumping = false;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 2f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 10f);

    private Animator anim;
    private enum MovementState { idling, running, jumping, falling, sliding, doubleJump }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * runSpeed, rb.velocity.y);

        if (!PauseController.gameIsPaused)
        {
            Jump();
            WallJump();
        }
        WallSlide();
        UpdateMovementAnimation();
    }
    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded() || isPlatform())
            {
                rb.velocity = Vector2.up * jumpPower;
                audioManager.PlaySFX(audioManager.jump);
                isDoubleJumping = false;
                doubleJump = true;
            }
            else if (doubleJump)
            {
                rb.velocity = Vector2.up * jumpPower * 0.7f;
                audioManager.PlaySFX(audioManager.jump);
                doubleJump = false;
                StartCoroutine(SetIsDoubleJump());
            }
        }
    }
    IEnumerator SetIsDoubleJump()
    {
        isDoubleJumping = true;
        yield return new WaitForSeconds(0.5f);
        isDoubleJumping = false;
    }
    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke("SetWallJumpingToFalse");
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            if (wallJumpingCounter > 0f)
            {
                isWallJumping = true;
                rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
                wallJumpingCounter = 0f;
                audioManager.PlaySFX(audioManager.jump);
                doubleJump = true;
            }
            else if (doubleJump)
            {
                rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x * 0.7f, wallJumpingPower.y * 0.7f);
                audioManager.PlaySFX(audioManager.jump);
                doubleJump = false;
                StartCoroutine(SetIsDoubleJump());
            }

            if (transform.localScale.x != wallJumpingDirection)
            {
                Flip();
            }

            Invoke("SetWallJumpingToFalse", wallJumpingDuration);
        }
    }

    private void SetWallJumpingToFalse()
    {
        isWallJumping = false;
    }

    private void WallSlide()
    {
        if (isTouchingFront() == true && isGrounded() == false && dirX != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
    }

    private void UpdateMovementAnimation()
    {
        MovementState state;

        //change animation state
        if (dirX > 0f) //running right
        {
            state = MovementState.running;
        }
        else if (dirX < 0f) //running left
        {
            state = MovementState.running;
        }
        else
        {
            state = MovementState.idling;
        }

        if (rb.velocity.y > 0.1f && isTouchingFront() == false)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }

        if (isDoubleJumping)
        {
            state = MovementState.doubleJump;
        }

        if (isWallSliding)
        {
            state = MovementState.sliding;
        }

        //change player facing
        if (dirX > 0f && facingRight == true) //running right
        {
            Flip();
        }
        else if (dirX < 0f && facingRight == false) //running left
        {
            Flip();
        }

        anim.SetInteger("movement_state", (int)state);
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }

    private bool isGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down; //check on ground so direction down
        float distance = .5f;
        RaycastHit2D hit = Physics2D.BoxCast(position, coll.bounds.size, 0f, direction, distance, jumpableGround);
        return hit;
    }

    private bool isPlatform()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down; //check on ground so direction down
        float distance = .5f;
        RaycastHit2D hit = Physics2D.BoxCast(position, coll.bounds.size, 0f, direction, distance, jumpablePlatform);
        return hit;
    }

    private bool isTouchingFront()
    {
        Vector2 position = transform.position;
        Vector2 direction = facingRight ? Vector2.left : Vector2.right;
        float distance = 1.0f;

        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, jumpableGround);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }
}
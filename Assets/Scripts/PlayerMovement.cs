using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float dirX;
    private BoxCollider2D coll;
    AudioManager audioManager;

    //wall slide
    private bool facingRight;
    private bool wallSliding = false;

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

    //wall jump
    private bool isWallJumping = false;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 2f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 10f);

    private Animator anim;
    private enum MovementState { idling, running, jumping, falling, sliding }

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * runSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (isGrounded() || isPlatform()))
        {
            // rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            rb.velocity = Vector2.up * jumpPower;
            //jumpSound.Play();
            audioManager.PlaySFX(audioManager.jump);
        }

        if (isTouchingFront() == true && isGrounded() == false && dirX != 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }

        if (wallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        UpdateMovementAnimation();
        WallJump();
    }
    private void WallJump()
    {
        if (wallSliding)
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
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

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
    private void UpdateMovementAnimation()
    {
        MovementState state;

        //change animation state
        if (dirX > 0f) //running right
        {
            state = MovementState.running;
            // sr.flipX = false;
        }
        else if (dirX < 0f) //running left
        {
            state = MovementState.running;
            // sr.flipX = true;
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

        if (wallSliding)
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
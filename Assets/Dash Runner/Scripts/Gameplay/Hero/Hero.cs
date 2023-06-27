using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hero : MonoBehaviour
{
    /// <summary>
    /// Reference to the GameObject that will simulate the jump.
    /// </summary>
    public Rigidbody2D simulatedPhysics;

    /// <summary>
    /// Running speed.
    /// </summary>
    public float runningSpeed = 15;

    /// <summary>
    /// Strength of the force applied to the simulated physics on a slide.
    /// </summary>
    public float slideImpulsionStrength = 5;

    /// <summary>
    /// Length of the dash.
    /// </summary>
    public float dashLength = 20;

    /// <summary>
    /// Speed coefficient applied on the game on dashing.
    /// </summary>
    public float dashStrength = 2;

    /// <summary>
    /// Threshold to apply when checking if the hero is back to his normal place.
    /// </summary>
    [Tooltip("Threshold to apply when checking if the Hero is back to the middle of the screen")]
    public float isInMiddleThreshold = 0.1f;

    /// <summary>
    /// Reference to the prefab for the dead effect.
    /// </summary>
    public GameObject deadEffect;

    /// <summary>
    /// Reference to the AudioSource for the dash.
    /// </summary>
    public AudioSource dashSound;

    /// <summary>
    /// Reference to the AudioSource for the jump.
    /// </summary>
    public AudioSource jumpSound;

    /// <summary>
    /// Reference to the AudioSource for the counter-jump.
    /// </summary>
    public AudioSource counterJumpSound;

    /// <summary>
    /// Reference to its own Transform.
    /// </summary>
    Transform trsf;

    /// <summary>
    /// References to the transform of the actual Hero sprite.
    /// </summary>
    Transform spriteTransform;

    /// <summary>
    /// Reference to the Animator component.
    /// </summary>
    Animator animator;

    /// <summary>
    /// Real speed to apply.
    /// </summary>
    float actualSpeed;

    /// <summary>
    /// Coefficient to apply to the actual speed.
    /// </summary>
    float speedCoefficient;
    public float SpeedCoefficient
    {
        get
        {
            return speedCoefficient;
        }

        set
        {
            speedCoefficient = value;
        }
    }

    /// <summary>
    /// Is the game on?
    /// </summary>
    bool isAlive = false;

    /// <summary>
    /// Are inputs activated?
    /// </summary>
    bool listenToInput = false;

    /// <summary>
    /// Flag determining the direction of the slide.
    /// </summary>
    bool isSlidingDown = false;

    /// <summary>
    /// Flag determining if the Hero is currently dashing.
    /// </summary>
    bool isDashing = false;
    public bool IsDashing
    {
        get
        {
            return isDashing;
        }
    }

    /// <summary>
    /// Flag determining if the Hero is currently jumping.
    /// </summary>
    bool isJumping = false;

    /// <summary>
    /// X coordinate of the dash's start point.
    /// </summary>
    float dashStartPosition;

    /// <summary>
    /// Slide direction input.
    /// </summary>
    Vector2 slideDirection = Vector2.zero;

    /// <summary>
    /// Is the player tapping.
    /// </summary>
    bool isTapping = false;

    /// <summary>
    /// Last time the player jumped.
    /// </summary>
    float jumpTime = 0;

    /// <summary>
    /// Stored velocity for smooth damping.
    /// </summary>
    float velocityY;

    void Awake()
    {
        trsf = transform;
        spriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        actualSpeed = runningSpeed;
        isAlive = false;
    }

    void Update()
    {
        if (isAlive)
        {
            bool isInTheMiddle = IsInTheMiddle();
            bool isCounterSliding = DoesPlayerCounterSlide(slideDirection);
            bool isSliding = Mathf.Abs(slideDirection.y) > Mathf.Abs(slideDirection.x);
            
            // Jump can be done anytime if the Hero is in the middle and not jumping already
            if (isInTheMiddle && !isJumping && isSliding)
            {
                Jump(slideDirection);
            }
            else if (isJumping)
            {
                isJumping = Mathf.Abs(simulatedPhysics.velocity.y) > 0.2f || Time.time - jumpTime < 0.5f;
            }

            // Dash can be done anytime, except during another dash
            if (!isDashing && isTapping)
            {
                Dash();
            }

            // Dash ends when the Hero is dashing and when the dash lenght is reached
            if (isDashing && trsf.position.x - dashStartPosition >= dashLength)
            {
                DashEnd();
            }

            // Counterjumps are only when the Hero is not in the middle OR when the Hero is dashing
            if ((!isInTheMiddle || isDashing) && isSliding && isCounterSliding)
            {
                CounterJump();

                // if it is during a dash, then the dash must stop
                if (isDashing)
                {
                    DashEnd();
                }
            }

            Move();

            // Reset inputs
            slideDirection = Vector2.zero;
            isTapping = false;
        }
    }

    /// <summary>
    /// Starts the game.
    /// </summary>
    /// <param name="listenToInput">Are inputs activated?</param>
    public void StartGame(bool listenToInput = true)
    {
        isAlive = true;
        this.listenToInput = listenToInput;
    }

    /// <summary>
    /// Sets up or remove the pause.
    /// </summary>
    /// <param name="isPause">Wether to sets up pause or not.</param>
    public void Pause(bool isPause)
    {
        isAlive = !isPause;
        if (isAlive)
        {
            simulatedPhysics.WakeUp();
        }
        else
        {
            simulatedPhysics.Sleep();
        }

        listenToInput = !isPause;
    }

    /// <summary>
    /// Registers a slide input.
    /// </summary>
    /// <param name="context">Input action callback context</param>
    public void Slide(InputAction.CallbackContext context)
    {
        if (!isAlive)
        {
            return;
        }
        
        slideDirection = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Registers a swipe input.
    /// </summary>
    /// <param name="swipe">Performed swipe.</param>
    public void Slide(Vector2 swipe)
    {
        if (!isAlive)
        {
            return;
        }

        slideDirection = swipe;
    }

    /// <summary>
    /// Registers a tap input.
    /// </summary>
    public void Tap()
    {
        if (!isAlive)
        {
            return;
        }

        isTapping = true;
    }

    /// <summary>
    /// Forces specific inputs.
    /// </summary>
    /// <param name="slideDirection">Forced slide direction.</param>
    /// <param name="isTapping">Forced is tapping.</param>
    public void ForceInput(Vector2 slideDirection, bool isTapping)
    {
        this.slideDirection = slideDirection;
        this.isTapping = isTapping;
    }

    /// <summary>
    /// Handles the game over behaviour.
    /// </summary>
    public void GameOver(ContactPoint2D contactPoint)
    {
        isAlive = false;

        spriteTransform.gameObject.SetActive(false);

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, contactPoint.normal);
        Instantiate(deadEffect, contactPoint.point, rotation);

        MainGameManager.Instance.PreGameOver();

        StopAllCoroutines();
        StartCoroutine(AfterGameOver());
    }

    /// <summary>
    /// Prepares the hero to continue the game after a game over.
    /// </summary>
    public void PrepareToContinue()
    {
        spriteTransform.gameObject.SetActive(true);

        // Force the physic to the ground.
        simulatedPhysics.velocity = Vector2.zero;
        Vector2 physicsPosition = simulatedPhysics.position;
        physicsPosition.y = 0;
        simulatedPhysics.position = physicsPosition;

        // Force hero to the ground.
        Vector2 heroPosition = trsf.position;
        heroPosition.y = 0;
        trsf.position = heroPosition;
    }

    /// <summary>
    /// Resume the game after a Game Over.
    /// </summary>
    public void Continue()
    {
        isAlive = true;
    }

    /// <summary>
    /// Accessor for isAlive property.
    /// </summary>
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
    }

    /// <summary>
    /// Determines if the Hero is vertically in the middle of the screen.
    /// </summary>
    /// <returns><c>true</c> if his vertical position is between -isInMiddleThreshold and isInMiddleThreshold, <c>false</c> otherwise.</returns>
    bool IsInTheMiddle()
    {
        return Mathf.Abs(trsf.position.y) < isInMiddleThreshold;
    }

    /// <summary>
    /// Determines if the player perform a counter slide.
    /// </summary>
    /// <param name="slideDirection">Direction of the slide.</param>
    /// <returns><c>true</c> if he does, <c>false</c> otherwise.</returns>
    bool DoesPlayerCounterSlide(Vector2 slideDirection)
    {
        return Mathf.Abs(slideDirection.y) > Mathf.Abs(slideDirection.x)
            && (isSlidingDown ? slideDirection.y > 0 : slideDirection.y < 0);
    }

    /// <summary>
    /// Makes a jump in the slideDirection.
    /// </summary>
    /// <param name="slideDirection">Direction of the slide the player did.</param>
    void Jump(Vector2 slideDirection)
    {
        if (isDashing)
        {
            DashEnd();
        }
        isJumping = true;
        jumpTime = Time.time;
        isSlidingDown = slideDirection.y < 0;
        simulatedPhysics.AddForce(trsf.up * slideImpulsionStrength);
        jumpSound.Play();
        animator.SetBool("dashing", false);
    }

    /// <summary>
    /// Counters the jump and go back in the middle.
    /// </summary>
    void CounterJump()
    {
        jumpTime = 0;
        simulatedPhysics.AddForce(-trsf.up * slideImpulsionStrength * 2);
        counterJumpSound.Play();
        animator.SetBool("dashing", false);
    }

    /// <summary>
    /// Dashes.
    /// </summary>
    void Dash()
    {
        isDashing = true;
        dashStartPosition = trsf.position.x;
        simulatedPhysics.Sleep();

        actualSpeed *= dashStrength;

        dashSound.Play();
        animator.SetBool("dashing", true);
    }

    /// <summary>
    /// Ends the dash.
    /// </summary>
    void DashEnd()
    {
        isDashing = false;
        simulatedPhysics.WakeUp();

        actualSpeed = runningSpeed;

        animator.SetBool("dashing", false);
    }

    /// <summary>
    /// Moves the Hero forward.
    /// </summary>
    void Move()
    {
        Vector3 newPosition = trsf.position + trsf.right * actualSpeed * speedCoefficient * Time.deltaTime;
        float newY = simulatedPhysics.position.y * (isSlidingDown ? -1 : 1);
        newPosition.y = Mathf.SmoothDamp(trsf.position.y, newY, ref velocityY, Time.fixedDeltaTime * 2);
        trsf.position = newPosition;

        animator.SetFloat("speed", actualSpeed);
    }

    /// <summary>
    /// What happens when the player lose.
    /// </summary>
    IEnumerator AfterGameOver()
    {
        yield return new WaitForSeconds(1);

        MainGameManager.Instance.GameOver();
    }
}

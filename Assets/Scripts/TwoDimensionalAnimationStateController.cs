using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    Transform characterTransform;
    Rigidbody rb;
    
    float velocityZ = 0.0f;
    float velocityX = 0.0f;

    [Header("Movement")]
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2f;
    public float walkSpeed = 1.0f;
    public float runSpeed = 3.0f;
    public float jumpForce;
    public float jumpCooldown;
    bool readyToJump;

    [Header("Camera rotation")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    private float rotationX = 0;
    private bool canMove = true;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    int VelocityZHash;
    int VelocityXHash;

    private Vector3 movementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        characterTransform = GetComponent<Transform>();
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        readyToJump = true;
    }
    void changeVelocity(bool backPressed,bool forwardPressed, bool leftPressed, bool rightPressed,bool runPressed, float currentMaxVelocity)
    {
        //forward
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        //back
        if (backPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }
        if (!backPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
        //right
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        //left
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }
    }
    void lockOrResetVelocity(bool backPressed, bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // Reset velocityZ 
        if (!forwardPressed && !backPressed)
        {
            if (velocityZ > 0.0f)
            {
                velocityZ -= Time.deltaTime * deceleration;
                if (velocityZ < 0.05f) velocityZ = 0.0f;
            }
            else if (velocityZ < 0.0f)
            {
                velocityZ += Time.deltaTime * deceleration;
                if (velocityZ > -0.05f) velocityZ = 0.0f;
            }
        }
        // Reset velocityX 
        if (!leftPressed && !rightPressed)
        {
            if (velocityX > 0.0f)
            {
                velocityX -= Time.deltaTime * deceleration;
                if (velocityX < 0.05f) velocityX = 0.0f;
            }
            else if (velocityX < 0.0f)
            {
                velocityX += Time.deltaTime * deceleration;
                if (velocityX > -0.05f) velocityX = 0.0f;
            }
        }
        //forward velocity
        if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > (currentMaxVelocity - 0.05f) && velocityZ < currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        //backward velocity
        if (backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        else if (backPressed && velocityZ < (-currentMaxVelocity + 0.05f) && velocityZ > -currentMaxVelocity)
        {
            velocityZ = -currentMaxVelocity;
        }
        //right velocity
        if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && velocityX > (currentMaxVelocity - 0.05f) && velocityX < currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        //left velocity
        if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && velocityX < (-currentMaxVelocity + 0.05f) && velocityX > -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
    }
    private void Jump(bool forwardPressed, bool backPressed, bool leftPressed, bool rightPressed, bool runPressed)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity
        float appliedJumpForce = jumpForce;
        appliedJumpForce = jumpForce;
        rb.AddForce(Vector3.up * appliedJumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool backPressed = Input.GetKey(KeyCode.S);
        bool spacePressed = Input.GetKeyDown(KeyCode.Space);

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        float currentSpeed = runPressed ? runSpeed : walkSpeed;

        // Update movement velocities
        changeVelocity(backPressed, forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        lockOrResetVelocity(backPressed, forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        // Ground check
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        float rayLength = playerHeight * 0.5f + 0.2f;
        grounded = Physics.Raycast(rayStart, Vector3.down, rayLength, whatIsGround);
        Debug.DrawRay(rayStart, Vector3.down * rayLength, grounded ? Color.green : Color.red, 0.1f);
        // Camera and character rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        Vector3 moveDirection = new Vector3(velocityX, 0, velocityZ).normalized;
        Vector3 targetVelocity = moveDirection * currentSpeed;

        //natural falling
        if (!grounded)
        {
            targetVelocity *= 0.8f; 
            targetVelocity = transform.TransformDirection(targetVelocity); 
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z); 
        }
        else
        {
            targetVelocity = transform.TransformDirection(targetVelocity); 
            rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z); 
        }
        // Jump logic
        if (spacePressed && readyToJump && grounded)
        {
            readyToJump = false;
            Jump(forwardPressed, backPressed, leftPressed, rightPressed, runPressed);
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        // Update animations
        if (animator != null)
        {
            animator.SetFloat(VelocityZHash, velocityZ);
            animator.SetFloat(VelocityXHash, velocityX);
        }
        transform.Translate(movementDirection * Time.deltaTime * currentSpeed);
    }   
}

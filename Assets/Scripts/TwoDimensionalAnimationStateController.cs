using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    Transform characterTransform;
    
    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2f;
    public float walkSpeed = 1.0f;
    public float runSpeed = 3.0f;

    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;

    int VelocityZHash;
    int VelocityXHash;

    private Vector3 movementDirection;

    
    void Start()
    {
        
        characterTransform = GetComponent<Transform>();
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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


    // Update is called once per frame
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool backPressed = Input.GetKey(KeyCode.S);

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        float currentSpeed = runPressed ? runSpeed : walkSpeed;

        changeVelocity(backPressed, forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        lockOrResetVelocity(backPressed, forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        movementDirection = new Vector3(velocityX, 0, velocityZ).normalized;

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        transform.Translate(movementDirection * Time.deltaTime * currentSpeed);
        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
    }
}

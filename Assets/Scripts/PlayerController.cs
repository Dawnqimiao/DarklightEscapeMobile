//Jump function tutorial: https://www.youtube.com/watch?v=XhwRYNie-aI
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public LayerMask ceilingLayer;

    private Rigidbody2D rb;
    private Vector2 movement;
    [SerializeField] private int jumpPower = 5;
    private bool isGrounded;
    private bool isCeiling;
    private bool isInAntiGravity = false;
    private bool isSmall = false; 
    private bool isFalling = false;
    [SerializeField] private float groundCheckRadius = 0.8f;

    private float mobileHorizontalInput = 0f;
    public RectTransform buttonRect;

    public void OnPointerDown(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            buttonRect, pointerData.position, pointerData.pressEventCamera, out localPoint);

        if (localPoint.x < 0)
        {
            mobileHorizontalInput = -1f; // Move to left
        }
        else
        {
            mobileHorizontalInput = 1f; // Move to right
        }
    }

    public void OnPointerUp(BaseEventData data)
    {
        mobileHorizontalInput = 0f; 
    }

    public void Jump()
    {
        if (!isInAntiGravity && isGrounded) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
        else if (isInAntiGravity && isCeiling && isSmall) 
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -jumpPower);
        }
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {

        movement = new Vector2(mobileHorizontalInput, 0);

        Debug.Log("isCeiling: " + isCeiling + ", isSmall: " + isSmall);

        isFalling = rb.linearVelocity.y < 0;

        if (!isInAntiGravity)
        {
            //This method causes the player jump twice when the player closes to obstacles
            //isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            //Use ray to detect ground vertically under the player
            isGrounded = Physics2D.Raycast(transform.position + Vector3.down * 0.4f, Vector2.down, 0.1f, groundLayer);
            isCeiling = false;
        }
        else
        {
            isGrounded = false;
            isCeiling = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, ceilingLayer);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * speed, rb.linearVelocity.y);
    }

    // Enter anti-gravity zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AntiGravityZone"))
        {
            if (isSmall) 
            {
                isInAntiGravity = true;
                rb.gravityScale = -1f;
                FlipGroundCheck();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 5f); // be pulled up
                Debug.Log("The player has shrinked. Enter anti-gravity zone.");
            }
            else
            {
                Debug.Log("The player is too big to enter anti-gravity zone.");
            }
        }
    }



    // Leave anti-gravity zone
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("AntiGravityZone") && isSmall)
        {
            isInAntiGravity = false;
            rb.gravityScale = 1f;
            FlipGroundCheck();
        }
    }

    // flip groundCheck's position
    private void FlipGroundCheck()
    {
        if (groundCheck == null)
        {
            Debug.LogError("groundCheck is None！");
            return;
        }

        float newY = -groundCheck.localPosition.y; 
        groundCheck.localPosition = new Vector3(groundCheck.localPosition.x, newY, groundCheck.localPosition.z);
        Debug.Log("groundCheck flips，current Y is: " + groundCheck.localPosition.y);
    }

    public void SetSmallState(bool small)
    {
        isSmall = small;
    }

    public bool IsSmall()
    {
        return isSmall;
    }
    public void ResetGravity()
    {
        isInAntiGravity = false;
        rb.gravityScale = 1f; // Restore gravity 
        FlipGroundCheck(); // Restore groundCheck 
        Debug.Log("ResetGravity() is using. Gravity is normal！");
    }

}

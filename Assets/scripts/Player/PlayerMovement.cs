using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 5f;
 
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 moveDirection;
    Animator anim;
    private float mobileX;
    private float mobileY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    float x;
    float y;
    public bool isMobileControls;

    private void Update()
    {

        float keyX = Input.GetAxisRaw("Horizontal");
        float keyY = Input.GetAxisRaw("Vertical");


        x = (keyX != 0 || keyY != 0) ? keyX : mobileX;
        y = (keyX != 0 || keyY != 0) ? keyY : mobileY;

        if (x != 0) y = 0;
        moveInput = new Vector2(x, y);

        anim.SetFloat("vertical", y);
        anim.SetFloat("horizontal", x);
        moveDirection = moveInput.normalized;
    }

    private void FixedUpdate()
    {
      
        rb.linearVelocity = moveDirection * moveSpeed;
    }
    public void moveX(float horizontal)
    {
        x = horizontal;
        mobileX = horizontal;
    }

    public void moveY(float vertical)
    {
        y = vertical;
        mobileY = vertical; 
    }
}

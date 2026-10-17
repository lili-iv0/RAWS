using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownControls : MonoBehaviour
{

    //Declare Rigidbody 2D
    Rigidbody2D rb;

    Animator anim;

    //Move Speed Multiplier
    public float moveSpeed = 5f;

    void Start()
    {
        //Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    void Update()
    {

        //Store Vertical & Horizontal Axis Input
        float vAxis = Input.GetAxisRaw("Vertical");
        float hAxis = Input.GetAxisRaw("Horizontal");

        anim.SetFloat("vertical", vAxis);
        anim.SetFloat("horizontal", hAxis);

        if (vAxis != 0) hAxis = 0;

        //Set Velocity of Rigidbody
        rb.linearVelocity = new Vector2(hAxis * moveSpeed, vAxis * moveSpeed);

    }
}

using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
       //Declare Rigidbody 2D
    Rigidbody2D rb;

    //Move Speed Multiplier
    public float moveSpeed = 10f;

    void Start()
    {
        //Get Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }


    void Update()
    {

        //Store Vertical & Horizontal Axis Input
        float vAxis = Input.GetAxis("Vertical");
        float hAxis = Input.GetAxis("Horizontal"); 

        //Set Velocity of Rigidbody
        rb.linearVelocity = new Vector2(hAxis * moveSpeed, vAxis * moveSpeed);
        
    }
}

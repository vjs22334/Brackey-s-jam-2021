using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float maxJumpHeight = 4f;
    public float TimeToApex = 0.5f;

    float gravity;
    float jumpVelocity;
    
    Vector2 velocity;
    Vector2 input;
    CharacterController2D controller2D;


    // Start is called before the first frame update
    void Start()
    {
        controller2D = GetComponent<CharacterController2D>();

        gravity = -(2*maxJumpHeight)/(TimeToApex*TimeToApex);

        jumpVelocity = gravity*-1*TimeToApex;

        Debug.Log("Gravity:"+gravity);
        Debug.Log("JumpVelocity:"+jumpVelocity);
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if(controller2D.collisionInfos.below || controller2D.collisionInfos.above){
            velocity.y = 0;
        }   

        velocity.x = input.x*MoveSpeed;

        if(Input.GetKeyDown(KeyCode.Space) && controller2D.collisionInfos.below){
            velocity.y = jumpVelocity;
        }

        velocity.y += gravity*Time.deltaTime;

        controller2D.Move(velocity*Time.deltaTime);

    }
}

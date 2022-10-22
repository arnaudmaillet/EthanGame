using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    [Header("Speed")]
    public float speed = 4f;
    public float sprintMultiplier = 1.5f;

    [Header("Gravity")]
    public float gravity = -8f;
    public float jumpHeight = 3f;

    [Header("GroundHit")]
    public Transform groundHit;
    public float groundDistance = 0.7f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        // Creer une sphere en dessous du joueur (controle de la valeur velocity.y)
        isGrounded = Physics.CheckSphere(groundHit.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            controller.Move(move * speed * sprintMultiplier * Time.deltaTime);
        }
        else
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

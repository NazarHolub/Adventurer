using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int lives = 5;
    [SerializeField] private float jumpForce;
    [SerializeField] private const float groundCheckRadius = 0.05f;
    private bool isGrounded = true;


    private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;


    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        //m_GroundCheck = transform.GetChild(0).gameObject.transform;
    }

    private void Update()
    {
        IsGrounded();

        if (Input.GetButton("Horizontal"))
            Run();

        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();
    }

    private void FixedUpdate()
    {
        IsGrounded();
    }

    private void Run()
    {
        Vector3 dir = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);

        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        //rigidbody.AddForce(transform.up * jumpForce , ForceMode2D.Impulse);
        rigidbody.AddForce(new Vector2(0f, jumpForce * 10));
    }

    private void IsGrounded()
    {

        Collider2D[] col = Physics2D.OverlapCircleAll(m_GroundCheck.position, groundCheckRadius, ground);

        isGrounded = col.Length > 0;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_GroundCheck.position, groundCheckRadius);
    }
}

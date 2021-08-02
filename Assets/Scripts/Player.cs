using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_GroundCheck;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float hp = 5;
    [SerializeField] private float timeBtwAttack = 0.1f;
    [SerializeField] private float jumpForce;
    [SerializeField] private const float groundCheckRadius = 0.05f;

    private bool facingRight = true;
    private bool isGrounded = true;
    private float moveInput;

    private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;
    private Animator anim;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        //m_GroundCheck = transform.GetChild(0).gameObject.transform;
    }

    private void Update()
    {
        IsGrounded();

        Run();

        if (isGrounded && Input.GetButtonDown("Jump"))
            Jump();


        if (isGrounded)
            anim.SetBool("IsJumping", false);
        else
            anim.SetBool("IsJumping", true);
    }

    private void FixedUpdate()
    {
        IsGrounded();
    }

    private void Run()
    {
        moveInput = Input.GetAxis("Horizontal");
        rigidbody.velocity =new Vector2(moveInput * speed, rigidbody.velocity.y);

        if(facingRight == false && moveInput > 0)
            Flip();
        else if(facingRight == true && moveInput < 0)
            Flip();

        if(moveInput != 0)
            anim.SetBool("IsRunning", true);
        else
            anim.SetBool("IsRunning", false);

    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
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

    public void TakeDamage(int damage)
    {
        hp -= damage;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(m_GroundCheck.position, groundCheckRadius);
    }

}

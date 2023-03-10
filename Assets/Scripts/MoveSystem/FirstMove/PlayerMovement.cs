using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动速度")] 
    private float moveSpeed;

    public float walkSpeed;
    public float sprintSpeed;

    public Transform orientation;
    [Header("跳跃相关设置")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;
    [Header("蹲伏设置")] 
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    [Header("按键绑定")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    [Header("地面检测")] 
    public float groundDrag;
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody m_rigidbody;
    [Header("移动状态")] 
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        air
    }
    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;
    }

    private void Start()
    {
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MyInput();
        SpeedControl();
        StateHandler();
        CalculateDrag();
    }

    private void CalculateDrag()
    {
        if (grounded)
            m_rigidbody.drag = groundDrag;
        else
        {
            m_rigidbody.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    /// <summary>
    /// 输入监听
    /// </summary>
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump),jumpCooldown);
        }
        //下蹲检测
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
        }
    }
    /// <summary>
    /// 人物移动
    /// </summary>
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(grounded)
            m_rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f,ForceMode.Force);
        else if(!grounded)
            m_rigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier,ForceMode.Force);
    }
    /// <summary>
    /// 速度限制
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(m_rigidbody.velocity.x, 0f, m_rigidbody.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            m_rigidbody.velocity = new Vector3(limitedVel.x, m_rigidbody.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
        m_rigidbody.AddForce(transform.up * jumpForce,ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}

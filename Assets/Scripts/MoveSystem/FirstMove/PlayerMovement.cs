using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : SingletonMono<PlayerMovement>
{
    [HideInInspector]
    public float moveSpeed;
    [Header("移动速度")]
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
    public Vector3 boxRadius;
    public Vector3 offsetRadius;
    public LayerMask whatIsGround;
    private bool isGrounded;
    [Header("上坡最大角度")] 
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope; //是否退出斜坡
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
        crouching,
        air
    }

    protected override void Awake()
    {
        base.Awake();
        m_rigidbody = GetComponent<Rigidbody>();
        m_rigidbody.freezeRotation = true;
    }

    private void Start()
    {
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        CheckGrounded();
        MyInput();
        SpeedControl();
        StateHandler();
        CalculateDrag();
    }
    /// <summary>
    /// 检测是否在地面上
    /// </summary>
    private void CheckGrounded()
    {
        Collider[] hitArr = Physics.OverlapBox(transform.position+offsetRadius,boxRadius,
            Quaternion.identity,whatIsGround);
        if (hitArr.Length > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        // isGrounded = Physics.Raycast(transform.position, Vector3.down,
        //     playerHeight * 0.5f + 0.2f, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube(transform.position+offsetRadius,boxRadius*2f);
        Gizmos.DrawLine(transform.position,transform.position+Vector3.down);
    }

    private void CalculateDrag()
    {
        if (isGrounded)
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
        horizontalInput = InputController.Instance.PlayerMoveInput().x;
        verticalInput = InputController.Instance.PlayerMoveInput().y;
      
        if (InputController.Instance.canJump && readyToJump && isGrounded)
        {
            Debug.Log("jump sssss");
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump),jumpCooldown);
        }
        //下蹲检测
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            m_rigidbody.AddForce(Vector3.down * 5f ,ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }
    /// <summary>
    /// 状态处理
    /// </summary>
    private void StateHandler()
    {
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (isGrounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }else if (isGrounded)
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
        //Debug.Log(transform.position+"trans");
        moveDirection = (orientation.forward * verticalInput + orientation.right * horizontalInput).normalized;
    
        //如果在斜坡上
        if (OnSlope() && !exitingSlope)
        {
            m_rigidbody.AddForce(GetSlopeMoveDirection()*moveSpeed*15f,ForceMode.Force);
            if (m_rigidbody.velocity.y > 0)
            {
                m_rigidbody.AddForce(Vector3.down*50f,ForceMode.Force);
            }
        }
        if(isGrounded)
            m_rigidbody.AddForce(moveDirection * moveSpeed * 10f,ForceMode.Force);
        else if(!isGrounded)
            m_rigidbody.AddForce(moveDirection* moveSpeed * 10f * airMultiplier,ForceMode.Force);
        //在斜坡上就关闭重力
        m_rigidbody.useGravity = !OnSlope();
    }
    /// <summary>
    /// 速度限制
    /// </summary>
    private void SpeedControl()
    {
        if (OnSlope()&&!exitingSlope)
        {
            if (m_rigidbody.velocity.magnitude > moveSpeed)
                m_rigidbody.velocity = m_rigidbody.velocity.normalized * moveSpeed;
        }
        else
        {
            Vector3 flatVel = new Vector3(m_rigidbody.velocity.x, 0f, m_rigidbody.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                m_rigidbody.velocity = new Vector3(limitedVel.x, m_rigidbody.velocity.y, limitedVel.z);
            }
        }
        
    }

    private void Jump()
    {
        exitingSlope = true;
        //m_rigidbody.velocity = new Vector3(m_rigidbody.velocity.x, 0, m_rigidbody.velocity.z);
        m_rigidbody.AddForce(transform.up * jumpForce,ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }
    /// <summary>
    /// 判断是否在斜坡上
    /// </summary>
    /// <returns></returns>
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 
            out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }
    /// <summary>
    /// 得到斜坡上的移动方向
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection,slopeHit.normal).normalized;
    }
}

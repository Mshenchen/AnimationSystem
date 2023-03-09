using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HutaoController : MonoBehaviour
{
    private Animator animator;
    //玩家输入监听
    private Vector2 m_Move;
    private Vector3 playerMovement;
    private bool isRunning;
    private Transform playerTransform;

    private Vector3 move = Vector3.zero;
    //旋转速度
    public float rotateSpeed = 1000f;
    private float currentSpeed;
    private float targetSpeed;
    //移动速度
    [SerializeField] private float walkSpeed =1.28f;
    [SerializeField] private float runSpeed = 3.3f;
    private bool armedRifle;
    private float currentFatigue;
    private float minFatigue = 0f;
    private float maxFatigue = 10f;
    //动画层级
    private int fatigueLayerIndex;
    private Camera m_camera;
    //刚体
    private Rigidbody m_rigidbody;
    private void Awake()
    {
        m_camera = Camera.main;
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = transform;
        fatigueLayerIndex = animator.GetLayerIndex("Fatigue");
    }

    private void FixedUpdate()
    {
        
        m_rigidbody.velocity =  move * targetSpeed;
    }

    private void Update()
    {
        RatatePlayer();
        MovePlayer();
        CalculateFatigue();
    }
    /// <summary>
    /// 玩家移动监听
    /// </summary>
    /// <param name="ctx"></param>
    public void GetPlayerMoveInput(InputAction.CallbackContext ctx)
    {
        m_Move = ctx.ReadValue<Vector2>();

    }
    /// <summary>
    /// 玩家奔跑监听
    /// </summary>
    /// <param name="ctx"></param>
    public void GetPlayerRunInput(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.ReadValue<float>() > 0 ? true : false;

    }
    /// <summary>
    /// 玩家旋转控制
    /// </summary>
    void RatatePlayer()
    {
        if(m_Move.Equals(Vector2.zero))
            return;
        playerMovement.x = m_Move.x;
        playerMovement.z = m_Move.y;
        Vector3 camForwardPos = new Vector3(m_camera.transform.forward.x, 0, m_camera.transform.forward.z).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(camForwardPos, Vector3.up);
        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation,targetRotation,rotateSpeed*Time.deltaTime);
    }
    /// <summary>
    /// 玩家移动控制
    /// </summary>
    void MovePlayer()
    {
        targetSpeed = isRunning ? runSpeed : walkSpeed;
        targetSpeed *= m_Move.magnitude;
        Vector3 camForwardPos = new Vector3(m_camera.transform.forward.x, 0, m_camera.transform.forward.z).normalized;
        //playerMovement = camForwardPos * m_Move.y + m_camera.transform.right * m_Move.x;
        //m_camera.transform
       
        //currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.5f);
        //float scaledMoveSpeed = targetSpeed * Time.deltaTime;
        move = new Vector3(m_Move.x, 0, m_Move.y).normalized;
        //move =  rot * Vector3.forward*m_Move.x  + rot*Vector3.forward*m_Move.y;
        Debug.Log(move+"move");
        //playerMovement = playerTransform.InverseTransformVector(playerMovement);
        //transform.position += move * scaledMoveSpeed;
        animator.SetFloat("Speed",targetSpeed);
        
    }
    /// <summary>
    /// 玩家举枪监听
    /// </summary>
    /// <param name="ctx"></param>
    public void GetArmedRifleInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>()==0)
        {
            armedRifle = !armedRifle;
            animator.SetBool("Rifle",armedRifle);
        }
    }
    /// <summary>
    /// 疲劳反馈计算
    /// </summary>
    void CalculateFatigue()
    {
        if (currentSpeed < 1f && currentFatigue >= minFatigue)
        {
            currentFatigue -= Time.deltaTime;
        }else if (currentSpeed > 2f && currentFatigue <= maxFatigue)
        {
            currentFatigue += Time.deltaTime;
        }else return;

        currentFatigue = Mathf.Clamp(currentFatigue, minFatigue, maxFatigue);
        animator.SetLayerWeight(fatigueLayerIndex,currentFatigue/maxFatigue);
    }
}

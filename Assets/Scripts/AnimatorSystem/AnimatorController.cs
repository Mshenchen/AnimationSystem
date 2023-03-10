using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    private Animator m_animator;
    private float currentSpeed;
    private float targetSpeed;
    private Vector2 m_moveInput;
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        m_moveInput = InputController.Instance.PlayerMoveInput().normalized;
        currentSpeed = PlayerMovement.Instance.moveSpeed;
        currentSpeed *= m_moveInput.magnitude;
        m_animator.SetFloat("Speed",currentSpeed);
    }
}

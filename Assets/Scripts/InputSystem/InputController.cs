using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputController : SingletonMono<InputController>
{
    private Vector2 m_MoveInput;
    public bool canJump = false;
    
    /// <summary>
    /// 得到移动控制
    /// </summary>
    /// <param name="ctx"></param>
    public void GetPlayerMoveInput(InputAction.CallbackContext ctx)
    {
        m_MoveInput = ctx.ReadValue<Vector2>();
    }

    /// <summary>
    /// 得到跳跃控制
    /// </summary>
    /// <returns></returns>
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            StartCoroutine(PlayerJumpInput());
        }
    }
    public Vector2 PlayerMoveInput()
    {
        return m_MoveInput;
    }

    private IEnumerator PlayerJumpInput()
    {
        canJump = true;
        yield return new WaitForSeconds(0.1f);
        canJump = false;
    }
}

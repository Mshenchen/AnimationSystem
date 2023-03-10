using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
      public void LoadScene(string name, UnityAction fun)
      {
            SceneManager.LoadScene(name);
            fun();
      }
      
      public void LoadSceneAsyn(string name, UnityAction fun)
      {
            MonoMgr.Instance.StartCoroutine(ReallyLoadSceneAsyn(name, fun));
      }
      /// <summary>
      /// 协程异步加载场景
      /// </summary>
      /// <param name="name"></param>
      /// <param name="fun"></param>
      /// <returns></returns>
      private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
      {
            AsyncOperation ao = SceneManager.LoadSceneAsync(name);
            while (!ao.isDone)
            {
                  //EventCenter.GetInstance().EventTrigger("sdad",ao.progress);
                  yield return ao.progress;
            }
            fun();
      }
}

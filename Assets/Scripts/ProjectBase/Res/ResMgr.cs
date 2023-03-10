using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else
            return res;
    }
    /// <summary>
    /// 异步加载接口
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    public void LoadAsync<T>(string name,UnityAction<T> callback) where T : Object
    {
        MonoMgr.Instance.StartCoroutine(ReallyLoadAsync(name,callback));
    }

    private IEnumerator ReallyLoadAsync<T>(string name,UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;
        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);

    }
}

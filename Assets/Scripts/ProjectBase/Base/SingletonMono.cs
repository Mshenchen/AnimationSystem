using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            return instance;
        }
        
    }

    protected virtual void Awake()
    {
        if(instance!=null)
            Destroy(gameObject);
        else
            instance = this as T;
    }

    protected void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}

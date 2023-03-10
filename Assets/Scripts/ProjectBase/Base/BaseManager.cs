using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Base<T> where T : Base<T>
{
    T Value { get; set; }  
}
public class BaseManager<T> where T:new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = new T();
            return instance;
        }
        
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono : SingletonAutoMono<TestMono>
{
    public void Test()
    {
        Debug.Log(TestMono.Instance.name);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

internal static class ExampleUtil
{
    public static bool isPresent()
    {
        var xrDisplaySubsystems = new List<XRDisplaySubsystem>();
        SubsystemManager.GetSubsystems<XRDisplaySubsystem>(xrDisplaySubsystems);

        foreach (var xrDisplay in xrDisplaySubsystems)
        {
            if (xrDisplay.running)
            {
                return true;
            }
        }
        return false;
    }
}

public class DeviceType : MonoBehaviour
{
    //private string m_DeviceType;

    //private void Awake()
    //{
    //    if (ExampleUtil.isPresent())
    //    {
    //        m_DeviceType = "VR";
    //    }
    //    else
    //    {
    //        m_DeviceType = "PC";
    //    }

    //    if (XRSettings.isDeviceActive) // 케이블이 연결되어 있으면 true 아니면 false
    //    {
    //        Debug.Log("2VR");
    //    }
    //    else
    //    {
    //        Debug.Log("2PC");
    //    }
    //}

    //private void Update()
    //{
    //    Debug.Log(m_DeviceType);
    //}
}
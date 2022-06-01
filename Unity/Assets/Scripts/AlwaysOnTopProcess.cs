using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AlwaysOnTopProcess
{
    private Process process;

    public AlwaysOnTopProcess()
    {
        process = new Process();
        process.StartInfo.FileName = "C:/project/Health_E_moji/Unity/always-on-top.exe";
    }

    public void subProcessStart()
    {
        process.Start();
        UnityEngine.Debug.Log("Start");
    }

    public void subProcessClose()
    {
        process.Kill();
    }
}

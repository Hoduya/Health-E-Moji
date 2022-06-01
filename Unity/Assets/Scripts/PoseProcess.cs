using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Text;


public class PoseProcess
{
    private Process pythonProc;

    private bool isValidData;
    //private bool isRunning;
    private string[] poseData;

    public delegate void PoseDataUpdateEventHandler();
    public event PoseDataUpdateEventHandler updatePoseData;

    public PoseProcess(int mode)
    {
        //isRunning = false;
        isValidData = false;
        pythonProc = new Process();

        if (mode == 0)
        {
            pythonProc.StartInfo.Arguments = "-u c:/project/Healthy_3D_Emoji/Python/Setting.py";
        }
        else
        {
            pythonProc.StartInfo.Arguments = "-u c:/project/Healthy_3D_Emoji/Python/Main.py";
        }
        pythonProc.StartInfo.FileName = "C:/Users/aass7/AppData/Local/Programs/Python/Python38/python.exe";
        pythonProc.StartInfo.UseShellExecute = false;
        pythonProc.StartInfo.CreateNoWindow = true;
        pythonProc.StartInfo.RedirectStandardOutput = true;
        pythonProc.EnableRaisingEvents = true;

        pythonProc.OutputDataReceived += process_OutputDataReceived;
    }


    private void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {   
        if (e.Data.GetType() != typeof(string))
        {
            return;
        }

        poseData = e.Data.Split(" ");


        if (poseData[0].Equals("FromPython"))
        {
            isValidData = true;
            updatePoseData();
        }
        else
        {
            isValidData = false;
        }
    }

    public string[] getPoseData()
    {
        // Return the data only if the pose data updated and valid state.
        if (isValidData == false)
        {
            return null;
        }

        return poseData;
    }

    public bool isValidState()
    {
        return isValidData;
    }

    public void subProcessStart()
    {
        pythonProc.Start();
        pythonProc.BeginOutputReadLine();

       // isRunning = true;
    }

    public void subProcessClose()
    {
        pythonProc.Close();

        //isRunning = false;
    }
}



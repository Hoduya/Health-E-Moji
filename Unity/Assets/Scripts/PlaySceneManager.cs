using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlaySceneManager : MonoBehaviour
{
    public static bool isProcessRunning = false;

    private PoseProcess pythonProcess;
    private AlwaysOnTopProcess onTopProcess;

    private PoseMapping poseMapping;

    public GameObject smileHead;
    public GameObject sadHead;
    public PoseAlert PoseAlertSystem;

    public Button exitButton;
    public TMPro.TMP_Text onTopText;
  
    private void Awake()
    {
        onTopProcess = new AlwaysOnTopProcess();
        pythonProcess = new PoseProcess(1);

        onTopProcess.subProcessStart();
        pythonProcess.subProcessStart();

        pythonProcess.updatePoseData += this.updatePoseData;
        Screen.SetResolution(480, 320, false);
    }

    void Start()
    {
        poseMapping = new PoseMapping(smileHead, sadHead);
        PoseAlertSystem.changeHead += poseMapping.changeHead;

        exitButton.onClick.AddListener(exit);
    }

    // called once per scene frame
    void Update()
    {
        poseMapping.updatePose();
    }

    private void OnDestroy()
    {
        pythonProcess.subProcessClose();
        onTopProcess.subProcessClose();
    }

    // called once per image frame
    void updatePoseData()
    {
        string[] poseData = pythonProcess.getPoseData();

        if (poseData != null)
        {
            isProcessRunning = true;
            PoseAlertSystem.setPoseData(poseData);
            poseMapping.setPoseData(poseData);
        }
    }

    public void PointerHover()
    {
        StartCoroutine(exitButtonONOFF());
    }

    public void exit()
    {
        SceneManager.LoadScene("MenuScene");
    }

    IEnumerator exitButtonONOFF()
    {
        exitButton.gameObject.SetActive(true);
        onTopText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        exitButton.gameObject.SetActive(false);
        onTopText.gameObject.SetActive(false);
    }
}




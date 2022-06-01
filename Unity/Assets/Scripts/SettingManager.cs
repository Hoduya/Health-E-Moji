using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class SettingManager : MonoBehaviour
{
    public const int MeasureFrame = 30;

    public CanvasGroup GuidePanel1;
    public CanvasGroup GuidePanel2;
    public CanvasGroup SettingPanel;

    public Button NextButton;
    public Button XButton;
    public Button WebCamStartButton;
    public Button SetButton;
    public Button GoBackButton;

    public TMP_Text SetUpText;

    private PoseProcess SettingProcess;

    private string[] poseData;

    public ActiveManager activeManager;

    public void Start()
    {
        XButton.onClick.AddListener(BackToMenu);
        GoBackButton.onClick.AddListener(BackToMenu);
        NextButton.onClick.AddListener(NextButtonClick);
        WebCamStartButton.onClick.AddListener(WebCamStartButtonClick);
        SetButton.onClick.AddListener(setButtonClick);

        inActive();
        GuidePanel1.gameObject.SetActive(true);

        SettingProcess = new PoseProcess(0);
        SettingProcess.updatePoseData += UpdatePoseData;
    }
    public void OnEnable()
    {
        Screen.SetResolution(480, 640, false);

        inActive();
        GuidePanel1.gameObject.SetActive(true);
    }

    public void Update()
    {
        SetButton.interactable = SettingProcess.isValidState();
    }

    public void inActive()
    {
        GuidePanel1.gameObject.SetActive(false);
        GuidePanel2.gameObject.SetActive(false);
        SettingPanel.gameObject.SetActive(false);
    }

    public void BackToMenu()
    {
        activeManager.SetActiveCanvas("Menu");
    }

    public void NextButtonClick()
    {
        inActive();
        GuidePanel2.gameObject.SetActive(true);
        StartCoroutine(DoFadeIn(GuidePanel2));
    }

    public void WebCamStartButtonClick()
    {
        inActive();
        SettingProcess.subProcessStart();
        SettingPanel.gameObject.SetActive(true);
        GoBackButton.gameObject.SetActive(false);
        StartCoroutine(DoFadeIn(SettingPanel));
    }

    public void setButtonClick()
    {
        
        SetButton.gameObject.SetActive(false);
        StartCoroutine(Mesure());
    }

    public void setFinish(float earSum, float csDistSum)
    {
        GoBackButton.gameObject.SetActive(true);

        Debug.Log("next");
        SettingProcess.subProcessClose();

        float normalEAR  = earSum / MeasureFrame;
        float normalCSDist = csDistSum / MeasureFrame;

        normalEAR = Mathf.Round(normalEAR * 1000) / 1000;
        normalCSDist = Mathf.Round(normalCSDist);

        SetUpText.fontSize = 25;
        SetUpText.fontStyle = FontStyles.Bold;
        SetUpText.text = "Set up complete!";

        SetUpText.text += "\nEAR Value  : " + normalEAR;
        SetUpText.text += "\nCSDistance : " + normalCSDist;

        UserInfo.instance.NormalEAR = normalEAR;
        UserInfo.instance.NormalCSDist = normalCSDist;
    }

    void UpdatePoseData()
    {
        poseData = SettingProcess.getPoseData();
    }

    IEnumerator Mesure()
    {
        int count = 0;
        float earSum = 0, csDistSum = 0;
        string dots = "";

        SetUpText.fontSize = 30;
        SetUpText.fontStyle = FontStyles.Bold;
        SetUpText.text = "Measuring" + dots + "\n Please keep your posture";

        while (count < MeasureFrame)
        {
            if (count % 10 == 0)
            {
                dots += ".";
                SetUpText.text = "Measuring" + dots + "\n Please keep your posture";
            }

            earSum += float.Parse(poseData[1]);
            csDistSum += float.Parse(poseData[2]);

            count++;
            yield return new WaitForSeconds(0.2f);
        }
        setFinish(earSum, csDistSum);
        yield return null;
    }

    IEnumerator DoFadeIn(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * 3;
            yield return null;
        }

        yield return null;
    }
}

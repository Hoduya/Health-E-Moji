using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveManager : MonoBehaviour
{
    public Canvas MenuCanvas;
    public GameObject SettingCanvas;

    public CanvasGroup MenuCanvasGroup;
    public CanvasGroup SettingCanvasGroup;
    public Button settingButton;

    private void Awake()
    {
        SetActiveCanvas("Menu");
        settingButton.onClick.AddListener(settingButtonClick);
    }

    public void settingButtonClick()
    {
        SetActiveCanvas("Setting");
    }

    public void SetActiveCanvas(string canvasString)
    {
        SetAllDisabled();
           
        switch (canvasString)
        {
            case "Menu":
                MenuCanvas.enabled = true;
                Fade(MenuCanvasGroup);
                break;

            case "Setting":
                SettingCanvas.SetActive(true);
                Fade(SettingCanvasGroup);
                break;
        }
    }

    private void SetAllDisabled()
    {
        MenuCanvas.enabled = false;
        SettingCanvas.SetActive(false);
    }

    public void Fade(CanvasGroup canvasGroup)
    {
        StartCoroutine(DoFadeIn(canvasGroup));
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

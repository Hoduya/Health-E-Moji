using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public Button startButton;
    public Canvas menuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(480, 520, false);
        startButton.onClick.AddListener(startButtonClick);
    }

    private void OnEnable()
    {
        menuCanvas.enabled = true;
    }

    public void startButtonClick()
    {
        SceneManager.LoadScene("PlayScene");
    }
}

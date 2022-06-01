/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInManager:MonoBehaviour
{   
    [Header("LogIn")]
    public Button signInButton;
    public InputField inputID;
    public InputField inputPassWord;
    public Button SignUpButton;

    [Header("Sign UP")]
    public Button newSignUpButton;
    public InputField newInputID;
    public InputField newInputPassWord;


    public string LogInURL;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(480, 640, false);
        LogInURL = "http://localhost/login.php";
        signInButton.onClick.AddListener(signInButtonClick);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void signInButtonClick()
    {
        StartCoroutine(LoginCo());
    }

    IEnumerator LoginCo()
    {
        Debug.Log(inputID.text);
        Debug.Log(inputPassWord.text);

        WWWForm form = new WWWForm();
        form.AddField("Input_user", inputID.text);
        form.AddField("Input_pass", inputPassWord.text);

        WWW webRequest = new WWW(LogInURL, form);
        yield return webRequest;

        Debug.Log(webRequest.text);
    }

    public void signIn(string id, string pw)
    {
        userInfoInit();
    }

    public void userInfoInit()
    {
        loadMenuScene();
    }

    public void loadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}*/

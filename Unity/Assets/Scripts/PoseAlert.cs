using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PoseAlert : MonoBehaviour
{
    private float NormalEAR = 0.34f;
    private float NormalCSDist = 80.5f;

    private float EARThresh;
    private float CSDistThresh;

    private float ear;
    private float csDist;

    private int frameCount;
    private int totalBlink;
    private int totalBlinkPerMin;

    private const int EarIdx = 7;
    private const int CsDistIdx = 8;

    private const float AlertDuration = 0.5F;
    private Color normalColor = Color.white;
    private Color AlertColor = Color.red;

    private bool isFHP;
    private bool preFHP;

    public TimeManager timeManager;
    public GameObject DirectionalLight;
    private Light Light;
    public TMP_Text TimeText;
    public TMP_Text BlinkCntText;

    public AudioSource beepAudio;

    public delegate void perMinEventHandler(int i);
    public event perMinEventHandler changeHead;

    public void Start()
    {
        setThreshHold();

        frameCount = 0;
        totalBlink = 0;
        totalBlinkPerMin = 0;

        preFHP = false;
        isFHP = false;

        Light = DirectionalLight.GetComponent<Light>();

        timeManager = this.GetComponent<TimeManager>();

        InvokeRepeating("perMin", 62, 60);
    }

    public void Update()
    {
        if (isFHP && !preFHP)
        {
            beepAudio.Play();
            preFHP = true;
        }

        if (isFHP)
        {
            float t = Mathf.PingPong(Time.time, AlertDuration) / AlertDuration;
            Light.color = Color.Lerp(normalColor, AlertColor, t);
        }
        else {
            Light.color = normalColor;
        }

        TimeText.text = timeManager.getTimeString();
        BlinkCntText.text = totalBlinkPerMin.ToString();
    }

    // called once per image frame
    public void setPoseData(string[] p)
    {
        ear = float.Parse(p[EarIdx]);
        csDist = float.Parse(p[CsDistIdx]);

        detectEyeBlink();
        detectForwardHeadPosture();
    }

    public void setThreshHold()
    {
        NormalEAR = UserInfo.instance.NormalEAR;
        NormalCSDist = UserInfo.instance.NormalCSDist;
        EARThresh = 0.3f;
        CSDistThresh = 70f;
    }

    public void detectEyeBlink()
    {
        if (ear < EARThresh)
        {
            frameCount += 1;
        }
        else
        {
            if(frameCount > 2)
            {
                totalBlink += 1;
                totalBlinkPerMin += 1;
                frameCount = 0;
            }
        }
    }

    public void detectForwardHeadPosture()
    {

        if (csDist < CSDistThresh)
        {
            isFHP = true;
        }
        else
        {
            isFHP = false;
            preFHP = false;
        }
    }

    public void perMin()
    {
        changeHead(totalBlinkPerMin < 12 ? 1 : 0);
        totalBlinkPerMin = 0;
    }
}

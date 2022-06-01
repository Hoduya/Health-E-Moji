using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TimeManager : MonoBehaviour
{
    float time;
    int h, m, s;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlaySceneManager.isProcessRunning) return;

        time += Time.deltaTime;

        h = ((int)time / 3600);
        m = ((int)time / 60 % 60);
        s = ((int)time % 60);
    }

    public string getTimeString()
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);
    }   
}

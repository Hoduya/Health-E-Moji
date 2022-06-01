using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoseMapping
{
    private float posX, posY;
    private float sizeHead;
    private float roll, yaw, pitch;

    private const int PosXIdx = 1;
    private const int PosYIdx = 2;
    private const int SizeHeadIdx = 3;
    private const int RollIdx = 4;
    private const int YawIdx = 5;
    private const int PitchIdx = 6;

    public GameObject smileHead;
    public GameObject sadHead;
    private GameObject CurrentHead;

    // Start is called before the first frame update
    public PoseMapping(GameObject smileHead, GameObject sadHead)
    {
        this.smileHead = smileHead;
        this.sadHead = sadHead;

        CurrentHead = smileHead;
        CurrentHead.transform.position = new Vector3(0, 0.7f, 0.48f);

        this.sizeHead = 1;
    }

    // 0 for smile, 1 for sad
    public void changeHead(int head)
    {
        if (head == 0 && CurrentHead.Equals(smileHead))
            return;
        if (head == 1 && CurrentHead.Equals(sadHead))
            return;
        
        CurrentHead.SetActive(false);
        CurrentHead = (head == 0) ? smileHead : sadHead;
        CurrentHead.SetActive(true);
    }

    // Update is called once per frame
    public void updatePose()
    {
        Vector3 TargetPosition = new Vector3(-posX, posY, 0.0f);
        Quaternion TargetRotaion = Quaternion.Euler(pitch, -yaw, roll);
        Vector3 TargetScale = new Vector3(sizeHead, sizeHead, sizeHead);

        // Smooth transform, rotation
        CurrentHead.transform.position = Vector3.Slerp(CurrentHead.transform.position, TargetPosition, Time.deltaTime * 5.0f);
        CurrentHead.transform.rotation = Quaternion.Slerp(CurrentHead.transform.rotation, TargetRotaion, Time.deltaTime * 5.0f);
        CurrentHead.transform.localScale = Vector3.Slerp(CurrentHead.transform.localScale, TargetScale, Time.deltaTime * 5.0f);
    }

    public void setPoseData(string[] p)
    {
        pos(int.Parse(p[PosXIdx]), int.Parse(p[PosYIdx]));
        scaleHead(int.Parse(p[SizeHeadIdx]));
        turn(int.Parse(p[RollIdx]), int.Parse(p[YawIdx]), int.Parse(p[PitchIdx]));
    }

    public void pos(int x, int y)
    {
        posX = map(x, -100, 500, -1.7f, 1.7f);
        posY = map(y, 0, 480, 1.3f, -0.2f);
    }

    public void scaleHead(int head)
    {
        sizeHead = map(head, 70, 370, 0.9f, 2.8f);
    }

    public void turn(int r, int y, int p)
    {
        roll = map(r, -50, 50, -40, 40);

        yaw = map(y, -70, 70, -30, 30);

        pitch = map(p, -35f, 60f, 30, -30);
    }

    public static float map(int x, float x1, float x2, float y1, float y2)
    {
        var m = (y2 - y1) / (x2 - x1);
        var c = y1 - m * x1; // point of interest: c is also equal to y2 - m * x2, though float math might lead to slightly different results.

        return m * x + c;
    }
}


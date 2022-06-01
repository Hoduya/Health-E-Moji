using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo: MonoBehaviour
{
    public static UserInfo instance = null;

    public const float DefaultNormalCSDIst= 80.4f;

    private string dataBaseID;
    private string id;
    private string password;
    private float normalEAR;
    private float normalCSDist;
    private float EARThresh;
    private float CSDistThresh;
    
    public float NormalEAR 
    {
        get { return normalEAR; }
        set { normalEAR = value; }
    }

    public float NormalCSDist
    {
        get { return normalCSDist; }
        set { normalCSDist = value; }
    }


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        normalEAR = 0.37f;
        normalCSDist = 100f;

        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {

    }
}

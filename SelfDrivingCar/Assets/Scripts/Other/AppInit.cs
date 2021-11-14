using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppInit : MonoBehaviour
{
    public InputField input;

    private void Start()
    {
        input.text = "HERE 1";
        HDebug();
        DB_Init.CheckTables();
        HDebug();
    }

    public void HDebug()
    {
        input.text = "HERE 2";
        //text.text = "URI=file:" + Application.dataPath + "/DB/DeepNet.db";  //Path to database
        try
        {
            //input.text = Application.persistentDataPath;
            input.text = DB_Init.CheckIfTableExists("Network").ToString();
        }
        catch (Exception e)
        {
            input.text = e.ToString();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppInit : MonoBehaviour
{
    private void Start()
    {
        DB_Init.CheckTables();
    }
}

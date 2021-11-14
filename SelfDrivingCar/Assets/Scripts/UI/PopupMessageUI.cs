using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupMessageUI : MonoBehaviour
{
    public Animator popupMessageAnimator;
    public GameObject popupMessageContainer;

    public Action callBack;

    public void OkBtn()
    {
        StartCoroutine("OkCoroutine");
    }

    IEnumerator OkCoroutine()
    {
        popupMessageAnimator.SetTrigger("Hide");
        yield return new WaitForSecondsRealtime(0.25f);
        popupMessageContainer.SetActive(false);
        callBack();
    }
}

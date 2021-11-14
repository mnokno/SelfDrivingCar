using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupYesNoUI : MonoBehaviour
{

    public Animator popupYesNoAnimator;
    public GameObject popupYesNoContainer;

    public Action<Anwser> callBack;
    
    public void YesBtn()
    {
        StartCoroutine("GotAnwserCoroutine", Anwser.Yes);
    }

    public void NoBtn()
    {
        StartCoroutine("GotAnwserCoroutine", Anwser.No);
    }


    IEnumerator GotAnwserCoroutine(Anwser anwser)
    {
        popupYesNoAnimator.SetTrigger("Hide");
        //yield return new WaitForSecondsRealtime(0.25f);
        yield return new WaitForSecondsRealtime(0.00f);
        popupYesNoContainer.SetActive(false);
        callBack(anwser);
    }

    public enum Anwser
    {
        Yes,
        No
    }
}

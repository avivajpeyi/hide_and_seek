using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Countdown : MonoBehaviour {
    public float timerWillRunFor = 3;
    public float remainingTime = 3;
    public TMP_Text textBox;

    
    // Use this for initialization
    void Start ()
    {
        textBox.text = " ";
    }

    public void ResetTimer(float timerTime)
    {
        this.enabled = true;
        timerWillRunFor = timerTime;
        remainingTime = timerWillRunFor;
        WriteTimeToText();
    }

    private void WriteTimeToText()
    {
        textBox.text = Mathf.Round(remainingTime).ToString();
    }

    private void UpdateTimer()
    {
        if (remainingTime >= 0)
        {
            remainingTime -= Time.deltaTime;
            WriteTimeToText();    
        }
        else
        {
            remainingTime = 0;
            textBox.text = " ";
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update ()
    {
        UpdateTimer();
    }
}
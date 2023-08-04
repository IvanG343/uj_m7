using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageTimer : MonoBehaviour {

    public float maxTime;
    public bool tick;

    public Image timerImg;
    public Text timerText;
    private float currentTime;

    void Start() {
        timerImg = GetComponent<Image>();
        currentTime = maxTime;
    }

    void Update() {
        currentTime -= Time.deltaTime;
        tick = false;

        if(currentTime <= 0) {
            tick = true;
            currentTime = maxTime;
        }

        timerImg.fillAmount = currentTime / maxTime;
        timerText.text = Mathf.Round(currentTime).ToString();
    }
}

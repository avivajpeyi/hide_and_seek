using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaAnimator : MonoBehaviour
{
    public TMP_Text command;
    private Light sceneLight;


    public GameObject ground;
    public Material groundMat;
    public Material seekerMat;
    public Material hiderMat;
    
    
    // Start is called before the first frame update
    void OnEnable()
    {
        sceneLight = FindObjectOfType<Light>();

    }

    public void ResetAnimations()
    {
        sceneLight.intensity = 2;
        ground.GetComponent<Renderer>().material = groundMat;

    }


    IEnumerator LerpTextAlpha(TMP_Text textToFade, float endValue, float duration)
    {
        float time = 0;
        Color startValue = textToFade.color;
        Color endColor = startValue;
        endColor.a = 0;

        while (time < duration)
        {
            textToFade.color = Color.Lerp(startValue, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        textToFade.color = endColor;
    }
    
    
    IEnumerator LerpLightIntensity(Light lightToFade, float endIntensity, float duration)
    {
        float time = 0;
        float startValue = lightToFade.intensity;

        while (time < duration)
        {
            lightToFade.intensity= Mathf.Lerp(startValue, endIntensity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        lightToFade.intensity = endIntensity;
    }
    
    public void DimLight(float duration = 3)
    {
        StartCoroutine(LerpLightIntensity(sceneLight, 0.5f, duration));
    }

    public void ResetLight()
    {
        sceneLight.intensity = 2;
    }


    public void FadeCommandText(float duration = 2f)
    {
        StartCoroutine(LerpTextAlpha(command, 0, duration));
    }

    public void HiderWon()
    {
        ground.GetComponent<Renderer>().material = hiderMat;
    }
    
    public void SeekerWon()
    {
        ground.GetComponent<Renderer>().material = seekerMat;
    }
}

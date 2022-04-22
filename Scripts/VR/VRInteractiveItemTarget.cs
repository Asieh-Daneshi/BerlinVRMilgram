using System;
using System.Collections;
using Experiment.Scripts.Core;
using UnityEngine;

using System.IO;
public class VRInteractiveItemTarget : VRInteractiveItemBaseclass
{
    public AudioSource audioCorrect;
    public AudioSource audioWrong;
    bool ErrorSound;
    GameObject goFire;


    public const string strSettingAudioFeeback = "Audio Error Feedback";


    public static bool IsEnabledAudioFeedback
    {
        get => Convert.ToBoolean(PlayerPrefs.GetInt(strSettingAudioFeeback));
        set => PlayerPrefs.SetInt(strSettingAudioFeeback, Convert.ToInt16(value));
    }


    void Start()
    {
        ErrorSound = IsEnabledAudioFeedback;
        goFire = transform.Find("FirePS").gameObject;   // AD: in Environment, we have FirePS, which is the fire object. This code finds FirePS and defines a gameObject named goFire with that!
    }

    public override void FocusIn()
    {
        //      Debug.Log("VRInteractiveItemTarget::focusIn: " + gameObject.name);
        bHasFocus = true;

        if (goFire) StartCoroutine(DimLights());

        if (ExperimentManager.expRunning && !ExperimentManager.expPaused)
        {
            LogManager.instance.WriteTimeStampedEntry("TargetHit: " + gameObject.name);

            if (ErrorSound)
            {
                if (ExperimentManager.currentTargetPpn == "choose")
                    audioCorrect.Play();
                else if (ExperimentManager.currentTargetPpn != name)
                    audioWrong.Play();
                else
                    audioCorrect.Play();
            }
        }
    }

    public override void FocusOut()
    {
        Debug.Log("VRInteractiveItemTarget::focusOut: " + gameObject.name);
        bHasFocus = false;
    }

    #region FireStarts
    public void TurnFireOn(bool b)
    {
        if (b)
        {
            goFire.GetComponent<ParticleSystem>().Play();   // AD: ParticleSystem is in FirePS, which is now in our gameObject "goFire". It makes the fire start!
            goFire.GetComponent<Light>().intensity = 2.5f;  // AD: Light is also in FirePS, which is now in our gameObject "goFire"
        }
        else
        {
            StartCoroutine(DimLights());    // AD: This line calls the function "DimLights" that stops the fire!
        }
    }
    #endregion

    #region FireStops
    IEnumerator DimLights()
    {
        //       Debug.Log("VRInteractiveItemTarget::DimLights:");
        if (goFire)
        {
            var fIntensity = goFire.GetComponent<Light>().intensity; // avoid turning it on... 1.5f;    // AD: This line finds the intensity of the fire in the scene.  

            while (fIntensity > 0.21)   // AD: In line 66 we made the fire start with the intensity of 2.5f every time that we trigger it. So this line actually checks if the intensity is higher than baseline or not, and if it is, starts decreasing until we reach the baseline!
            {
                fIntensity = Mathf.Lerp(fIntensity, 0.2f, .25f);    // AD: Mathf.Lerp(float a, float b, float t) linearly interpolates between between a and b by t. When t = 0 returns a; When t = 1 return b; When t = 0.5 returns the midpoint of a and b.
                // So, the above line simply linearly decreases the current intensity to 0.2f, with the slope of 0.25f
                goFire.GetComponent<Light>().intensity = fIntensity;    // AD: finds the current intensity to see if the while loop is still valid or not!
                //print("fIntensity: " + fIntensity);
                yield return new WaitForSecondsRealtime(500 / 1000);    // AD: 'WaitForSecondsRealtime' Suspends the coroutine execution for the given amount of seconds using unscaled time. It helps to slow down the fire turning off!
            }

            goFire.GetComponent<Light>().intensity = 0f;    // AD: When the intensity is below 0.21, it simply stops the light completely
            //  yield return new WaitForSecondsRealtime(2); // AD: Here we can make a delay between stopping the light and stopping the fire!
            goFire.GetComponent<ParticleSystem>().Stop();   // AD: Here we stop the fire! when the intensity is below 0.21, we stop the fire completely
        }
    }
    #endregion
}
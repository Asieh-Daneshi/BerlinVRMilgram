using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Experiment.Scripts.Core
{
    public class ExperimentManager : MonoBehaviour
    {
        Text textUI;
        TextAsset txtAssetPause;
        TextAsset txtAssetThanks;

        bool bBlank;
        Canvas canvasUI;
        ExperimentConfigNew experimentConfigNew;
        public static bool expRunning;
        public static bool expPaused;
        public static string currentTarget;
        public static string currentTargetPpn;

        [SerializeField] List<GameObject> lCharacters;

        //  IDictionary<string, GameObject> dictTargets;

        public HMDRecord hmdRecord;


        void Start()
        {
            //         GameObject go = GameObject.Find("Canvas");
            //         if (go != null) {
            //             canvasUI = go.GetComponent<Canvas>();
            //         } else {
            // #if UNITY_EDITOR
            //             EditorUtility.DisplayDialog("Error", "Canvas not found", "OK");
            // #endif
            //             Application.Quit();
            //         }
            //
            //         go = GameObject.Find("TextUI");
            //         if (go != null) {
            //             textUI = go.GetComponent<Text>();
            //         } else {
            // #if UNITY_EDITOR
            //             EditorUtility.DisplayDialog("Error", "TextUI not found", "OK");
            // #endif            
            //             Application.Quit();
            //         }

            #region pause between blocks
            TextAsset txtAssetPause;
            var strFNPause = Path.Combine(Application.streamingAssetsPath, "Pause.txt");    // AD: Use the StreamingAssets folder to store Assets. At run time, Application.streamingAssetsPath provides the path to the folder. Add the Asset name to Application.streamingAssetsPath.
            if (!File.Exists(strFNPause))
            {
                var strErrorMessage = "Error:\n\"" + strFNPause + "\"\ndoes not exist";
                txtAssetPause = new TextAsset(strErrorMessage);
            }
            else
            {
                txtAssetPause = new TextAsset(File.ReadAllText(strFNPause));
            }
            #endregion

            #region thank message at the ned of the experiment and ending the experiment
            TextAsset txtAssetThanks;
            var strFNThanks = Path.Combine(Application.streamingAssetsPath, "Thanks.txt");
            if (!File.Exists(strFNThanks))
            {
                var strErrorMessage = "Error:\n\"" + strFNThanks + "\"\ndoes not exist";
                txtAssetThanks = new TextAsset(strErrorMessage);
            }
            else
            {
                txtAssetThanks = new TextAsset(File.ReadAllText(strFNThanks));
            }

            expRunning = false;
            expPaused = false;
            #endregion
            // string Nr = EnterNR.NumberText;
            var jsonfile = "00001.json"; //string.Format("{0}.json", Nr);   //AD: introducing the file containing the experimental parameters

            //--  TextAsset txtAssetJSON = (TextAsset)Resources.Load("exp_v04");
            var strFNJSON = Path.Combine(Application.streamingAssetsPath, jsonfile);
            experimentConfigNew = JsonConvert.DeserializeObject<ExperimentConfigNew>(File.ReadAllText(strFNJSON));


            ////Debug.Log(instructions_condition);

            TextAsset txtAssetInstructions1;
            var condNR = experimentConfigNew.condition;
            var instr11 = string.Format("instructions1{0}.txt", condNR);
            var strFNInstructions1 = Path.Combine(Application.streamingAssetsPath, instr11);


            if (!File.Exists(strFNInstructions1))
            {
                var strErrorMessage = "Error:\n\"" + strFNInstructions1 + "\"\ndoes not exist";
                txtAssetInstructions1 = new TextAsset(strErrorMessage);
            }
            else
            {
                txtAssetInstructions1 = new TextAsset(File.ReadAllText(strFNInstructions1));
            }

            bBlank = true;

            // LogManager.instance.WriteEntry("Participant: " + experimentConfigNew.participant);
            hmdRecord.WriteHeader();
            //LogManager.instance.WriteEntry("Condition: " + experimentConfigNew.condition);
            hmdRecord.WriteHeader();

            var lstPositions = experimentConfigNew.getPositions();
            for (var i = 0; i < lCharacters.Count; i++)
            {
                var position = lstPositions[i];
                var c = lCharacters.ElementAt(i);
                if (c != null)
                {
                    var temp = new Vector3(position[0], c.transform.position.y, position[1]);
                    c.transform.position = temp;
                }
            }

            TurnAllHeads("Panel");
        }

        void Update()
        {
            if (!expRunning) StartCoroutine(RunExperiment());
        }

        IEnumerator RunExperiment()
        {
            var fSdSOA = experimentConfigNew.soa_sd_ms;
            var timeExpMS = experimentConfigNew.time_exp_ms;
            var condition = experimentConfigNew.condition;

            LightTargetFires(false);

            VRInteractiveItemPanel vriiPanel = null;
            var goPanel = GameObject.Find("Panel");
            if (goPanel) vriiPanel = goPanel.GetComponent<VRInteractiveItemPanel>();

            expRunning = true;

            var blocks = experimentConfigNew.blocks;

            var iNrBlocks = blocks.Count;
            var iBlockCount = 0;
            foreach (var b in blocks)
            {
                iBlockCount++;

                SetScreenBlank(false);
                // looping over blocks
                var trials = b.trials;
                foreach (var t in trials)
                {
                    var fMeanSOA = t.soa_means_ms;
                    var normalDist = new Normal(fMeanSOA, fSdSOA);

                    hmdRecord.StartRecording();

                    var timeBlankMS = t.time_blank_ms;
                    yield return new WaitForSecondsRealtime(timeBlankMS / 1000f);

                    var strTarget = "target0" + t.gaze_loc;
                    var target = GameObject.Find(strTarget);
                    var targetPos = target.transform.position;
                    currentTarget = strTarget;
                    var strTargetPpn = "";

                    if (condition == 1)
                    {
                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }
                        else
                        {
                            strTargetPpn = "target0" + t.audio_cue;
                            currentTargetPpn = strTargetPpn;
                        }
                    }

                    if (condition == 2)
                    {
                        if (t.audio_cue == 2)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }

                        if (t.audio_cue == 1)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = strTargetPpn;
                        }

                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target02";
                            currentTargetPpn = strTargetPpn;
                        }
                    }

                    if (condition == 3)
                    {
                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }

                        if (t.audio_cue == 2)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = strTargetPpn;
                        }

                        if (t.audio_cue == 1)
                        {
                            strTargetPpn = "target02";
                            currentTargetPpn = strTargetPpn;
                        }
                    }

                    if (condition == 4)
                    {
                        if (t.audio_cue == 2)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }

                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = strTargetPpn;
                        }

                        if (t.audio_cue == 1)
                        {
                            strTargetPpn = "target02";
                            currentTargetPpn = strTargetPpn;
                        }
                    }

                    if (condition == 5)
                    {
                        if (t.audio_cue == 1)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }

                        if (t.audio_cue == 2)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = strTargetPpn;
                        }

                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target02";
                            currentTargetPpn = strTargetPpn;
                        }
                    }

                    if (condition == 6)
                    {
                        if (t.audio_cue == 1)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = "choose";
                        }

                        if (t.audio_cue == 3)
                        {
                            strTargetPpn = "target01";
                            currentTargetPpn = strTargetPpn;
                        }

                        if (t.audio_cue == 2)
                        {
                            strTargetPpn = "target02";
                            currentTargetPpn = strTargetPpn;
                        }
                    }


                    var targetPpn = GameObject.Find(strTargetPpn);
                    var targetPosPpn = targetPpn.transform.position;

                    LightTargetFires(true);

                    var audioCue = t.audio_cue;
                    var gazeLoc = t.gaze_loc;
                    var charactersGazing = t.characters_gazing;

                    if (condition == 1)
                        if (t.audio_cue != 3)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 2)
                        if (t.audio_cue != 2)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 3)
                        if (t.audio_cue != 3)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 4)
                        if (t.audio_cue != 2)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 5)
                        if (t.audio_cue != 1)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 6)
                        if (t.audio_cue != 1)
                        {
                            //LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            //LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }


                    //LogManager.instance.WriteTimeStampedEntry("audio cue:" + audio_cue);
                    //LogManager.instance.WriteTimeStampedEntry("gaze_loc:" + gaze_loc);
                    //--double fSOAms = normalDist.Sample();
                    //--//LogManager.instance.WriteTimeStampedEntry("SOA_ms:" + fSOAms.ToString());

                    //LogManager.instance.WriteTimeStampedEntry("characters:" + characters_gazing.Length);
                    //LogManager.instance.WriteTimeStampedEntry("TargetHit:");
                    PlayAudioCue.audioPlayer.playAudioCue(audioCue);

                    /* create array with delays for each character */
                    var arrDelays = new double[charactersGazing.Length];
                    normalDist.Samples(arrDelays);
                    Array.Sort(arrDelays);


                    /* SOA between audio event and character response
                 * ATM single wait time for all characters */
                    // -- yield return new WaitForSecondsRealtime((float)(fSOAms / 1000));


                    // ADD THE DELAYS HERE

                    double fDelayOld = 0;
                    var ii = 0;

                    foreach (var iChar in charactersGazing)
                    {
                        var fDelay = Math.Abs(arrDelays[ii]); // abs, just in case we got a negative number...
                        //LogManager.instance.WriteTimeStampedEntry("character:" + iChar.ToString() + ", delay: " + fDelay.ToString()); //could this 

                        /* we only wait the difference in delay */
                        var fDeltaDelay = fDelay - fDelayOld;
                        Debug.Log("fDeltaDelay: " + fDeltaDelay);
                        yield return new WaitForSecondsRealtime((float) (fDeltaDelay / 1000));
                        var c = lCharacters.ElementAt(iChar);
                        if (c != null)
                        {
                            var cc = (charControl) c.GetComponent(typeof(charControl));
                            cc.setGazeTarget(targetPos);
                        }

                        fDelayOld = fDelay;
                        ii++;
                    }


                    yield return new WaitForSecondsRealtime(timeExpMS / 1000f);

                    yield return new WaitUntil(() => vriiPanel.hasFocus());


                    LightTargetFires(false);
                    currentTarget = "";
                    currentTargetPpn = "";

                    // resetAllHeads();
                    /* only do it for the ones that we set gaze location, or the non-lookers show a head jolt... */
                    foreach (var iChar in charactersGazing)
                    {
                        var c = lCharacters.ElementAt(iChar);
                        if (c != null)
                        {
                            var cc = (charControl) c.GetComponent(typeof(charControl));
                            cc.resetGaze();
                        }
                    }

                    hmdRecord.StopAndWriteData();
                }

                Debug.Log("end of block: pause");
                expPaused = true;

                if (iBlockCount < iNrBlocks)
                {
                    // don't do pause screen after last block...
                    var strFNPause = Path.Combine(Application.streamingAssetsPath, "Pause.txt");
                    txtAssetPause = new TextAsset(File.ReadAllText(strFNPause));
                    // textUI.text = txtAssetPause.text;
                    SetScreenBlank(true);

                    yield return new WaitUntil(() => expPaused == false);
                }
            }

            expRunning = false;
            var strFNThanks = Path.Combine(Application.streamingAssetsPath, "Thanks.txt");
            txtAssetThanks = new TextAsset(File.ReadAllText(strFNThanks));
            // textUI.text = txtAssetThanks.text;
            SetScreenBlank(true);

            yield return new WaitForSecondsRealtime(1);
        }

        void LightTargetFires(bool b)
        {
            var gos = GameObject.FindGameObjectsWithTag("target");
            foreach (var go in gos)
            {
                var cc = (VRInteractiveItemTarget) go.GetComponent(typeof(VRInteractiveItemTarget));
                cc.TurnFireOn(b);
            }
        }

        void TurnAllHeads(string _strTarget)
        {
            var target = GameObject.Find(_strTarget);
            if (target != null)
            {
                var targetPos = target.transform.position;
                foreach (var c in lCharacters)
                {
                    //Debug.Log("# item: " + c);
                    var cc = (charControl) c.GetComponent(typeof(charControl));
                    cc.setGazeTarget(targetPos);
                }
            }
            else
            {
                Debug.Log("ERROR: Target not found: " + _strTarget);
            }
        }

        void ResetAllHeads()
        {
            foreach (var c in lCharacters)
            {
                var cc = (charControl) c.GetComponent(typeof(charControl));
                cc.resetGaze();
            }
        }

        void ToggleBlankScreen()
        {
            if (!bBlank)
            {
                //goCanvas.GetComponent<Image>().color = new Color(0, 0, 0, 255);
                canvasUI.enabled = true;
                ResetAllHeads();
                bBlank = true;
            }
            else
            {
                //goCanvas.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                canvasUI.enabled = false;
                bBlank = false;
            }
        }

        void SetScreenBlank(bool _bBlank)
        {
            // if (_bBlank) {
            //     if (m_ShowDebugMainController) {
            //         print("setSceenBlank::blank");
            //     }
            //
            //     canvasUI.enabled = true;
            //     ResetAllHeads();
            //     bBlank = true;
            // } else {
            //     if (m_ShowDebugMainController) {
            //         print("setSceenBlank::unblank");
            //     }
            //
            //     canvasUI.enabled = false;
            //     bBlank = false;
            // }
        }
    }
}
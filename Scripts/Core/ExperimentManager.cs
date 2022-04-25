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
                //textUI.text = txtAssetPause.text;
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

            #region instructions at the beginning of the experiment
            if (!File.Exists(strFNInstructions1))
            {
                var strErrorMessage = "Error:\n\"" + strFNInstructions1 + "\"\ndoes not exist";
                txtAssetInstructions1 = new TextAsset(strErrorMessage);
            }
            else
            {
                txtAssetInstructions1 = new TextAsset(File.ReadAllText(strFNInstructions1));
            }
            #endregion
            bBlank = true;

            #region write the data with the name of the participant and condition
            LogManager.instance.WriteEntry("Participant: " + experimentConfigNew.participant);
            hmdRecord.WriteHeader();
            LogManager.instance.WriteEntry("Condition: " + experimentConfigNew.condition);
            hmdRecord.WriteHeader();
            #endregion

            #region updating the positions of the agents
            var lstPositions = experimentConfigNew.getPositions();
            for (var i = 0; i < lCharacters.Count; i++)
            {
                var position = lstPositions[i];
                var c = lCharacters.ElementAt(i);
                if (c != null)
                {
                    var temp = new Vector3(position[0], c.transform.position.y, position[1]);   // AD: position in y direction doesn't change and there is no need to update it!
                    c.transform.position = temp;
                }
            }
            #endregion
            TurnAllHeads("Panel");  // AD: "TurnAllHeads" is a function that is introduced in line 460
        }

        void Update()
        {
            if (!expRunning) StartCoroutine(RunExperiment());   // AD: if the experiment is not running, start "RunExperiment" Coroutine
            /* AD: "public Coroutine StartCoroutine(IEnumerator routine)" Starts a Coroutine.
            The execution of a coroutine can be paused at any point using the yield statement. When a yield statement is used, the coroutine pauses execution and automatically resumes at the next frame.*/
        }

        IEnumerator RunExperiment()
        {
            // AD: getting parameters of the expeiment from json files
            var fSdSOA = experimentConfigNew.soa_sd_ms;
            var timeExpMS = experimentConfigNew.time_exp_ms;
            var condition = experimentConfigNew.condition;

            LightTargetFires(false);    // AD: This is a function introduced in line 454

            // AD: activating the panel that the video is showing on it. It is in "Environment> projection stage". Also, the 8 min video is in this object!
            VRInteractiveItemPanel vriiPanel = null;
            var goPanel = GameObject.Find("Panel");
            if (goPanel) vriiPanel = goPanel.GetComponent<VRInteractiveItemPanel>();

            expRunning = true;

            var blocks = experimentConfigNew.blocks;    // AD: gets the blocks of the experiment from json files

            var iNrBlocks = blocks.Count;   // AD: number of blocks in the experiment
            var iBlockCount = 0;    // AD: setting the initial value for the blocks
            foreach (var b in blocks)
            {
                iBlockCount++;

                SetScreenBlank(false);  // AD: this line is supposed to deactivate the function introduced in line 515. But, that function is commented for now, and if we uncomment it, we will realize that the object that this function is attached to it is gone!
                // looping over blocks
                var trials = b.trials;  // AD: gets the "trials" variable from the json files
                foreach (var t in trials)   // AD: t goes on the trials. In other words, it goes from 1 to 66
                {
                    var qq = 0;
                    qq++;
                    print("trial " + qq);
                    var fMeanSOA = t.soa_means_ms;  // AD: gets the starting time of the audio for the current trial
                    var normalDist = new Normal(fMeanSOA, fSdSOA);  // AD: "fSdSOA" comes from the "soa_sd_ms" in json files. It is always zero. Therefore, this Normal distribution does nothing! But, if we want to set different gaze times for each agent (line 378), we should change the standard deviation.

                    hmdRecord.StartRecording();     // AD: calls "hmdRecord" that we introduced in line 31 as "HMDRecord" which is a separate piece of code! So, here the data recording is started.

                    var timeBlankMS = t.time_blank_ms;  // AD: the time interval between the current trial and the previous trial.
                    yield return new WaitForSecondsRealtime(timeBlankMS / 1000f);

                    var strTarget = "target0" + t.gaze_loc; // AD: "gaze_loc" comes from json files (it can be either 1 or 2, depending on the target that the agents in the current trial are supposed to look at. So, "strTarget" can be either target01 or target02. Both of them are in gameObjects.
                    var target = GameObject.Find(strTarget);
                    var targetPos = target.transform.position;  // AD: find the position of the current target.
                    currentTarget = strTarget;
                    var strTargetPpn = "";  // AD: defines a new variable "strTargetPpn", which its initial value is empty!

                    #region permuting audios in "currentTargetPpn"
                    /* AD: in this region all the possible permutations from 3 different audio cues are generated. "currentTargetPpn" can be 1, 2, or "choose", meaning that the participant must look at target 1, 2, or has the option to choose between them!
                    also, we have 3 different audios: 1) gunshot, 2) collapse, and 3) glass breaking. 3 different audios and three option for placing each of them ("currentTargetPpn") makes 6 permutations!*/

                    if (condition == 1)     // AD: condition is extracted from json files in line 150. We have 6 different conditions. Each json file has 1 condition. They are not shuffled. So, files 1 to 6 are corresponding to conditions 1 to 6, and then these will repeat again, meaning that 8th file for example has condition 2.
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
                    #endregion

                    var targetPpn = GameObject.Find(strTargetPpn);
                    var targetPosPpn = targetPpn.transform.position;

                    LightTargetFires(true);

                    var audioCue = t.audio_cue;     // AD: comes from json files. It can be 1, 2, or 3.
                    var gazeLoc = t.gaze_loc;     // AD: comes from json files. It can be either 1 or 2, depending on the target that the agents are supposed to look at in each trial.
                    var charactersGazing = t.characters_gazing;     // AD: comes from json files, the agents that look at one of the targets in each trial.
                    #region record experiment configuration
                    // in this region we generally record audio_cue and gaze_loc. We also record the target name and position if the "currentTargetPpn" is not "choose".
                    if (condition == 1)
                        if (t.audio_cue != 3)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 2)
                        if (t.audio_cue != 2)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 3)
                        if (t.audio_cue != 3)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 4)
                        if (t.audio_cue != 2)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 5)
                        if (t.audio_cue != 1)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    if (condition == 6)
                        if (t.audio_cue != 1)
                        {
                            LogManager.instance.WriteTimeStampedEntry("target:" + strTargetPpn);
                            LogManager.instance.WriteTimeStampedEntry("target position:" + targetPosPpn.ToString());
                        }

                    // AD: in the next two lines, we generally save "audio_cue" and "gaze_loc"
                    LogManager.instance.WriteTimeStampedEntry("audio cue:" + t.audio_cue);
                    LogManager.instance.WriteTimeStampedEntry("gaze_loc:" + t.gaze_loc);
                    #endregion
                    //--double fSOAms = normalDist.Sample();
                    //--//LogManager.instance.WriteTimeStampedEntry("SOA_ms:" + fSOAms.ToString());

                    LogManager.instance.WriteTimeStampedEntry("characters:" + t.characters_gazing.Length);  // AD: number of agents looking at the target
                    LogManager.instance.WriteTimeStampedEntry("TargetHit:"+ t.gaze_loc);
                    PlayAudioCue.audioPlayer.playAudioCue(audioCue);

                    // AD: create array with delays for each character. Currently, in line 178, the standard deviation is zero. So, the normal distribution is just a dirac delta function at the mean value of the Normal dictribution.
                    var arrDelays = new double[charactersGazing.Length];
                    normalDist.Samples(arrDelays);
                    Array.Sort(arrDelays);


                    /* SOA between audio event and character response
                 * ATM single wait time for all characters */
                    // -- yield return new WaitForSecondsRealtime((float)(fSOAms / 1000));


                    #region add delays
                    // AD: here, we find the delay for each agent from the beginnig of the trial. ATM waiting time is the same for all the agents. Therefore, this region currently does nothing

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
                    #endregion

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
                    textUI.text = txtAssetPause.text;
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

        #region turn on the fire on the supposed targets in the current trial
        void LightTargetFires(bool b)
        {
            var gos = GameObject.FindGameObjectsWithTag("target");  // AD: "GameObject.FindGameObjectsWithTag" returns an array of active GameObjects tagged tag. Returns empty array if no GameObject was found.
            foreach (var go in gos)
            {
                var cc = (VRInteractiveItemTarget) go.GetComponent(typeof(VRInteractiveItemTarget));    // AD: there is a piece of code named "VRInteractiveItemTarget". This line calls that
                cc.TurnFireOn(b);   // AD: "TurnFireOn" is a function in "VRInteractiveItemTarget".
            }
        }
        #endregion

        #region in this block, heads of all the agents that are supposed to look at one of the targets at each trial will be turned toward that target
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
        #endregion

        #region in this block, heads of all the agents that were turned in the current trial will move back to the original position
        void ResetAllHeads()
        {
            foreach (var c in lCharacters)
            {
                var cc = (charControl) c.GetComponent(typeof(charControl));
                cc.resetGaze();
            }
        }
        #endregion

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
            // if (_bblank) {
            //     if (m_showdebugmaincontroller) {
            //         print("setsceenblank::blank");
            //     }
            //
            //     canvasui.enabled = true;
            //     resetallheads();
            //     bblank = true;
            // } else {
            //     if (m_showdebugmaincontroller) {
            //         print("setsceenblank::unblank");
            //     }
            //
            //     canvasui.enabled = false;
            //     bblank = false;
            // }
        }
    }
}
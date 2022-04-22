using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

public class charControl : MonoBehaviour {

    private Animator anim;
    private Vector3 targetPos;
    private float lookIKWeight;
    private bool bReset;


    void Start() {
        anim = GetComponent<Animator>();
        // anim.speed = 0.1f; // this controls the speed of the idle animation
          bReset = false;
    }

    // Update is called once per frame
    void Update() {
    }

    void OnAnimatorIK(int layerIndex) {
        #region smoothly look at the target
        if (!bReset) {
            anim.SetLookAtWeight(lookIKWeight, .2f, 1.0f, 0f, 0f);  // AD: SetLookAtWeight(float weight, float bodyWeight = 0.2f, float headWeight = 1.0f, float eyesWeight = 0.0f, float clampWeight = 0.0f);
            anim.SetLookAtPosition(targetPos);  // AD: SetLookAtPosition(Vector3 lookAtPosition); 'lookAtPosition' is he position to look at.
            lookIKWeight = Mathf.Lerp(lookIKWeight, 1f, Time.deltaTime * 3.0f); // this is the trick for smoothing the gaze movement
        }
        #endregion

        #region smoothly turns the gaze back to the screen
        else
        {
            anim.SetLookAtWeight(lookIKWeight, .2f, 1.0f, 0f, 0f);
            anim.SetLookAtPosition(targetPos);
            lookIKWeight = Mathf.Lerp(lookIKWeight, 0, Time.deltaTime * 4.0f);
        }
        #endregion
    }


    public void setGazeTarget(Vector3 pos) {
        bReset = false;
        lookIKWeight = 0.0f;
        targetPos = pos;
    }

    public void resetGaze() {
        // Debug.Log("charControl::resetGaze()");
        /* this gaze resetting is a tricky business; whenever you set the lookIKWeight to 0, the head will spin to neutral at full speed.
         * therefore we need to slowly decrease lookIKWeight to have a smooth reset */

        lookIKWeight = 1.0f;
        bReset = true;
    }

}


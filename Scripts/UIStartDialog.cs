using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIStartDialog : MonoBehaviour
{
    //Make sure to attach these Buttons in the Inspector
    public Button btnStartExperiment, btnCancel;


    public Text textInstructions;
    public string nameSceneExperiment;

    void Start()
    {
        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        btnStartExperiment.onClick.AddListener(StartExperiment);
        btnCancel.onClick.AddListener(CancelExperiment);

        var mytxtData = (TextAsset) Resources.Load("Instructions");
        var txt = mytxtData.text;
        textInstructions.text = txt;
    }

    void StartExperiment()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("StartExperiment");

        SceneManager.LoadScene(nameSceneExperiment, LoadSceneMode.Single);
    }

    void CancelExperiment()
    {
        //Output this to console when the Button2 is clicked
        Debug.Log("CancelExperiment");
    }
}
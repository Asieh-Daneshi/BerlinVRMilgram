using UnityEngine;

public class PlayAudioCue : MonoBehaviour
{
    public static PlayAudioCue audioPlayer;

    public AudioSource audio01;
    public AudioSource audio02;
    public AudioSource audio03;

    //Awake is always called before any Start functions
    void Awake()
    {
        //        print("PlayAudioCue::Awake");
        if (audioPlayer == null) //if not, set instance to this
            audioPlayer = this;

        //If instance already exists and it's not this:
        else if
            (audioPlayer !=
             this) //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    public void playAudioCue(int iC)
    {
        switch (iC)
        {
            case 1:
                audio01.Play();
                break;
            case 2:
                audio02.Play();
                break;
            default:
                audio03.Play();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Q)) {
        //     //audio01.Play();
        //     playAudioCue(1);
        // }
        // if (Input.GetKeyDown(KeyCode.W)) {
        //     audio02.Play();
        // }
        // if (Input.GetKeyDown(KeyCode.E)) {
        //     audio03.Play();
        // }
    }
}
using UnityEngine;

public class PlayAudioFeedback : MonoBehaviour
{
    public static PlayAudioFeedback audioPlayer;

    public AudioSource audioCorrect;
    public AudioSource audioWrong;

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

    public void playAudioCorrect()
    {
        audioCorrect.Play();
        //audioWrong.PlayClipAtPoint(clip, new Vector3(5, 1, 2))
    }

    public void playAudioWrong()
    {
        audioWrong.Play();
        //audioWrong.PlayClipAtPoint(clip, new Vector3(5, 1, 2))
    }
}
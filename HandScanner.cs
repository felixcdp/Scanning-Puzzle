using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class HandScanner : MonoBehaviour {

    public GameObject[] solution;

    [SerializeField] private Text topPanelText;
    [SerializeField] private Text bottomPanelText;
    private AudioSource audioSource;
    public AudioClip sfxSuccess;
    public AudioClip sfxFailure;

    [SerializeField] string DoorControllerIPAddress;

    [SerializeField] private GameObject[] guess;

    private enum ScanStatus { Idle, Scanning, Success, Failure };
    private ScanStatus scanStatus = ScanStatus.Idle;

    public void onTouch(GameObject gO) {
        int finger = Input.touchCount - 1;
        guess[finger] = gO;
    }

    public void onRelease(GameObject gO) {
        for(int i = 0; i < guess.Length; i++) {
            if(guess[i] == gO) { guess[i] = null; }
        }
    }
    // Use this for initialization
    void Start () {
        topPanelText.text = "Place Hand";
        guess = new GameObject[solution.Length];
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {

        int numFingers = Input.touchCount;

        switch (scanStatus) {
            case ScanStatus.Idle:
                bottomPanelText.text = string.Format("{0} fingers scanned", numFingers);
                if (numFingers == solution.Length) {
                    scanStatus = ScanStatus.Scanning;
                }
                break;

            case ScanStatus.Scanning:
                Debug.Log("Scanning");
                topPanelText.text = "Scanning";

                bool allCorrect = true;
                for (int i = 0; i < solution.Length; i++) {
                    if(!(guess[i] != null && guess[i].Equals(solution[i]))){
                        allCorrect = false;
                    }
                }
                
                if(allCorrect) {
                    Debug.Log("Success");
                    audioSource.clip = sfxSuccess;
                    audioSource.Play();
                    StartCoroutine(SendUnlockTrigger());
                    topPanelText.text = "Access Granted";
                    scanStatus = ScanStatus.Success;
                }
                else {
                    Debug.Log("Failure");
                    audioSource.clip = sfxFailure;
                    audioSource.Play();
                    topPanelText.text = "Access Denied";
                    scanStatus = ScanStatus.Failure;
                }
                break;

            case ScanStatus.Success:
                if (numFingers == 0) {
                    topPanelText.text = "Place Hand";
                    scanStatus = ScanStatus.Idle;
                }
                break;

            case ScanStatus.Failure:
                if (numFingers == 0) {
                    topPanelText.text = "Place Hand";
                    scanStatus = ScanStatus.Idle;
                }
                break;

        }
       
    }

    IEnumerator SendLockTrigger() {
        string URL = string.Concat(DoorControllerIPAddress, "/L");
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(www.error);
            bottomPanelText.text = www.error;
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            bottomPanelText.text = www.downloadHandler.text;
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    IEnumerator SendUnlockTrigger() {
        string URL = string.Concat(DoorControllerIPAddress, "/H");
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError) {
            Debug.Log(www.error);
            bottomPanelText.text = www.error;
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            bottomPanelText.text = www.downloadHandler.text;
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }
}
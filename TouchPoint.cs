using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchPoint : MonoBehaviour
{
    public void onPress() {
        GetComponentInChildren<Text>().text = Input.touchCount.ToString();
        Debug.Log("Pressed");
    }

    public void onRelease() {
        GetComponentInChildren<Text>().text = "";
        Debug.Log("Released");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
https://youtu.be/AtOX6bXcQJE
**/
public class OculusDebug : MonoBehaviour
{

    public static OculusDebug Instance;

    bool inMenu;
    Text logText;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        var rt = DebugUIBuilder.instance.AddLabel("Debug"); // rectTransform
        logText = rt.GetComponent<Text>();
        DebugUIBuilder.instance.Show();
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }

    }

    public void log(string message)
    {
        logText.text = message;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
[Script Header] FPSCounter Version 0.0.1
Created by: Will
Date: 10/10/2018
Description: Calculate FPS

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour 
{
    #region Fields/Properties
    [SerializeField]
    float updateCheckTime = 1f;

    Text text;
    #endregion

    #region Methods
    IEnumerator UpdateFPSCounter()
    {
        var waitForDelay = new WaitForSeconds(updateCheckTime);

        while (true)
        {
            int lastFrameCount = Time.frameCount;
            int lastTime = (int) Time.realtimeSinceStartup;

            yield return waitForDelay;

            int timeDelta = (int) Time.realtimeSinceStartup - lastTime;
            int frameDelta = Time.frameCount - lastFrameCount;

            text.text = $" FPS : {frameDelta / timeDelta}";
        }
    }
    #endregion

    #region UnityMethods
    void Awake()
    {
        text = GetComponent<Text>();   
    }

    void Start()
    {
        StartCoroutine(UpdateFPSCounter());
    }
    #endregion
}

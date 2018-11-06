using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

/*
[Script Header] CATA_EnemyLifeBar Version 0.0.1
Created by:
Date: 
Description:

///
[UPDATES]
Update n°:
Updated by:
Date:
Description:
*/

public class TDS_EnemyLifeBar : MonoBehaviour 
{
    #region Fields/Properties
    [SerializeField] TDS_Enemy owner;
    [SerializeField] Image background;
    [SerializeField] Image fillForground;

    float fillingValue = 1;

    public Vector2 ScreenPosition
    {
        get
        {
            if (!owner) return Vector3.zero;
            else return Camera.main.WorldToScreenPoint(owner.transform.position) + (Vector3.up*2); 
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Fill the bar until it's to the filling Value
    /// </summary>
    /// <returns></returns>
    IEnumerator Fill()
    {
        while (fillForground.fillAmount != fillingValue)
        {
            fillForground.fillAmount = Mathf.Lerp(fillForground.fillAmount, fillingValue, Time.deltaTime *2);
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// If there is no owner, destroy the objects
    /// else set new position 
    /// and add a method to the owner "OnHealthModification" event
    /// </summary>
    private void Init()
    {
        if(!owner)
        {
            if(background) Destroy(background.gameObject);
            if(fillForground) Destroy(fillForground.gameObject);
            Destroy(this);
            return; 
        }
        transform.position = Camera.main.WorldToScreenPoint(owner.transform.position); 
        owner.OnHealthModification += ModifyFillAmount;
    }

    /// <summary>
    /// Change the filling value
    /// Call the coroutine to fill the bar
    /// </summary>
    /// <param name="_newValue"></param>
    private void ModifyFillAmount(float _newValue)
    {
        if (!fillForground) return;
        fillingValue = _newValue;
        StartCoroutine(Fill()); 
    }

    /// <summary>
    /// The position of the bar follow the enemy on the screen
    /// </summary>
    void UpdatePosition()
    {
         if (!owner) return;
         transform.position = ScreenPosition; 
    }
    #endregion

    #region UnityMethods
    private void Start()
    {
        Init(); 
    }
    private void Update()
    {
        UpdatePosition(); 

    }

   
    #endregion
}

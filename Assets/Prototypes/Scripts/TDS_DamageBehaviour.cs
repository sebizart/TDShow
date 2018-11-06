using System;
using System.Collections;
using UnityEngine;
using TMPro;

/*
[Script Header] CATA_DamageBehaviour Version 0.0.1
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

public enum DamageType
{
    Cool,
    Great,
    Super,
}

public class TDS_DamageBehaviour : MonoBehaviour 
{
    #region Fields/Properties
    // The text object of this damage behaviour
    [SerializeField] private TextMeshPro text = null;

    // The life time of this object
    [SerializeField] private float lifeTime = 1;
    #endregion

    #region Methods
    // Behaviour of the object : setting its text, color, floating upper and disappear after a certain time
    private IEnumerator Behave(DamageType _type)
    {
        // Show the damage type as text
        text.text = _type.ToString();
        // Changes the color depending on the damage type
        switch (_type)
        {
            case DamageType.Cool:
                text.color = Color.grey;
                break;
            case DamageType.Great:
                text.color = Color.green;
                break;
            case DamageType.Super:
                text.color = Color.cyan;
                break;
            default:
                break;
        }

        float _lifeTimeTimer = lifeTime;

        // Change the alpha of the text and its position at each frame
        while (_lifeTimeTimer > 0)
        {
            transform.position += Vector3.up * 0.001f;
            text.alpha = _lifeTimeTimer / lifeTime;

            _lifeTimeTimer -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        // After all of this, destroys this game object
        Destroy(gameObject);
        yield return null;
    }
    // Behaviour of the object : setting its text, color, floating upper and disappear after a certain time
    private IEnumerator Behave(string _message)
    {
        // Show the message as text
        text.text = _message;
        // Changes the color of this text
        text.color = Color.red;

        float _lifeTimeTimer = lifeTime;

        // Change the alpha of the text and its position at each frame
        while (_lifeTimeTimer > 0)
        {
            transform.position += Vector3.up * 0.001f;
            text.alpha = _lifeTimeTimer / lifeTime;

            _lifeTimeTimer -= Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        // After all of this, destroys this game object
        Destroy(gameObject);
        yield return null;
    }

    /// <summary>
    /// Initializes the damage's behaviour
    /// </summary>
    /// <param name="_type">Type of the damage</param>
    public void Init(DamageType _type)
    {
        StartCoroutine(Behave(_type));
    }
    /// <summary>
    /// Initializes the damage's behaviour
    /// </summary>
    /// <param name="_message">Message to write</param>
    public void Init(string _message)
    {
        StartCoroutine(Behave(_message));
    }
    #endregion
}

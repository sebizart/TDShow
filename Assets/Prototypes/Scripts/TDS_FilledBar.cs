using System.Collections;
using System.Collections.Generic;
using System; 
using UnityEngine;
using UnityEngine.UI; 

public class TDS_FilledBar : MonoBehaviour
{
    public event Func<IEnumerator> OnValueChanged; 

    #region Fields and properties
    [Header("Settings")]
    [SerializeField] Image filledImage;
    [SerializeField, Range(1, 10)] float speed = 1;
    public TDS_Enemy Owner { get; set; }
    private float currentValue = 1; 
    #endregion

    #region Methods
    private void UpdateFilledAmount()
    {
        if (!filledImage || currentValue == filledImage.fillAmount) return;
        filledImage.fillAmount = Mathf.Lerp(filledImage.fillAmount, currentValue, Time.deltaTime); 
    }

    public void TakingDamages()
    {
        currentValue = (float)Owner.Health / Owner.MaxHealth;
    }
    #endregion

    #region Unity Methods
    private void Update()
    {
        UpdateFilledAmount(); 
    }
    #endregion
}

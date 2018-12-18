using System.Collections;
using System.Collections.Generic;
using System; 
using UnityEngine;
using UnityEngine.UI; 


// LINK TO THE CANVAS 
// IDK HOW BUT DO IT FAGGOT

public class TDS_FilledBar : MonoBehaviour
{
    public event Func<IEnumerator> OnValueChanged; 

    #region Fields and properties
    [Header("Settings")]
    [SerializeField] Image filledImage;
    [SerializeField, Range(1, 10)] float speed = 1;
    public TDS_Enemy Owner { get; set; }
    private float currentValue = 1;
    [SerializeField] private int parentID;
    
    #endregion

    #region Methods
    private void UpdateFilledAmount()
    {
        if (!filledImage) return;
        if(filledImage.fillAmount <= 0.01f)
        {
            Destroy(gameObject);
            return; 
        }
        if(currentValue == filledImage.fillAmount) return;
        filledImage.fillAmount = Mathf.Lerp(filledImage.fillAmount, currentValue, Time.deltaTime * speed); 
        
    }

    public void TakingDamages()
    {
        currentValue = Mathf.Clamp((float)Owner.Health / Owner.MaxHealth, 0, 1);
    }

    public void SetCanvas(Canvas _enemyCanvas)
    {
        parentID = _enemyCanvas.GetComponent<PhotonView>().viewID;
        transform.SetParent(_enemyCanvas.transform); 
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        if(!PhotonNetwork.isMasterClient && parentID != 0)
        {
            Transform _parent = PhotonView.Find(parentID).transform;
            transform.SetParent(_parent); 
        }
    }
    private void Update()
    {
        UpdateFilledAmount(); 
    }
    #endregion
}

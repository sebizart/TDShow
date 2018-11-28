using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_MysteryBall : MonoBehaviour
{
    /* MysteryBall :
	*
	* [Write the Description of the Script here !]
	*/

    #region Events

    #endregion

    #region Fields / Accessors

    #endregion

    #region Methods
    #region Unity Methods
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    #endregion
	
    #region Original Methods
	
    #endregion
    #endregion
}

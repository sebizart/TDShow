using Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TDS_Item : PunBehaviour
{
	/* TDS_Item :
	*
	* [Write the Description of the Script here !]
	*/
	
	#region Events
	
    #endregion
	
    #region Fields / Accessors
	
    #endregion
	
    #region Methods
    #region Unity Methods
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
    /// <summary>
    /// Call this when you want to use this item
    /// </summary>
    protected abstract void Use();
    #endregion
    #endregion
}

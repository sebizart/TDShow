using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[Script Header] CATA_IInterractible Version 0.0.1
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

public interface TDS_IInterractible 
{
    #region Fields/Properties

    #endregion

    #region Methods
    /// <summary>
    /// The Interractible Element take an amount of damages
    /// </summary>
    /// <param damages="_damages"></param>
    void TakeDamages(int _damages);

    /// <summary>
    /// The Interractible element is projected and take an amount of damages
    /// </summary>
    /// <param damages="_damages"></param>
    void BeingProjected(int _damages, Transform _playerPosition);
	#endregion
}

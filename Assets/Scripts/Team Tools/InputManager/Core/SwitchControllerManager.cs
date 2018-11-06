using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControllerManager : MonoBehaviour
{
    #region Switch hide N show cursor
    void CursorHide(bool _test)
    {
        if (_test == true && Cursor.visible == true)
        {
            Cursor.visible = false;
            Debug.Log("Disable");
        }
    }

    void CursorShow(bool _test)
    {
        if (_test == true && Cursor.visible == false)
        {
            Cursor.visible = true;
            Debug.Log("Enable");
        }
    }
    #endregion

    #region Input detected
    void XboxController()
    {
        InputsManager.OnADownInputPress += CursorHide;
        InputsManager.OnBDownInputPress += CursorHide;
        InputsManager.OnYDownInputPress += CursorHide;
        InputsManager.OnXDownInputPress += CursorHide;
        InputsManager.OnStartDownInputPress += CursorHide;
        InputsManager.OnBackDownInputPress += CursorHide;
    }

    void KeyboardMouseController()
    {
        //mouse
        InputsManager.OnLeftClickDownInputPress += CursorShow;
        InputsManager.OnRightClickDownInputPress += CursorShow;
        InputsManager.OnWheelClickDownInputPress += CursorShow;
        //kb letres
        InputsManager.OnKBADownInputPress += CursorShow;
        InputsManager.OnKBZDownInputPress += CursorShow;
        InputsManager.OnKBEDownInputPress += CursorShow;
        InputsManager.OnKBRDownInputPress += CursorShow;
        InputsManager.OnKBTDownInputPress += CursorShow;
        InputsManager.OnKBYDownInputPress += CursorShow;
        InputsManager.OnKBUDownInputPress += CursorShow;
        InputsManager.OnKBIDownInputPress += CursorShow;
        InputsManager.OnKBODownInputPress += CursorShow;
        InputsManager.OnKBPDownInputPress += CursorShow;
        InputsManager.OnKBQDownInputPress += CursorShow;
        InputsManager.OnKBSDownInputPress += CursorShow;
        InputsManager.OnKBDDownInputPress += CursorShow;
        InputsManager.OnKBFDownInputPress += CursorShow;
        InputsManager.OnKBGDownInputPress += CursorShow;
        InputsManager.OnKBHDownInputPress += CursorShow;
        InputsManager.OnKBJDownInputPress += CursorShow;
        InputsManager.OnKBKDownInputPress += CursorShow;
        InputsManager.OnKBLDownInputPress += CursorShow;
        InputsManager.OnKBMDownInputPress += CursorShow;
        InputsManager.OnKBWDownInputPress += CursorShow;
        InputsManager.OnKBXDownInputPress += CursorShow;
        InputsManager.OnKBCDownInputPress += CursorShow;
        InputsManager.OnKBVDownInputPress += CursorShow;
        InputsManager.OnKBBDownInputPress += CursorShow;
        InputsManager.OnKBNDownInputPress += CursorShow;
        //directional arrow
        InputsManager.OnLeftArrowDownInputPress += CursorShow;
        InputsManager.OnRightArrowDownInputPress += CursorShow;
        InputsManager.OnUpArrowDownInputPress += CursorShow;
        InputsManager.OnDownArrowDownInputPress += CursorShow;    
        //alphanumerique
        InputsManager.OnKBAOneDownInputPress += CursorShow;
        InputsManager.OnKBATwoDownInputPress += CursorShow;
        InputsManager.OnKBAThreeDownInputPress += CursorShow;
        InputsManager.OnKBAFourDownInputPress += CursorShow;
        InputsManager.OnKBAFiveDownInputPress += CursorShow;
        InputsManager.OnKBASixDownInputPress += CursorShow;
        InputsManager.OnKBASevenDownInputPress += CursorShow;
        InputsManager.OnKBAEightDownInputPress += CursorShow;
        InputsManager.OnKBANineDownInputPress += CursorShow;
        InputsManager.OnKBAZeroDownInputPress += CursorShow;
        //keypad
        InputsManager.OnKPAOneDownInputPress += CursorShow;
        InputsManager.OnKPATwoDownInputPress += CursorShow;
        InputsManager.OnKPAThreeDownInputPress += CursorShow;
        InputsManager.OnKPAFourDownInputPress += CursorShow;
        InputsManager.OnKPAFiveDownInputPress += CursorShow;
        InputsManager.OnKPASixDownInputPress += CursorShow;
        InputsManager.OnKPASevenDownInputPress += CursorShow;
        InputsManager.OnKPAEightDownInputPress += CursorShow;
        InputsManager.OnKPANineDownInputPress += CursorShow;
        InputsManager.OnKPAZeroDownInputPress += CursorShow;
        //Special keys
        InputsManager.OnEscapeClickDownInputPress += CursorShow;
    }
    #endregion

    #region UniMeth
    private void Awake()
    {
        KeyboardMouseController();
        XboxController();
    }
    #endregion
}
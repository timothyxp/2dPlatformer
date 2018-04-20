using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenuController : MonoBehaviour {
    private bool isMenu = false;

    public bool GetIsMenu()
    {
        return isMenu;
    }

    public void SetIsMenu(bool d)
    {
        isMenu = d;
    }
}

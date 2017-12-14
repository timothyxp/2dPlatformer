using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
    [SerializeField]
    public bool Enter;
    [SerializeField]
    public bool Exit;
    [SerializeField]
    public bool autor;
    [SerializeField]
    public bool settings;
    [SerializeField]
    private bool level1 = false;
    [SerializeField]
    private bool level2 = false;
    [SerializeField]
    private bool level3 = false;
    [SerializeField]
    private Camera Camera1;
    [SerializeField]
    private Camera Camera2;
    [SerializeField]
    private Camera Camera3;
    [SerializeField]
    private Camera Camera4;
    [SerializeField]
    private bool Low = false;
    [SerializeField]
    private bool Medium = false;
    [SerializeField]
    private bool High = false;
    [SerializeField]
    private bool Fantastic = false;
    [SerializeField]
    public bool BackMenu = false;
    [SerializeField]
    private bool BackMenu1=false;
    [SerializeField]
    private bool BackMenu2 = false;

    void OnMouseEnter()
    {
        GetComponent<Renderer>().material.color = Color.cyan;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material.color = Color.white; 
    }

    void OnMouseUp()
    {
        if (Enter)
        {
            Camera1.enabled = false;
            Camera4.enabled = true;
        }
        if (BackMenu2)
        {
            Camera1.enabled = true;
            Camera4.enabled = false;
        }
        if (level1)
        {
            Application.LoadLevel(1);
        }
        if (level2)
        {
            Application.LoadLevel(2);
        }
        if (level3)
        {
            Application.LoadLevel(3);
        }
        if (settings)
        {
            Camera1.enabled = false;
            Camera2.enabled = true;
        }
        if (BackMenu)
        {
            Camera1.enabled = true;
            Camera2.enabled = false;
        }
        if (autor)
        {
            Camera3.enabled = true;
            Camera1.enabled = false;
        }
        if (BackMenu1)
        {
            Camera1.enabled = true;
            Camera3.enabled = false;
        }
        if (Low)
        {
            QualitySettings.currentLevel = QualityLevel.Fast;
        }
        if (Medium)
        {
            QualitySettings.currentLevel = QualityLevel.Good;
        }
        if (High)
        {
            QualitySettings.currentLevel = QualityLevel.Beautiful;
        }
        if (Fantastic) 
        {
            QualitySettings.currentLevel = QualityLevel.Fantastic;
        }
        if (Exit)
        {
            Application.Quit();
        }
    }
}

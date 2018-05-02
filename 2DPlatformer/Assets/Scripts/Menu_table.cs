using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_table : MonoBehaviour {
    private bool created = false;
    [SerializeField]
    public MainMenu menu;
    [SerializeField]
    public Settings settings;
    [SerializeField]
    private int dif;
    [SerializeField]
    public InterfaceControl Control;
    Character character;
    private bool isjump = false;
    private int move = 0;
    private bool openmenu = false;
    [SerializeField]
    private bool isAndroid = false;

    public void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
    }

    public void OnLevelWasLoaded()
    {
        character = FindObjectOfType<Character>();
        if (character && isAndroid)
        {
            Control.Activate(true);
        }
        else
        {
            Control.Activate(false);
        }
    }

    public void setJump()
    {
        isjump = true;
    }

    public bool getJump()
    {
        bool copy = isjump;
        isjump = false;
        return copy;
    }

    public void setMove(int r)
    {
        move = r;
    }

    public int getMove()
    {
        return move;
    }

    public void setMenu()
    {
        openmenu = true;
    }

    public bool getMenu()
    {
        bool copy = openmenu;
        openmenu = false;
        return copy;
    }

    public void SetDifficult(int d)
    {
        dif = d;
    }

    public int GetDifficult()
    {
        return dif;
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void SetAct(bool q)
    {
        if (q)
        {
            menu.SetAct(true);
            settings.SetAct(false);
            Control.SetAct(false);
        }
        else
        {
            menu.SetAct(false);
            settings.SetAct(false);
            Control.SetAct(true);
        }
    }
}

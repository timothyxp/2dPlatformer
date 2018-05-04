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
    public bool isAndroid = false;
    [SerializeField]
    LoadMenuControl load;

    public void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
        load = FindObjectOfType<LoadMenuControl>();
    }

    public void OnLevelWasLoaded()
    {
        load = FindObjectOfType<LoadMenuControl>();
        isjump = false;
        openmenu = false;
        move = 0;
        character = FindObjectOfType<Character>();
        if (character && isAndroid)
        {
            Control.SetAct(true);
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

    public void Play()
    {
        load.SetLevel(1);
        load.loadlevel();
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

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

    public void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }

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
        }
        else
        {
            menu.SetAct(false);
            settings.SetAct(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Portal : MonoBehaviour {
    [SerializeField]
    public int level = 0;
    [SerializeField]
    LoadMenuControl load;

    public void Awake()
    {
        load = FindObjectOfType<LoadMenuControl>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        int r = PlayerPrefs.GetInt("level");
        if (r < level)
        {
            PlayerPrefs.SetInt("level", level);
        }
        Unit unit = collider.GetComponent<Unit>();
        load.SetLevel(level);
        load.loadlevel();
    }
}

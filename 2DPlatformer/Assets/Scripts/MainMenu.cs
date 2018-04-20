using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public void PlayPressed()
    {
        gameObject.SetActive(false);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    
    public void SetAct(bool q)
    {
        gameObject.SetActive(q);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
    
}

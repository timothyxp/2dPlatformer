using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadMenuControl : MonoBehaviour {
    private int PercentOfLoad;
    private bool loaded = false;
    [SerializeField]
    GUIContent load;
    private float timer = 0;
    [SerializeField]
    private int level;
    [SerializeField]
    public Image img;
    [SerializeField]
    public Text text;

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1 && !loaded)
        {
            StartCoroutine("LoadLevel");
            loaded = true;
        }
    }

    public IEnumerator LoadLevel()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(level);
        while (!async.isDone) 
        {
            PercentOfLoad = Mathf.RoundToInt((async.progress/0.9f)*100f);
            img.fillAmount = PercentOfLoad;
            text.text = PercentOfLoad.ToString()+"%";
            Debug.Log("Progress complete:" + PercentOfLoad.ToString()+"%");
            yield return true;
        }
    }
}

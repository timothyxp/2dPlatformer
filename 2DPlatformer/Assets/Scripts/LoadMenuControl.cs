using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMenuControl : MonoBehaviour {
    private int PercentOfLoad;
    private bool loaded = false;
    [SerializeField]
    GUIContent load;
    private float timer = 0;
    [SerializeField]
    private int level;

    public void Awake()
    {
        StartCoroutine("Wait");
    }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer > 3 && !loaded)
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
            PercentOfLoad = Mathf.RoundToInt(async.progress*100f);
            Debug.Log("Progress complete:" + PercentOfLoad.ToString()+"%");
            GUI.Box(new Rect((Screen.width - 1000) / 2, (Screen.height - 100) / 2, PercentOfLoad * 10, 100), load);
            yield return new WaitForSeconds(3);
        }
    }

    public IEnumerator Wait()
    {
        Debug.Log(Time.time);
        yield return new WaitForSeconds(3);
        Debug.Log(Time.time);
    }

}

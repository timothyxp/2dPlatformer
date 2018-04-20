using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Portal : MonoBehaviour {
    [SerializeField]
    private bool level1 = false;
    [SerializeField]
    private bool level2 = false;
    [SerializeField]
    private bool level3 = false;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Unit unit = collider.GetComponent<Unit>();

        if(unit && unit is Character)
        {
            if (level1)
            {
                SceneManager.LoadScene(2);
            }
            if (level2)
            {
                SceneManager.LoadScene(3);
            }
            if (level3)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour
{
    [SerializeField]
    public int diff;
    [SerializeField]
    Character charac;

    public void Awake()
    {
        if (!charac)
        {
            Debug.Log("dont find character");
        }
        int NowDifficult = charac.GetDifficult();
        if(NowDifficult <= diff)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Character character = collider.GetComponent<Character>();
        
        if (character)
        {
            character.Lives++;
            Destroy(gameObject);
        }
    }
}

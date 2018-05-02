using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Butoon : MonoBehaviour{
	public void SetAct(bool q)
    {
        gameObject.SetActive(q);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboPopup : MonoBehaviour
{
    public float popupLifespan = 0.8f;

    void Start()
    {
        Destroy(gameObject, popupLifespan);
    }

}

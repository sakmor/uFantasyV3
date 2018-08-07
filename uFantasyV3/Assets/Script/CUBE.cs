using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] //fixme:不知道會不會讓編輯器變超慢
public class CUBE : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
        {
            transform.position = new Vector3(
                Mathf.Round(transform.position.x),
                Mathf.Round(transform.position.y),
                Mathf.Round(transform.position.z));
        }
    }
}

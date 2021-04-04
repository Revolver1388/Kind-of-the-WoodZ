using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightCandle : MonoBehaviour
{
    private readonly Vector3 _offset = new Vector3(0,1.5f);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GameManager.Instance.GetPlayerTransform().position + _offset;
    }
}

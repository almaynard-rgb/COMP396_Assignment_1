using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    public GameObject cloud;
    public Transform camera;
    public float speed;


    // Update is called once per frame
    void Update()
    {
        cloud.transform.Translate(new Vector2((speed * Time.deltaTime), 0));

        if (cloud.transform.position.x >= 90)
            cloud.transform.position = new Vector2(-90, (Random.Range(20.0f, 50.0f)));
    }
}

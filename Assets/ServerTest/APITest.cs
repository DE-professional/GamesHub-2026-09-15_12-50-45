using System;
using UnityEngine;
public class APITest:MonoBehaviour
{
    private GameInfoClient client;
    private void Start()
    {
        client = gameObject.AddComponent<GameInfoClient>();
        //client.StartCoroutine("Start");
    }
}

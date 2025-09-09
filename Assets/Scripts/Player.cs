using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer && isClient)
        {
            SceneManager.LoadSceneAsync(1);
        }
        transform.position = new Vector3(netId * 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

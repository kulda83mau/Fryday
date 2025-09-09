using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
        transform.position = new Vector3(netId * 2, 0);
        if (!isLocalPlayer) Destroy(GetComponent<TwoDimensionalAnimationStateController>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

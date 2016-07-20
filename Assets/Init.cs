using UnityEngine;
using System.Collections;

public class Init : MonoBehaviour {

    [SerializeField]
    private Move localPlayer;
    [SerializeField]
    private Move remotePlayer;
     
	// Use this for initialization
	void Start () {
       

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void startCycles()
    {
        localPlayer.startMoving();
        remotePlayer.startMoving();
    }


}

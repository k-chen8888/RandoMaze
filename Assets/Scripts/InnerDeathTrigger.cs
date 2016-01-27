using UnityEngine;
using System.Collections;

public class InnerDeathTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter(Collider otherObject)
    {
        // Kill me
        DeathPause.S.PauseDead();
    }
}

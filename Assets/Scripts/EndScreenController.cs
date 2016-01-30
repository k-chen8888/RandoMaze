using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EndScreenController : MonoBehaviour {

    /* The first level */
    public string levelOne = "Maze0";


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown("space"))
        {
            SceneManager.LoadScene(levelOne);
        }
	}
}

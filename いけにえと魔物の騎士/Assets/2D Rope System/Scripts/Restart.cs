using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {

	//when mouse is clicked on this object reload current scene
	void OnMouseDown()
	{
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//if touchcount is more than 2 reload current scene
		if(Input.touchCount > 2)
            SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
	}
}

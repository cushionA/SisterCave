using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
	
	void OnGUI() {
		if (GUI.Button(new Rect(Screen.width - 100, 10, 70, 30), "Scene 1"))
            SceneManager.LoadScene (0);

		if (GUI.Button(new Rect(Screen.width - 100, 50, 70, 30), "Scene 2"))
            SceneManager.LoadScene (1);

		if (GUI.Button(new Rect(Screen.width - 100, 90, 70, 30), "Scene 3"))
            SceneManager.LoadScene (2);
	}
}

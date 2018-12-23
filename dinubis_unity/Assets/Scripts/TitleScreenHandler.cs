using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenHandler : MonoBehaviour {

  public void StartButtonClick()
  {
    SceneManager.LoadScene("CW_Scene");
  }

  public void CreditsButtonClick()
  {
    // TODO
    SceneManager.LoadScene("AG_Scene");
  }

  public void QuitButtonClick()
  {
    #if UNITY_EDITOR
      UnityEditor.EditorApplication.isPlaying = false;
    #else
      Application.Quit();
    #endif
  }
}

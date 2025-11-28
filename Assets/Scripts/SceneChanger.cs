using UnityEngine;
using UnityEngine.SceneManagement;

    public class SceneChanger : MonoBehaviour
    {
        public void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
        public void RestartScene()
        {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
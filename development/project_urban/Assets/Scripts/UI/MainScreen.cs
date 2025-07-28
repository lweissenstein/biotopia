using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class MainScreen : MonoBehaviour
{
    public UIDocument uiDocument;
    public UIDocument loadingScreen;
    private VisualElement background;
    private ProgressBar loadingBar;
    private Label title;
    private Button playtutorial, play, quit, f4f, igz;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = uiDocument.rootVisualElement;

        background = root.Q<VisualElement>("background");
        title = root.Q<Label>("title");
        playtutorial = root.Q<Button>("playtutorial");
        play = root.Q<Button>("play");
        quit = root.Q<Button>("quit");
        f4f = root.Q<Button>("f4f");
        igz = root.Q<Button>("igz");

        title.enableRichText = true;
        title.text = "<color=#379B1B>Bio</color>topia";

        playtutorial.clicked += () =>
        {
            GameState.isNewGame = true;
            LoadSceneWithProgress("tutorial");
        };
        play.clicked += () =>
        {
            GameState.isNewGame = true;
            LoadSceneWithProgress("SampleScene");
        };
        quit.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        };
        f4f.clicked += () =>
        {
            Application.OpenURL("https://www.food4future.de/de/home");
        };
        igz.clicked += () =>
        {
            Application.OpenURL("https://igzev.de/");
        };

        var loadingRoot = loadingScreen.rootVisualElement;
        loadingBar = loadingRoot.Q<ProgressBar>("loading");
        loadingScreen.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void LoadSceneWithProgress(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingScreen.rootVisualElement.style.display = DisplayStyle.Flex;
        loadingBar.value = 0;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f); // Bis max. 0.9
            loadingBar.value = progress * 100f;

            // Optional: Sofort aktivieren
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}

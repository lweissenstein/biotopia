using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class MainScreen : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement background;
    private Label title;
    private Button playtutorial, play, quit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var root = uiDocument.rootVisualElement;

        background = root.Q<VisualElement>("background");
        title = root.Q<Label>("title");
        playtutorial = root.Q<Button>("playtutorial");
        play = root.Q<Button>("play");
        quit = root.Q<Button>("quit");

        title.enableRichText = true;
        title.text = "<color=#379B1B>Bio</color>topia";

        playtutorial.clicked += () =>
        {
            GameState.isNewGame = true;
            SceneManager.LoadSceneAsync("tutorial");
        }; 
        play.clicked += () =>
        {
            GameState.isNewGame = true;
            SceneManager.LoadSceneAsync("SampleScene");
        };
        quit.clicked += () =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        };


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

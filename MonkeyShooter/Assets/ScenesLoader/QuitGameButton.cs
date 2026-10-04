using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class QuitGameButton : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(QuitGame);
    }

    private void QuitGame()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
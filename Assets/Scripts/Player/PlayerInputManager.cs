using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public void HandleRestartScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    
    public void HandleFirstScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene(0);
        }
    }
    
    public void HandleNextDialogueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DialogueManager.Instance.HandleNextDialogueInput();
        }
    }
}
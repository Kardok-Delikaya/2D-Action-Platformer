using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    PlayerManager player;

    public static DialogueManager Instance;

    [SerializeField] GameObject dialoguePanel;
    [SerializeField] Image playerIcon;
    [SerializeField] Image npcIcon;
    [SerializeField] GameObject playerNameParentObject;
    [SerializeField] GameObject npcParentObject;
    [SerializeField] TMP_Text npcName;
    [SerializeField] TMP_Text dialogueArea;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    public bool isDialogueActive = false;
    bool isTypingDialogue = false;
    [SerializeField] float typingSpeed = .02f;

    string currentTalkingCharacter;
    DialogueLine currentLine;

    void Start()
    {
        player=FindAnyObjectByType<PlayerManager>();

        if (Instance == null)
            Instance = this;
    }

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        player.isInteracting=true;
        dialoguePanel.SetActive(true);

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentLine = lines.Dequeue();

        if (currentLine.character.isPlayer)
        {
            playerIcon.enabled = true;
            playerNameParentObject.SetActive(true);
            npcParentObject.SetActive(false);
        }
        else
        {
            npcIcon.enabled = true;
            playerNameParentObject.SetActive(false);
            npcParentObject.SetActive(true);

            if (currentLine.character.icon != null)
            {
                npcIcon.sprite = currentLine.character.icon;
            }
            else
            {
                npcIcon.enabled = false;
            }
        }

        npcName.text = currentLine.character.name;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        isTypingDialogue = true;
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            yield return new WaitForSeconds(typingSpeed);
            dialogueArea.text += letter;
        }

        isTypingDialogue = false;
        yield return new WaitForSeconds(5f);

        DisplayNextDialogueLine();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        npcIcon.enabled = false;
        playerIcon.enabled = false;
        playerNameParentObject.SetActive(false);
        npcParentObject.SetActive(false);
        dialoguePanel.SetActive(false);
        player.isInteracting = false;
    }

    public void HandleNextDialogueInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isTypingDialogue)
            {
                StopAllCoroutines();
                dialogueArea.text = currentLine.line;
                isTypingDialogue = false;
            }
            else
            {
                DisplayNextDialogueLine();
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private PlayerManager player;

    public static DialogueManager Instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image npcIcon;
    [SerializeField] private GameObject playerNameParentObject;
    [SerializeField] private GameObject npcParentObject;
    [SerializeField] private TMP_Text npcName;
    [SerializeField] private TMP_Text dialogueArea;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();

    public bool isDialogueActive = false;
    private bool isTypingDialogue = false;
    [SerializeField] private float typingSpeed = .02f;

    private string currentTalkingCharacter;
    private DialogueLine currentLine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        player=FindAnyObjectByType<PlayerManager>();
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

    private IEnumerator TypeSentence(DialogueLine dialogueLine)
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

    private void EndDialogue()
    {
        isDialogueActive = false;
        npcIcon.enabled = false;
        playerIcon.enabled = false;
        playerNameParentObject.SetActive(false);
        npcParentObject.SetActive(false);
        dialoguePanel.SetActive(false);
        player.isInteracting = false;
    }

    public void HandleNextDialogueInput()
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
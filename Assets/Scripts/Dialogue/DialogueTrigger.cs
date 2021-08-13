using System;
using System.Collections;
using System.Collections.Generic;
using DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject dialoguePrefab;
    [SerializeField] private EntityWithDialogue entityWithDialogue;
    [SerializeField] private Button triggerButton;

    private void OnEnable()
    {
        triggerButton.onClick.AddListener(StartDialogue);
    }

    private void OnDisable()
    {
        triggerButton.onClick.RemoveListener(StartDialogue);
    }

    private void StartDialogue()
    {
        triggerButton.interactable = false;
        
        var prefabInstance = Instantiate(dialoguePrefab, uiCanvas.transform, false);

        var dialogueUI = prefabInstance.GetComponent<Dialogue>();
        
        dialogueUI.OptionChosen += DialogueManager.Instance.ChooseAndContinue;
        dialogueUI.ContinueTriggered += DialogueManager.Instance.Continue;

        void OnContinue(DialogueData dialogueData)
        {
            dialogueUI.AdvanceDialogue(dialogueData.Text, dialogueData.CharacterName, dialogueData.Choices);
        }
        void OnEnd()
        {
            dialogueUI.EndDialogue();

            DialogueManager.Instance.Continued -= OnContinue;
            DialogueManager.Instance.Ended -= OnEnd;

            triggerButton.interactable = true;
        }

        DialogueManager.Instance.Continued += OnContinue;
        DialogueManager.Instance.Ended += OnEnd;

        DialogueManager.Instance.StartDialogue(
            new InkDialogueHandler(entityWithDialogue.Story));
    }
}

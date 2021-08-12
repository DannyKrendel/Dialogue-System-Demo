using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private GameObject dialoguePrefab;

    private IDialogueHandler _dialogueHandler;
    private Dialogue _dialogue;
    private bool _dialogueStarted;

    #region Singleton stuff

    private static DialogueManager _instance;

    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<DialogueManager>();
            return _instance;
        }
    }    

    #endregion

    public void StartDialogue(IDialogueHandler dialogueHandler)
    {
        if (_dialogueStarted) return;
        
        _dialogueHandler = dialogueHandler;
        
        if (!_dialogueHandler.CanContinue)
        {
            Debug.LogError("Dialog can't be continued");
            return;
        }
        
        var prefabInstance = Instantiate(dialoguePrefab, uiCanvas.transform, false);

        _dialogue = prefabInstance.GetComponent<Dialogue>();
        _dialogue.OptionChosen += ChooseAndAdvance;
        _dialogue.ContinueTriggered += OnContinue;

        _dialogueStarted = true;

        AdvanceDialogue();
    }

    private void OnContinue()
    {
        if (!_dialogueStarted) return;

        if (_dialogueHandler.CanContinue)
        {
            AdvanceDialogue();
        }
        else if (!_dialogueHandler.CanContinue && !_dialogueHandler.HasChoices)
        {
            _dialogue.EndDialogue();
            _dialogueHandler.ResetState();
            _dialogueStarted = false;
        }
    }

    private void ChooseAndAdvance(int index)
    {
        _dialogueHandler.Choose(index);
        if (_dialogueHandler.CanContinue)
        {
            AdvanceDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        string currentText = _dialogueHandler.Continue();
        string[] choices = null;

        if (_dialogueHandler.HasChoices) choices = _dialogueHandler.GetCurrentChoices().ToArray();
        
        _dialogue.AdvanceDialogue(currentText, _dialogueHandler.CurrentCharacterName, choices);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using DialogueSystem;
using Ink.Runtime;
using UnityEngine;

public class InkDialogueHandler : IDialogueHandler
{
    public bool CanContinue => _story.canContinue;
    public string CurrentCharacterName { get; private set; }
    public string CurrentText => _story.currentText;
    public bool HasChoices => _story.currentChoices.Count > 0;
    public int CurrentChoicesCount => _story.currentChoices.Count;

    private Story _story;
    private bool characterChanged;
    
    public InkDialogueHandler(Story story)
    {
        _story = story;
    }

    public IReadOnlyCollection<string> GetCurrentChoices()
    {
        return _story.currentChoices.Select(c => c.text).ToList();
    }

    public string Continue()
    {
        string text = "";

        while (_story.canContinue && _story.currentChoices.Count == 0)
        {
            text += _story.Continue();
            ProcessTags();
            if (characterChanged)
            {
                characterChanged = false;
                break;
            }
        }

        return text;
    }

    public void Choose(int choiceIndex)
    {
        _story.ChooseChoiceIndex(choiceIndex);
    }

    public void ResetState()
    {
        _story.ResetState();
    }

    private void ProcessTags()
    {
        var tags = _story.currentTags;
        foreach (var tag in tags)
        {
            var splitString = tag.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
            if (splitString.Length != 2)
            {
                Debug.LogError($"Couldn't parse tag: {tag}");
                continue;
            }

            var key = splitString[0].Trim();
            var value = splitString[1].Trim();

            switch (key)
            {
                case "character":
                    CurrentCharacterName = value;
                    characterChanged = true;
                    break;
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueTextAnimator
{
    public bool IsPlaying { get; private set; }
    public bool IsComplete { get; private set; }
    public event Action<char> CharacterAppeared;

    private readonly TMP_Text _targetText;
    private readonly WaitForSecondsRealtime _waitCoroutine;
    private Action _onComplete;
    private string _destinationText;
    private Coroutine _animationCoroutine;
    private DialogueText _currentDialogueText;

    public DialogueTextAnimator(TMP_Text targetText)
    {
        _targetText = targetText;
        _waitCoroutine = new WaitForSecondsRealtime(0);
    }

    public DialogueTextAnimator Play(string text, float speed)
    {
        _destinationText = text;
        _animationCoroutine = _targetText.StartCoroutine(PlayCoroutine(text, speed));
        return this;
    }

    public DialogueTextAnimator OnComplete(Action callback)
    {
        _onComplete = callback;
        return this;
    }

    public void Complete()
    {
        if (IsComplete) return;

        _targetText.StopCoroutine(_animationCoroutine);
        CompleteImmediate();
        OnAnimationEnd();
    }

    private void CompleteImmediate()
    {
        foreach (var taggedText in _currentDialogueText.TextFragments)
        {
            _targetText.text = taggedText.Text;
        }
    }

    private IEnumerator PlayCoroutine(string text, float speed)
    {
        _waitCoroutine.waitTime = speed;
        yield return null;

        OnAnimationStart();
        
        _currentDialogueText = new DialogueText(text);

        foreach (var taggedText in _currentDialogueText.TextFragments)
        {
            yield return ProcessTaggedText(taggedText);
        }
        
        OnAnimationEnd();
    }

    private IEnumerator ProcessTaggedText(TaggedText taggedText)
    {
        if (!taggedText.HasTags)
        {
            yield return ProcessCharacters(taggedText.Text);

            yield break;
        }

        var oldWaitTime = _waitCoroutine.waitTime;
        
        foreach (var tag in taggedText.Tags)
        {
            switch (tag)
            {
                case "slow":
                    _waitCoroutine.waitTime *= 5;
                    yield return ProcessCharacters(taggedText.Text);

                    break;
                default:
                    yield return ProcessCharacters(taggedText.RawText);

                    break;
            }
        }

        _waitCoroutine.waitTime = oldWaitTime;
    }

    private IEnumerator ProcessCharacters(IEnumerable<char> characters)
    {
        foreach (var ch in characters)
        {
            _targetText.text += ch;
            CharacterAppeared?.Invoke(ch);
            yield return _waitCoroutine;
        }
    }

    private void OnAnimationStart()
    {
        IsPlaying = true;
        IsComplete = false;
        _targetText.text = "";
    }

    private void OnAnimationEnd()
    {
        IsPlaying = false;
        IsComplete = true;
        _currentDialogueText = null;
        _destinationText = "";
        _onComplete?.Invoke();
    }
}

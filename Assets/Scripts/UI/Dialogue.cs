using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup choicesContainerCanvasGroup;
    [Range(.1f, 1)] [SerializeField] private float fadeDuration;

    private readonly List<DialogueChoiceButton> _choiceButtons = new List<DialogueChoiceButton>();
    private Sequence _choiceSequence;

    public event Action ContinueTriggered;
    public event Action<int> OptionChosen;

    private void OnEnable()
    {
        continueButton.onClick.AddListener(() => ContinueTriggered?.Invoke());
        continueButton.transform
            .DOMoveY(continueButton.transform.position.y - 20, .5f)
            .SetLoops(-1, LoopType.Yoyo);
        Show();
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.transform.DOComplete();
    }

    public void AdvanceDialogue(string text, string name, string[] choices = null)
    {
        HideContinueButton();

        // TODO: play animations here

        dialogueText.text = text;
        characterName.text = name;
        characterAvatar.sprite = AssetUtils.FindAndLoadAsset<Sprite>(
            $"{name} t:sprite", new[] {"Assets/Sprites/Avatars"});

        if (choices == null)
        {
            if (_choiceButtons.Count > 0) HideChoices(ClearChoices);
            ShowContinueButton();
            return;
        }

        if (_choiceButtons.Count > 0)
        {
            HideChoices(() =>
            {
                ClearChoices();
                CreateChoices(choices);
                ShowChoices();
            });
        }
        else
        {
            CreateChoices(choices);
            ShowChoices();
        }
    }

    public void EndDialogue()
    {
        Hide(() => Destroy(gameObject));
    }

    private void Show(Action onComplete = null)
    {
        canvasGroup.DOFade(1, fadeDuration).From(0).OnComplete(() => onComplete?.Invoke());
    }

    private void Hide(Action onComplete = null)
    {
        canvasGroup.DOFade(0, fadeDuration).From(1).OnComplete(() => onComplete?.Invoke());;
    }

    private void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);
    }

    private void HideContinueButton()
    {
        continueButton.gameObject.SetActive(false);
    }

    private void ShowChoices()
    {
        var position = choicesContainer.transform.position;
        var start = position.y - 50;
        var end = position.y;
        _choiceSequence = DOTween.Sequence()
            .Append(choicesContainerCanvasGroup.DOFade(1, .3f).From(0))
            .Join(choicesContainer.transform
                .DOMoveY(end, .3f)
                .From(start));
    }

    private void HideChoices(Action onComplete = null)
    {
        var position = choicesContainer.transform.position;
        _choiceSequence = DOTween.Sequence()
            .Append(choicesContainerCanvasGroup.DOFade(0, .3f).From(1))
            .Join(choicesContainer.transform.DOMoveY(-50, .3f).SetRelative())
            .AppendCallback(() => choicesContainer.transform.position = position)
            .OnComplete(() => onComplete?.Invoke());
    }

    private void CreateChoices(string[] choices)
    {
        for (var i = 0; i < choices.Length; i++)
        {
            var choiceText = choices[i];
            
            var choiceObject = Instantiate(choiceButtonPrefab, choicesContainer.transform, false);

            var choiceButton = choiceObject.GetComponent<DialogueChoiceButton>();
            choiceButton.Initialize(i, choiceText, OptionChosen);

            _choiceButtons.Add(choiceButton);
        }
    }

    private void ClearChoices()
    {
        foreach (var button in _choiceButtons)
        {
            Destroy(button.gameObject);
        }
        _choiceButtons.Clear();
    }
}

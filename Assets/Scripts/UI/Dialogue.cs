using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject rootObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterName;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup choicesContainerCanvasGroup;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject choiceButtonPrefab;

    [Header("Animation")]
    [Range(.1f, 1), SerializeField] private float fadeDuration = .5f;
    [Range(0, 1), SerializeField] private float typingAnimationSpeed = .05f;
    [Range(0, .5f), SerializeField] private float showChoicesDuration = .3f;
    [Range(0, .5f), SerializeField] private float hideChoicesDuration = .3f;

    private readonly List<DialogueChoiceButton> _choiceButtons = new List<DialogueChoiceButton>();
    
    private Sequence _choiceShowSequence;
    private Sequence _choiceHideSequence;
    private Tween _continueButtonLoopTween;
    private DialogueTextAnimator _dialogueTextAnimator;

    public event Action ContinueTriggered;
    public event Action<int> OptionChosen;

    private void Awake()
    {
        _dialogueTextAnimator = new DialogueTextAnimator(dialogueText);
        
        var position = choicesContainer.transform.position;
        var start = position.y - 50;
        var end = position.y;
        _choiceShowSequence = DOTween.Sequence()
            .Append(choicesContainerCanvasGroup.DOFade(1, showChoicesDuration).From(0).SetEase(Ease.OutSine))
            .Join(choicesContainer.transform.DOMoveY(end, showChoicesDuration).From(start).SetEase(Ease.OutSine))
            .SetAutoKill(false)
            .Pause();
        
        _choiceHideSequence = DOTween.Sequence()
            .Append(choicesContainerCanvasGroup.DOFade(0, hideChoicesDuration).From(1).SetEase(Ease.InSine))
            .Join(choicesContainer.transform.DOMoveY(-50, hideChoicesDuration).SetRelative().SetEase(Ease.InSine))
            .AppendCallback(() => choicesContainer.transform.position = position)
            .SetAutoKill(false)
            .Pause();

        _continueButtonLoopTween = continueButton.transform
            .DOMoveY(continueButton.transform.position.y - 20, .5f)
            .SetLoops(-1, LoopType.Yoyo)
            .Pause();
    }

    private void OnEnable()
    {
        // continueButton.onClick.AddListener(OnContinuePressed);

        _continueButtonLoopTween.Play();
        Show();
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.transform.DOComplete();
    }

    private void Update()
    {
        if (!IsRootClicked()) return;
        
        if (_dialogueTextAnimator.IsPlaying)
            _dialogueTextAnimator.Complete();
        else
            OnContinuePressed();
    }

    public void AdvanceDialogue(string text, string name, string[] choices = null)
    {
        HideContinueButton();
        
        characterName.text = name;
        characterAvatar.sprite = AssetUtils.FindAndLoadAsset<Sprite>(
            $"{name} t:sprite", new[] {"Assets/Sprites/Avatars"});

        if (_choiceButtons.Count > 0) HideChoices();

        _dialogueTextAnimator
            .Play(text, typingAnimationSpeed)
            .OnComplete(() =>
            {
                ClearChoices();
                var c = choices;
                if (choices == null)
                {
                    ShowContinueButton();
                    return;
                }
                
                CreateChoices(choices);
                ShowChoices();
            });
    }

    public void EndDialogue()
    {
        Hide(() => Destroy(gameObject));
    }

    private void OnContinuePressed()
    {
        ContinueTriggered?.Invoke();
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
        continueButton.interactable = true;
        continueButton.gameObject.SetActive(true);
    }

    private void HideContinueButton()
    {
        continueButton.interactable = false;
        continueButton.gameObject.SetActive(false);
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
    
    private void ShowChoices()
    {
        foreach (var button in _choiceButtons)
        {
            button.Interactable = true;
        }

        _choiceShowSequence.Restart();
        _choiceShowSequence.Play();
    }

    private void HideChoices()
    {
        foreach (var button in _choiceButtons)
        {
            button.Interactable = false;
        }

        _choiceHideSequence.Restart();
        _choiceHideSequence.Play();
    }

    private bool IsRootClicked()
    {
        return Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(
            rootObject.GetComponent<RectTransform>(),
            Input.mousePosition,
            null);
    }
}

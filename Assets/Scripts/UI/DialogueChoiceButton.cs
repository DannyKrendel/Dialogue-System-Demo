using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text choiceText;
    
    public int Id { get; private set; }
    public string Text => choiceText.text;
    public bool Interactable
    {
        get => button.interactable;
        set => button.interactable = value;
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    public void Initialize(int id, string text, Action<int> clickAction)
    {
        Id = id;
        choiceText.text = text;
        button.onClick.AddListener(() => clickAction?.Invoke(id));
    }
}

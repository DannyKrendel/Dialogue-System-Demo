using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private EntityWithDialogue entityWithDialogue;
    [SerializeField] private Button triggerButton;

    private void OnEnable()
    {
        triggerButton.onClick.AddListener(() => DialogueManager.Instance.StartDialogue(
            new InkDialogueHandler(entityWithDialogue.Story)));
    }

    private void OnDisable()
    {
        triggerButton.onClick.RemoveAllListeners();
    }
}

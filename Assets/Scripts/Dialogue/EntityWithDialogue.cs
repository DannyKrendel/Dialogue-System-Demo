using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

public class EntityWithDialogue : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;

    public Story Story { get; private set; }

    private void Awake()
    {
        Story = new Story(textAsset.text);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private CharacterSettings settings;
    private ICharacterInput input;

    private void Start()
    {
        // create input for player or AI
        input = settings.IsAi ? new AIInput() as ICharacterInput : new PlayerInput() ;
    }

    private void Update()
    {
        input.UpdateInput();
    }
}

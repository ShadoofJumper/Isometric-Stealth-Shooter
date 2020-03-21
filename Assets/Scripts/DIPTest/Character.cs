using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] private CharacterSettings settings;
    private ICharacterInput input;
    private CharacterMover characterMover;
    public bool isAlive = true;

    public CharacterCombat characterCombat;

    private void Start()
    {
        // create input for player or AI
        input = settings.IsAi ? new AIInput(settings, transform, this) as ICharacterInput : new PlayerInput();
        //create character mover
        characterMover = new CharacterMover(input, transform, settings);
        //create character combat
        characterCombat = new CharacterCombat(settings.Health, gameObject);

        characterMover.SetStartPosition();
    }

    // paint gizmos if ai
    private void OnDrawGizmos()
    {
        if (settings.IsAi)
        {
            Vector3 startPosition = settings.Path.transform.GetChild(0).position;
            Vector3 previosPosition = startPosition;

            // create path point vizualisation
            foreach (Transform pathPoint in settings.Path.transform)
            {
                Gizmos.DrawSphere(pathPoint.position, 0.2f);
                //paint lines between points
                Gizmos.DrawLine(previosPosition, pathPoint.position);
                previosPosition = pathPoint.position;
            }
            Gizmos.DrawLine(previosPosition, startPosition);
        }

    }

    private void FixedUpdate()
    {
        if (isAlive)
        {
            //move character
            characterMover.Move();
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            input.UpdateInput();
            // look to target character
            characterMover.UpdateLook();
            // check combat health
            characterCombat.Tik();
        }
    }


}

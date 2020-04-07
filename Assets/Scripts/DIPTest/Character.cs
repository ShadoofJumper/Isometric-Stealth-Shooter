using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{

    [SerializeField] private CharacterSettings settings;
    private ICharacterInput input;
    public  ICharacterMover characterMover;
    public Weapon weapon;

    public bool isAlive = false;
    public bool isPlayer;

    public CharacterCombat characterCombat;
    public CharacterSettings Settings => settings;

    private void Start()
    {
        // on creating add character component to gloval dictionary
        SceneController.instance.charactersOnScene.Add(transform, this);

    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy character");
        // removefrom gloval dict after destroy
        SceneController.instance.charactersOnScene.Remove(transform);
        //if (isPlayer)
        //{
        //    Debug.Log("OnDestroy character player");
        //    // disable other component on destroy
        //    gameObject.GetComponent<PitchVolumeField>().enabled = false;
        //}

    }

    public void InitializeCharacter(CharacterSettings settings)
    {
        this.settings = settings;

        isPlayer = !settings.IsAi;

        //create input for player or AI nav
        input = !isPlayer ? new AIInputNav(settings, transform, this) as ICharacterInput : new PlayerInput();

        //create mover
        characterMover = !isPlayer ? new AIMover(input, gameObject, settings) as ICharacterMover : new PlayerMover(input, transform, settings);

        //create character combat
        characterCombat = new CharacterCombat(input, settings.Health, weapon, gameObject, this);

        isAlive = true;
    }


    public void DestroyNavMeshAgent()
    {
        NavMeshAgent characterAgent = GetComponent<NavMeshAgent>();
        if (characterAgent != null)
        {
            Destroy(characterAgent);
        }
    }

    // paint gizmos if ai
    private void OnDrawGizmos()
    {
        if (settings.IsAi && isAlive)
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
            // update move anim
            characterMover.UpdateMoveAnim();
        }
    }

    private void Update()
    {
        if (isAlive)
        {
            input.UpdateInput();
            // look to target character
            characterMover.UpdateMover();
            // check combat health
            characterCombat.Tik();
        }

        // test
        if (isPlayer && Input.GetKeyDown(KeyCode.K))
        {
            characterCombat.TakeDamage(5);
        }
    }


}


public struct SpeedParams
{
    public float steals;
    public float walk;
    public float run;

    public SpeedParams(Vector3 speed)
    {
        steals  = speed[0];
        walk    = speed[1];
        run     = speed[2];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Toggles", menuName = "ScriptableObjects/Toggles", order = 1)]
public class Toggles : ScriptableObject
{
    public bool snapToEdge = false;

    public float pointOffset;

    public enum States
    {
        panning,
        rotatingX,
        rotatingY,
        rotatingZ,
        angle,
        pivot,
        move
    };

    private States currentState = States.panning;

    public void ChangeState(string newState)
    {
        if(System.Enum.TryParse(newState, true, out States parsed))
        {
            if (parsed == currentState) 
                currentState = States.panning;
            else
                currentState = parsed;
            Debug.Log("Our current state is: " + currentState.ToString());
        }
    }

    public States GetCurrentState() => currentState;

    public string GetCurrentStateString()
    {
        switch(currentState)
        {
            case States.rotatingX:
                return "Rotating X";
            case States.rotatingY:
                return "Rotating Y";
            case States.rotatingZ:
                return "Rotating Z";
            case States.angle:
                return "Angle";
            case States.pivot:
                return "Pivot select";
            case States.move:
                return "Move angle";
        }
        return "None";
    }

    public bool IsInRotatingState()
    {
        return currentState == States.rotatingX || currentState == States.rotatingY || currentState == States.rotatingZ;
    }
}

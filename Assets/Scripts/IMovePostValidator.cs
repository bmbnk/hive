using UnityEngine;

public interface IMovePostValidator
{
    bool IsAddingCorrect(GameObject hexToAdd, GameObject moveProposition);
    bool IsMovingCorrect(GameObject hexToMOve, GameObject moveProposition);
}

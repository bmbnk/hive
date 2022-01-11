using UnityEngine;

public interface IMoveValidator
{
    bool CanAdd(PieceType type, bool white);
    bool CanMove(GameObject hex);
}

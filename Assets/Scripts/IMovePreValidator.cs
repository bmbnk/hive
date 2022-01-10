using UnityEngine;

public interface IMovePreValidator
{
    bool CanAdd(PieceType type, bool white);
    bool CanMove(GameObject hex);
}

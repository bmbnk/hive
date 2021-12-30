using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpiderPieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;
    private List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> _oneStepPositionOffsets;

    void Start()
    {
        _type = PieceType.SPIDER;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] originalGameBoard)
    {
        //TODO: Check in the manual for this game, but you should probably check also path
        //from different neighbours that you will meet on this 3 steps road

        int[,] gameBoard = GetGameBoardWithoutHex(originalGameBoard, hexPosition);
        return getMovePositions(gameBoard, hexPosition);
    }

    private List<(int, int)> getMovePositions(int[,] gameBoard, (int, int) hexPosition)
    {
        List<(int, int)> positions = new List<(int, int)>();
        List<(int, int)> neighbours = PieceMovesTools.getNeighbours(hexPosition, gameBoard, _oneStepPositionOffsets);

        neighbours.ForEach(neighbour => {
            positions.Add(getMovePositionFromNeighbour(hexPosition, neighbour, gameBoard, true));
            positions.Add(getMovePositionFromNeighbour(hexPosition, neighbour, gameBoard, false));
        });

        positions = positions.Distinct().ToList();
        positions.Remove(hexPosition);

        return positions;
    }

    private (int, int) getMovePositionFromNeighbour((int, int) hexPosition, (int, int) neighbour, int[,] gameBoard, bool clockwise=true)
    {

        (int, int) movePosition = hexPosition;

        (int, int) lastPivotNeighbour;
        List<(int, int)> notAllowedPositions;
        List<(int, int)> neighbours;

        lastPivotNeighbour = neighbour;


        for (int i = 0; i < 3; i++)
        {
            neighbours = PieceMovesTools.getNeighbours(movePosition, gameBoard, _oneStepPositionOffsets, !clockwise);
            notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, movePosition, _oneStepPositionOffsets);
            (int, int) nextPosition = PieceMovesTools.nextPositionAroundHex(movePosition, lastPivotNeighbour, gameBoard, _oneStepPositionOffsets, clockwise);
            if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
            {
                movePosition = nextPosition;
            }
            else
            {
                int lastPivotNeighbourIdx = neighbours.FindIndex(neighbour => neighbour == lastPivotNeighbour);

                for (int j = 1; j < neighbours.Count; j++)
                {
                    (int, int) nextNeighbour = neighbours[(lastPivotNeighbourIdx + j) % neighbours.Count];
                    nextPosition = PieceMovesTools.nextPositionAroundHex(movePosition, nextNeighbour, gameBoard, _oneStepPositionOffsets, clockwise);
                    if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                    {
                        movePosition = nextPosition;
                        lastPivotNeighbour = nextNeighbour;
                        break;
                    }
                }
            }
        }

        return movePosition;
    }

    private int[,] GetGameBoardWithoutHex(int[,] originalGameBoard, (int, int) hexPosition)
    {
        int[,] gameBoard = (int[,])originalGameBoard.Clone();
        gameBoard[hexPosition.Item1, hexPosition.Item2] = 0;
        return gameBoard;
    }

    public void SetPositionOffsets(List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets)
    {
        _oneStepPositionOffsets = oneStepPositionOffsets;
    }
}

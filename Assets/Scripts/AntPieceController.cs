using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AntPieceController : MonoBehaviour, IPieceController
{
    private PieceType _type;
    private List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> _oneStepPositionOffsets;

    void Start()
    {
        _type = PieceType.ANT;
    }

    public PieceType GetPieceType() { return _type; }

    public List<(int, int)> GetPieceSpecificPositions((int, int) hexPosition, int[,] originalGameBoard)
    {
        // Szukamy przejscia wokol sasiada zgodnie z ruchem wskazowek zegara
        // czyli taka kolejnosc w jakiej sa wpisywane _oneStepPositionOffsets

        // dodajemy do pozycji startowa pozycje

        // PETLA //

        // 1. znajdz sasiadow dla aktualnej pozycji
        // 2. sprawdzaj, czy jestes w stanie poruszyc sie wokol ktoregos sasiada w te strone,
        // ktora jest ustawiona:
        // - bierz pod uwage, ze nie mozesz wejsc jak jest za ciasno (metoda getNotAllowedPositions())
        // - zawsze obracasz sie wokol danego hexa tak dlugo, az dalej nie bedziesz mogl
        // - jesli musisz przejsc na nastepnego hexa i masz wiecej niz jednego do wyboru
        // to wybierz tego, ktory sasiadowal z poprzednim
        // - przy kazdym przejsciu do nastepnej pozycji zapisuj te pozycje do listy
        // - jesli 2 ostatnio dodane pozycje stanowia subliste list positions to konczymy petle
        // i usuwamy ostatnie 2 pozycje

        // usuwamy z pozycji pozycje startowa

        int[,] gameBoard = GetGameBoardWithoutHex(originalGameBoard, hexPosition);

        List<(int, int)> positions = new List<(int, int)>();
        positions.Add(hexPosition);

        bool nextPositionFound = true;
        (int, int) lastPivotNeighbour;
        List<(int, int)> notAllowedPositions;

        List<(int, int)> neighbours = PieceMovesTools.getNeighbours(positions.Last(), gameBoard,
            _oneStepPositionOffsets, false);
        lastPivotNeighbour = neighbours[0];

        while (nextPositionFound)
        {
            nextPositionFound = false;

            neighbours = PieceMovesTools.getNeighbours(positions.Last(), gameBoard, _oneStepPositionOffsets, false);
            notAllowedPositions = PieceMovesTools.getNotAllowedNextPositions(neighbours, positions.Last(), _oneStepPositionOffsets);
            (int, int) nextPosition = PieceMovesTools.nextPositionAroundHex(positions.Last(), lastPivotNeighbour, gameBoard, _oneStepPositionOffsets);
            if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
            {
                positions.Add(nextPosition);
                nextPositionFound = true;
            } else
            {
                int lastPivotNeighbourIdx = neighbours.FindIndex(neighbour => neighbour == lastPivotNeighbour);

                for (int i = 1; i < neighbours.Count; i++)
                {
                    (int, int) nextNeighbour = neighbours[(lastPivotNeighbourIdx + i) % neighbours.Count];
                    nextPosition = PieceMovesTools.nextPositionAroundHex(positions.Last(), nextNeighbour, gameBoard, _oneStepPositionOffsets);
                    if (nextPosition != (-1, -1) && !notAllowedPositions.Contains(nextPosition))
                    {
                        positions.Add(nextPosition);
                        lastPivotNeighbour = nextNeighbour;
                        nextPositionFound = true;
                        break;
                    }
                }
            }

            if(positions.Count > 3 && PieceEnteredLoop(positions))
            {
                for (int i = 0; i < 2; i++)
                    positions.RemoveAt(positions.Count - 1);
                break;
            }
        }

        positions.Remove(hexPosition);

        return positions;
    }

    private int[,] GetGameBoardWithoutHex(int[,] originalGameBoard, (int, int) hexPosition)
    {
        int[,] gameBoard = (int[,])originalGameBoard.Clone();
        gameBoard[hexPosition.Item1, hexPosition.Item2] = 0;
        return gameBoard;
    }

    private bool PieceEnteredLoop(List<(int, int)> positions)
    {
        if(positions.Count > 3)
        {
            for (int i = 0; i < positions.Count - 3; i++)
            {
                if (positions[i] == positions[positions.Count - 2] && positions[i + 1] == positions[positions.Count - 1])
                    return true;
            }
        }
        return false;
    }

    public void SetPositionOffsets(List<((int, int) EvenRowPositionOffset, (int, int) OddRowPositionOffset)> oneStepPositionOffsets)
    {
        _oneStepPositionOffsets = oneStepPositionOffsets;
    }
}

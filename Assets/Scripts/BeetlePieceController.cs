using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class BeetlePieceController : MonoBehaviour, IPieceController
    {
        public PieceType GetPieceType() => PieceType.BEETLE;


        public List<(int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] gameBoard3D)
        {
            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = PieceMovesTools.GetGameBoard2Dfrom3D(gameBoard3D);

            List<(int, int)> positions;

            if (HexesOnPositionNumber(hexPosition, gameBoard3D) > 1)
            {
                positions = PieceMovesTools.GetPositionsAroundPosition(hexPosition);
            }
            else
            {
                positions = PieceMovesTools.GetPositionsNextToNeighboursAroundPosition(hexPosition, gameBoard);
                List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);
                neighbours.ForEach(neighbour => positions.Add(neighbour));
            }


            for (int i = positions.Count - 1; i >= 0; i--)
            {
                int heightOfLowerStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions[i], hexPosition, true), gameBoard3D);
                int heightOfSecondStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions[i], hexPosition, false), gameBoard3D);

                if (heightOfSecondStackNextToPosition < heightOfLowerStackNextToPosition)
                    heightOfLowerStackNextToPosition = heightOfSecondStackNextToPosition;

                int heightOfPositionStack = HexesOnPositionNumber(positions[i], gameBoard3D);
                int heightOfCurrentPositionStack = NumberOfHexesUnderHex(hexPosition3D, gameBoard3D);

                if (heightOfPositionStack < heightOfLowerStackNextToPosition && heightOfCurrentPositionStack < heightOfLowerStackNextToPosition)
                    positions.Remove(positions[i]);
            }

            return positions;
        }

        private int HexesOnPositionNumber((int, int) position, int[,,] gameBoard3D)
        {
            if (gameBoard3D[position.Item1, position.Item2, 0] != 0)
                return NumberOfHexesUnderHex((position.Item1, position.Item2, 0), gameBoard3D) + 1;
            return 0;
        }

        private int NumberOfHexesUnderHex((int, int, int) position, int[,,] gameBoard3D)
        {
            int[] positionStack = Enumerable.Range(position.Item3 + 1, gameBoard3D.GetLength(2)- (position.Item3 + 1))
                .Select(h => gameBoard3D[position.Item1, position.Item2, h])
                .ToArray();

            int hexesCounter = 0;

            foreach (var id in positionStack)
            {
                if (id == 0)
                    break;
                hexesCounter++;
            }

            return hexesCounter;
        }
    }
}

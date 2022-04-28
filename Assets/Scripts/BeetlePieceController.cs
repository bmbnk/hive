using System.Collections.Generic;

namespace Hive
{
    public class BeetlePieceController : IPieceController
    {
        public PieceType GetPieceType() => PieceType.BEETLE;


        public List<(int, int, int)> GetPieceSpecificPositions((int, int, int) hexPosition3D, int[,,] gameBoard3D)
        {
            (int, int) hexPosition = (hexPosition3D.Item1, hexPosition3D.Item2);
            int[,] gameBoard = PieceMovesTools.GetGameBoard2Dfrom3D(gameBoard3D);

            List<(int, int)> positions2D;

            if (HexesOnPositionNumber(hexPosition, gameBoard3D) > 1)
            {
                positions2D = PieceMovesTools.GetPositionsAroundPosition(hexPosition);
            }
            else
            {
                positions2D = PieceMovesTools.GetPositionsNextToNeighboursAroundPosition(hexPosition, gameBoard);
                List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(hexPosition, gameBoard);
                neighbours.ForEach(neighbour => positions2D.Add(neighbour));
            }


            for (int i = positions2D.Count - 1; i >= 0; i--)
            {
                int heightOfLowerStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions2D[i], hexPosition, true), gameBoard3D);
                int heightOfSecondStackNextToPosition = HexesOnPositionNumber(PieceMovesTools.GetNextPositionAroundHex(positions2D[i], hexPosition, false), gameBoard3D);

                if (heightOfSecondStackNextToPosition < heightOfLowerStackNextToPosition)
                    heightOfLowerStackNextToPosition = heightOfSecondStackNextToPosition;

                int heightOfPositionStack = HexesOnPositionNumber(positions2D[i], gameBoard3D);
                //int heightOfCurrentPositionStack = NumberOfHexesUnderHex(hexPosition3D);
                int heightOfCurrentPositionStack = hexPosition3D.Item3;

                if (heightOfPositionStack < heightOfLowerStackNextToPosition && heightOfCurrentPositionStack < heightOfLowerStackNextToPosition)
                    positions2D.Remove(positions2D[i]);
            }

            var positions = new List<(int, int, int)>();

            positions2D.ForEach(position =>
            {
                var positionHeight = HexesOnPositionNumber(position, gameBoard3D);
                positions.Add((position.Item1, position.Item2, positionHeight));
            });

            return positions;
        }

        private int HexesOnPositionNumber((int, int) position, int[,,] gameBoard3D)
        {
            for (int h = 0; h < gameBoard3D.GetLength(2); h++)
            {
                if (gameBoard3D[position.Item1, position.Item2, h] == 0)
                    return h;
            }
            return gameBoard3D.GetLength(2);
        }
    }
}

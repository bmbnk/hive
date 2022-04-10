using UnityEngine;
using System;

namespace Hive
{
    public class GameBoardScript : MonoBehaviour
    {
        public const int GameBoardSize = 25 ;
        public const int CenterPositionX = GameBoardSize / 2;
        public const int CenterPositionY = GameBoardSize / 2;
        private int[,] _gameBoard = new int[GameBoardSize, GameBoardSize];

        // think about board representation:
        // - when giving the board representation to AI it could be benefitial
        //  to include info about pieces that are under some other pieces
        //  (evaluation function has no knowledge about past moves so AI can't
        //  tell if there is a piece under the beetle or not)
        //
        // - there should be a representation with information on type and color
        //  of a piece on position (for AI)

        // there should be a gameboard with a height depth so that height logic
        // could be migrated from BeetlePiece here and RemoveElement could be
        // removed


        public int[,] GetGameBoard()
        {
            return (int[,])_gameBoard.Clone();
        }

        public void ClearGameBoard()
        {
            Array.Clear(_gameBoard, 0, _gameBoard.Length);
        }

        public bool AddElement(int hexId, (int, int) position)
        {
            if (_gameBoard[position.Item1, position.Item2] == 0)
            {
                _gameBoard[position.Item1, position.Item2] = hexId;
                CenterHive();
                return true;
            }
            return false;
        }

        public void RemoveElement(int hexId)
        {
            (int, int) hexPosition = GetPositionByHexId(hexId);
            _gameBoard[hexPosition.Item1, hexPosition.Item2] = 0;
        }

        public bool MoveElement(int hexId, (int, int) targetPosition)
        {
            (int, int) currentPosition = GetPositionByHexId(hexId);
            if (currentPosition != targetPosition
                && currentPosition != (-1, -1)
                && GetHexIdByPosition(targetPosition) == 0)
            {
                _gameBoard[currentPosition.Item1, currentPosition.Item2] = 0;
                _gameBoard[targetPosition.Item1, targetPosition.Item2] = hexId;
                CenterHive();
                return true;
            }
            return false;
        }

        public (int, int) GetPositionByHexId(int hexId)
        {
            for (int i = 0; i < GameBoardSize; i++)
                for (int j = 0; j < GameBoardSize; j++)
                    if (_gameBoard[i, j] == hexId)
                        return (i, j);
            return (-1, -1);
        }

        public int GetHexIdByPosition((int, int) position)
        {
            return _gameBoard[position.Item1, position.Item2];
        }

        private void CenterHive()
        {
            (int, int) massCenter = (0, 0);
            int hexesCounter = 0;

            for (int i = 0; i < GameBoardSize; i++)
                for (int j = 0; j < GameBoardSize; j++)
                    if (_gameBoard[i, j] != 0)
                    {
                        massCenter = (massCenter.Item1 + i,
                            massCenter.Item2 + j);
                        hexesCounter++;
                    }

            massCenter = (massCenter.Item1 / hexesCounter, massCenter.Item2 / hexesCounter);

            int rowOffset = massCenter.Item1 - CenterPositionY;
            int colOffset = massCenter.Item2 - CenterPositionX - (massCenter.Item2 % 2);

            if (rowOffset != 0 || colOffset != 0)
            {
                int[,] centeredGameBoard = new int[GameBoardSize, GameBoardSize];

                //TODO: Check if some hexes are lost when cutting board

                for (int i = 0; i < GameBoardSize; i++)
                    for (int j = 0; j < GameBoardSize; j++)
                        centeredGameBoard[i, j] =
                            _gameBoard[(GameBoardSize + i + rowOffset) % GameBoardSize,
                            (GameBoardSize + j + colOffset) % GameBoardSize];

                _gameBoard = centeredGameBoard;
            }
        }

    }
}

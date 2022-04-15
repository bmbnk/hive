using UnityEngine;
using System;
using System.Linq;

namespace Hive
{
    public class GameBoardScript : MonoBehaviour
    {
        public const int GameBoardSize = 30 ;
        public const int CenterPositionX = GameBoardSize / 2;
        public const int CenterPositionY = GameBoardSize / 2;
        public const int BeetlesPerPlayer = 2;
        public const int GameBoardHeight = 2 * BeetlesPerPlayer + 1;
        private int _firstHexOnBoardId = 0;
        private bool _firstHexAdded = false;
        private int[,,] _gameBoard = new int[GameBoardSize, GameBoardSize, GameBoardHeight];

        // height zero is the heighest 

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


        public int[,] GetGameBoard2D()
        {
            int[,] gameBoard2D = new int[GameBoardSize, GameBoardSize];

            for (int row = 0; row < GameBoardSize; row++)
                for (int col = 0; col < GameBoardSize; col++)
                    gameBoard2D[row, col] = _gameBoard[row, col, 0];

            return gameBoard2D; 
        }

        public int[,,] GetGameBoard3D()
        {
            return (int[,,])_gameBoard.Clone();
        }

        public void ClearGameBoard()
        {
            Array.Clear(_gameBoard, 0, _gameBoard.Length);
        }

        public void AddElement(int hexId, (int, int) position)
        {
            AddElementToArray(hexId, position);
            CenterHive();
            if (!_firstHexAdded)
            {
                _firstHexOnBoardId = hexId;
                _firstHexAdded = true;
            }
        }

        private void AddElementToArray(int hexId, (int, int) position)
        {
            int[] positionStack = Enumerable.Range(0, _gameBoard.GetLength(2))
                .Select(h => _gameBoard[position.Item1, position.Item2, h])
                .ToArray();

            _gameBoard[position.Item1, position.Item2, 0] = hexId;

            for (int i = 1; i < GameBoardHeight; i++)
            {
                _gameBoard[position.Item1, position.Item2, i] = positionStack[i - 1];
            }
        }

        public void RemoveElement(int hexId)
        {
            (int, int) hexPosition = Get2DTopPositionByHexId(hexId);
            RemoveElementFromArray(hexPosition);
        }

        private void RemoveElementFromArray((int, int) position)
        {
            if (position != (-1, -1))
            {
                int[] positionStack = Enumerable.Range(1, _gameBoard.GetLength(2)-1)
                    .Select(h => _gameBoard[position.Item1, position.Item2, h])
                    .ToArray();

                for (int i = 0; i < GameBoardHeight - 1; i++)
                {
                    _gameBoard[position.Item1, position.Item2, i] = positionStack[i];
                }

                _gameBoard[position.Item1, position.Item2, GameBoardHeight-1] = 0;
            }
        }

        public void MoveElement(int hexId, (int, int) targetPosition)
        {
            (int, int) currentPosition = Get2DTopPositionByHexId(hexId);
            if (currentPosition != targetPosition && currentPosition != (-1, -1))
            {
                RemoveElement(hexId);
                AddElement(hexId, targetPosition);
                CenterHive();
            }
        }

        public int NumberOfHexesUnderHex(int hexId)
        {
            (int, int, int) hexPosition = GetPositionByHexId(hexId);
            int height = _gameBoard.GetLength(2);
            int[] positionStack = Enumerable.Range(hexPosition.Item3 + 1, _gameBoard.GetLength(2)-(hexPosition.Item3+1))
                .Select(h => _gameBoard[hexPosition.Item1, hexPosition.Item2, h])
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

        public (int, int) GetFirstHexOnBoardPosition()
        {
            if (_firstHexAdded)
                return Get2DPositionByHexId(_firstHexOnBoardId);
            return (CenterPositionY, CenterPositionX);
        }

        public (int, int) Get2DTopPositionByHexId(int hexId)
        {
            for (int i = 0; i < GameBoardSize; i++)
                for (int j = 0; j < GameBoardSize; j++)
                    if (_gameBoard[i, j, 0] == hexId)
                        return (i, j);
            return (-1, -1);
        }

        public (int, int) Get2DPositionByHexId(int hexId)
        {
            var position3D = GetPositionByHexId(hexId);
            return (position3D.Item1, position3D.Item2);
        }

        public (int, int, int) GetPositionByHexId(int hexId)
        {
            for (int row = 0; row < GameBoardSize; row++)
                for (int col = 0; col < GameBoardSize; col++)
                    for (int height = 0; height < GameBoardHeight; height++)
                        if (_gameBoard[row, col, height] == hexId)
                            return (row, col, height);
            return (-1, -1, -1);
        }

        public int GetTopHexIdByPosition((int, int) position)
        {
            return _gameBoard[position.Item1, position.Item2, 0];
        }

        private void CenterHive()
        {
            (int, int) massCenter = (0, 0);
            int hexesCounter = 0;

            for (int i = 0; i < GameBoardSize; i++)
                for (int j = 0; j < GameBoardSize; j++)
                    if (_gameBoard[i, j, 0] != 0)
                    {
                        massCenter = (massCenter.Item1 + i,
                            massCenter.Item2 + j);
                        hexesCounter++;
                    }

            massCenter = (massCenter.Item1 / hexesCounter, massCenter.Item2 / hexesCounter);

            int rowOffset = massCenter.Item1 - CenterPositionY - ((massCenter.Item1 - CenterPositionY) % 2);
            int colOffset = massCenter.Item2 - CenterPositionX;

            if (rowOffset != 0 || colOffset != 0)
            {
                int[,,] centeredGameBoard = new int[GameBoardSize, GameBoardSize, GameBoardHeight];

                //TODO: Check if some hexes are lost when cutting board

                for (int row = 0; row < GameBoardSize; row++)
                    for (int col = 0; col < GameBoardSize; col++)
                        for (int height = 0; height < GameBoardHeight; height++)
                            centeredGameBoard[row, col, height] =
                                _gameBoard[(GameBoardSize + row + rowOffset) % GameBoardSize,
                                (GameBoardSize + col + colOffset) % GameBoardSize, height];

                _gameBoard = centeredGameBoard;
            }
        }
    }
}

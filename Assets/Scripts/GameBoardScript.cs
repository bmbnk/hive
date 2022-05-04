using UnityEngine;
using System;

namespace Hive
{
    public class GameBoardScript : MonoBehaviour
    {
        public const int GameBoardSize = 30;
        public const int CenterPositionX = GameBoardSize / 2;
        public const int CenterPositionY = GameBoardSize / 2;
        public const int BeetlesPerPlayer = 2;
        public const int GameBoardHeight = 2 * BeetlesPerPlayer + 1;

        private int _firstHexOnBoardId = 0;
        private bool _firstHexAdded = false;
        private int[,,] _gameBoard = new int[GameBoardSize, GameBoardSize, GameBoardHeight];

        // height zero is the lowest


        public int[,] GetGameBoard2D()
        {
            int[,] gameBoard2D = new int[GameBoardSize, GameBoardSize];

            for (int row = 0; row < GameBoardSize; row++)
                for (int col = 0; col < GameBoardSize; col++)
                    gameBoard2D[row, col] = GetTopHexIdByPosition((row, col));

            return gameBoard2D; 
        }

        public int[,,] GetGameBoard3D()
        {
            return (int[,,])_gameBoard.Clone();
        }

        public void ResetGameBoard()
        {
            _firstHexAdded = false;
            _firstHexOnBoardId = 0;
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

        public void AddElement(int hexId, (int, int, int) position)
        {
            if (_gameBoard[position.Item1, position.Item2, position.Item3] != 0)
                throw new Exception("Adding piece to the existing piece position.");

            AddElementToArray(hexId, (position.Item1, position.Item2));
            CenterHive();
            if (!_firstHexAdded)
            {
                _firstHexOnBoardId = hexId;
                _firstHexAdded = true;
            }
        }

        private void AddElementToArray(int hexId, (int, int) position)
        {
            for (int i = 0; i < GameBoardHeight; i++)
            {
                if (_gameBoard[position.Item1, position.Item2, i] == 0)
                {
                    _gameBoard[position.Item1, position.Item2, i] = hexId;
                    return;
                }
            }
            throw new Exception("There is no place to add a new piece.");
        }

        public void MoveElement(int hexId, (int, int) targetPosition)
        {
            (int, int) currentPosition = Get2DTopPositionByHexId(hexId);
            if (currentPosition != targetPosition && currentPosition != (-1, -1))
            {
                RemoveTopElementFromArray(hexId);
                AddElement(hexId, targetPosition);
                CenterHive();
            }
        }

        public void MoveElement(int hexId, (int, int, int) targetPosition)
        {
            (int, int, int) currentPosition = GetPositionByHexId(hexId);
            if (currentPosition != targetPosition && currentPosition != (-1, -1, -1))
            {
                RemoveTopElementFromArray(hexId);
                AddElement(hexId, targetPosition);
                CenterHive();
            } else
                throw new Exception("Moving piece that does not exist or " +
                    "moving it to the same position.");
        }

        private void RemoveTopElementFromArray(int hexId)
        {
            (int, int) position = Get2DTopPositionByHexId(hexId);

            if (position != (-1, -1))
            {
                for (int i = 1; i < GameBoardHeight; i++)
                {
                    if (_gameBoard[position.Item1, position.Item2, i] == 0)
                    {
                        _gameBoard[position.Item1, position.Item2, i - 1] = 0;
                        return;
                    }
                }

                _gameBoard[position.Item1, position.Item2, GameBoardHeight - 1] = 0;
            }
        }

        public int NumberOfHexesUnderHex(int hexId)
        {
            return GetPositionByHexId(hexId).Item3;
        }

        public (int, int, int) GetFirstHexOnBoardPosition()
        {
            if (_firstHexAdded)
                return GetPositionByHexId(_firstHexOnBoardId);
            return (CenterPositionY, CenterPositionX, 0);
        }

        public (int, int) Get2DTopPositionByHexId(int hexId)
        {
            for (int i = 0; i < GameBoardSize; i++)
                for (int j = 0; j < GameBoardSize; j++)
                    if (GetTopHexIdByPosition((i, j)) == hexId)
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
            int topHexId = 0;

            for (int h = 0; h < GameBoardHeight; h++)
            {
                if (_gameBoard[position.Item1, position.Item2, h] == 0)
                    break;
                topHexId = _gameBoard[position.Item1, position.Item2, h];
            }
            return topHexId;
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


            //var gameBoardRepresentation = GameBoardRepresentations.GetFlatGameboard(GetGameBoard3D());
            //Debug.Log(ArrayToString(gameBoardRepresentation));
            //var trimmedGameBoardRepresentation = GameBoardRepresentations.GetFlatGameboard(GetGameBoard3D(), true);
            //Debug.Log(ArrayToString(trimmedGameBoardRepresentation));
        }

        //private string ArrayToString(int[,] array)
        //{
        //    string arrayString = "";

        //    for (int i = 0; i < array.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < array.GetLength(1); j++)
        //            arrayString += (array[i, j] == 0 ? "_" : array[i, j].ToString()) + " ";
        //        arrayString += "\n";
        //    }
        //    return arrayString;
        //}
    }
}

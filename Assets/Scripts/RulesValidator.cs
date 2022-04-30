using System.Collections.Generic;
using UnityEngine;

namespace Hive
{
    public class RulesValidator : MonoBehaviour
    {
        private GameBoardScript _gameBoard;
        private GameEngineScript _gameEngine;

        void Start()
        {
            GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
            _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

            GameObject gameEngineGameObject = GameObject.FindWithTag("GameEngine");
            _gameEngine = gameEngineGameObject.GetComponent<GameEngineScript>();
        }


        public bool BlackHexesWon()
        {
            return IsBeeFullySurrounded(true);
        }

        public bool CanMakeMove(bool white)
        {
            return CanMove(white) || CanAdd(white);
        }

        public bool CanMoveHex(int hexId)
        {
            var position = _gameBoard.Get2DTopPositionByHexId(hexId);

            return position != (-1, -1)
                && IsBeeOnBoard(HexIdToPiecePropertyMapper.IsPieceWhite(hexId))
                && !IsOneHiveRuleBroken(hexId);
        }

        public bool GameIsDrawn()
        {
            return BlackHexesWon() && WhiteHexesWon();
        }

        private int GetFirstFoundPieceOnBoardId(bool white, PieceType pieceType)
        {
            var hexesOnBoardIds = white ? _gameEngine.WhiteHexesOnBoardIds : _gameEngine.BlackHexesOnBoardIds;
            int pieceHexId = -1;

            foreach (var hexId in hexesOnBoardIds)
            {
                if (HexIdToPiecePropertyMapper.GetPieceType(hexId) == pieceType)
                {
                    pieceHexId = hexId;
                    break;
                }
            }
            return pieceHexId;
        }

        public bool IsBeeOnBoard(bool white)
        {
            return GetFirstFoundPieceOnBoardId(white, PieceType.BEE) != -1;
        }

        public bool IsBeeOnBoardRuleBroken(bool white) //If it is fourth move of the player and the bee piece is not on the table than the rule is broken
        {
            var hexesOnBoardIds = white ? _gameEngine.WhiteHexesOnBoardIds : _gameEngine.BlackHexesOnBoardIds;
            return hexesOnBoardIds.Count > 2 && !IsBeeOnBoard(white);
        }

        public bool IsGameOver()
        {
            return BlackHexesWon() || WhiteHexesWon();
        }

        public bool IsOneHiveRuleBroken(int hexId)
        {
            if (_gameBoard.NumberOfHexesUnderHex(hexId) > 0)
                return false;

            int[,] gameBoardWithoutHex = _gameBoard.GetGameBoard2D();

            (int, int) hexPositionOnBoard = _gameBoard.Get2DTopPositionByHexId(hexId);

            if (hexPositionOnBoard == (-1, -1))
                return false;

            gameBoardWithoutHex[hexPositionOnBoard.Item1, hexPositionOnBoard.Item2] = 0;

            // TODO: You can optimize it by choosing one neighbour from groups of neighbours that are connected, because if you can reached one, than you can reach all of them
            List<(int, int)> neighboursPositions = PieceMovesTools.GetNeighbours(hexPositionOnBoard, gameBoardWithoutHex);

            (int, int) firstNeighbour = neighboursPositions[0];
            neighboursPositions.Remove(firstNeighbour);


            foreach (var neighbourPosition in neighboursPositions)
            {
                if (!PositionsAreConnected(firstNeighbour, neighbourPosition, gameBoardWithoutHex))
                    return true;
            }
            return false;
        }

        public bool WhiteHexesWon()
        {
            return IsBeeFullySurrounded(false);
        }


        private bool CanAdd(bool white)
        {
            return _gameEngine.GetAddingMovesForPlayer(white).Count > 0;
        }

        private bool CanMove(bool white)
        {
            var hexesOnBoardIds = white ? _gameEngine.WhiteHexesOnBoardIds : _gameEngine.BlackHexesOnBoardIds;

            foreach (var hexId in hexesOnBoardIds)
            {
                if (CanMoveHex(hexId))
                {
                    List<(int, int, int)> availableMovePositions = _gameEngine.GetAvailableMovePositionsForHex(hexId);
                    if (availableMovePositions.Count > 0)
                        return true;
                }
            }
            return false;
        }

        private bool DFS((int, int) startPosition, (int, int) endPosition, int[,] gameBoard)
        {
            List<(int, int)> visitedPositions = new List<(int, int)>();
            Stack<(int, int)> notFullyExploredPositions = new Stack<(int, int)>();
            notFullyExploredPositions.Push(startPosition);

            return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
        }

        private bool DFSStep((int, int) endPosition, int[,] gameBoard, Stack<(int, int)> notFullyExploredPositions, List<(int, int)> visitedPositions)
        {
            (int, int) currentPosition = notFullyExploredPositions.Pop();

            if (currentPosition == endPosition)
                return true;

            visitedPositions.Add(currentPosition);

            List<(int, int)> neighbours = PieceMovesTools.GetNeighbours(currentPosition, gameBoard);
            foreach (var neighbour in neighbours)
            {
                if (!visitedPositions.Contains(neighbour))
                {
                    notFullyExploredPositions.Push(currentPosition);
                    notFullyExploredPositions.Push(neighbour);
                    return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
                }
            }

            if (notFullyExploredPositions.Count == 0)
                return false;

            return DFSStep(endPosition, gameBoard, notFullyExploredPositions, visitedPositions);
        }

        private bool IsBeeFullySurrounded(bool white)
        {
            int beeHexId = GetFirstFoundPieceOnBoardId(white, PieceType.BEE);

            if (beeHexId != -1)
            {
                if (PieceMovesTools.GetNeighbours(_gameBoard.Get2DPositionByHexId(beeHexId), _gameBoard.GetGameBoard2D()).Count == 6)
                    return true;
            }

            return false;
        }

        private bool PositionsAreConnected((int, int) startPosition, (int, int) endPosition, int[,] gameBoard)
        {
            List<(int, int)> visitedPositions = new List<(int, int)>();
            visitedPositions.Add(startPosition);

            return DFS(startPosition, endPosition, gameBoard);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class RulesValidator : MonoBehaviour
    {
        public GameObject BeetlePiece;
        private HexesStoreScript _hexesStore;
        private HexesInfoProvider _hexesInfoProvider;
        private GameBoardScript _gameBoard;

        void Start()
        {
            GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
            _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

            GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
            _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

            GameObject hexesInfoProviderGameObject = GameObject.FindWithTag("HexesInfoProvider");
            _hexesInfoProvider = hexesInfoProviderGameObject.GetComponent<HexesInfoProvider>();
        }

        public bool CanMakeMove(bool white)
        {
            return CanMove(white) || CanAdd(white);
        }

        private bool CanMove(bool white)
        {
            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;

            foreach (var hex in hexes)
            {
                var hexScript = hex.GetComponent<HexWrapperController>();
                if (hexScript.isOnGameboard && CanMoveHex(hex))
                {
                    List<(int, int)> availableMovePositions = hexScript
                    .piece
                    .GetComponent<IPieceController>()
                    .GetPieceSpecificPositions(_gameBoard.GetPositionByHexId(hexScript.HexId), _gameBoard.GetGameBoard());
                    if (availableMovePositions.Count > 0)
                        return true;
                }
            }
            return false;
        }

        private bool CanAdd(bool white)
        {
            if (AnyPositionToAddExists(white))
            {
                foreach (var type in Enum.GetValues(typeof(PieceType)).Cast<PieceType>())
                {
                    if (AnyHexToAddExists(type, white))
                        return true;
                }
            }
            return false;
        }

        private bool AnyHexToAddExists(PieceType type, bool white)
        {
            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            int hexToAddPropositionIdx = hexes.FindIndex(hex =>
            {
                var hexScript = hex.GetComponent<HexWrapperController>();
                return !hexScript.isOnGameboard && hexScript.piece.GetComponent<IPieceController>().GetPieceType() == type;
            });
            return hexToAddPropositionIdx != -1;
        }

        private bool AnyPositionToAddExists(bool white)
        {
            if (!_hexesInfoProvider.FirstMovesWereMade())
                return true;

            var hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;
            List<(int, int)> availablePositions = PieceMovesTools.GetPositionsToAddHex(hexesOnBoardIds, _gameBoard.GetGameBoard());
            if (availablePositions.Count > 0)
                return true;

            return false;
        }

        public bool CanMoveHex(GameObject hex)
        {
            var hexScript = hex.GetComponent<HexWrapperController>();
            var beetleScript = BeetlePiece.GetComponent<BeetlePieceController>();
            return !beetleScript.IsHexUnderneathBeetle(hexScript.HexId)
                && _hexesInfoProvider.IsBeeOnBoard(hexScript.isWhite)
                && !IsOneHiveRuleBroken(hex);
        }

        public bool IsBeeOnGameboardRuleBroken(bool white) //If it is fourth move of the player and the bee piece is not on the table than the rule is broken
        {
            var hexesOnBoardIds = white ? _hexesStore.whiteHexesOnBoardIds : _hexesStore.blackHexesOnBoardIds;

            if (hexesOnBoardIds.Count > 2 && !_hexesInfoProvider.IsBeeOnBoard(white))
                return true;
            return false;
        }

        public bool IsOneHiveRuleBroken(GameObject hexToMove)
        {
            var hexToMoveScript = hexToMove.GetComponent<HexWrapperController>();

            if (hexToMoveScript.transform.position.y > 0)
                return false;

            //int[,] gameBoardWithoutHex = (int[,])_gameBoard.gameBoard.Clone();
            int[,] gameBoardWithoutHex = _gameBoard.GetGameBoard();

            (int, int) hexPositionOnBoard = _gameBoard.GetPositionByHexId(hexToMoveScript.HexId);
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

        private bool PositionsAreConnected((int, int) startPosition, (int, int) endPosition, int[,] gameBoard)
        {
            List<(int, int)> visitedPositions = new List<(int, int)>();
            visitedPositions.Add(startPosition);

            return DFS(startPosition, endPosition, gameBoard);
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
    }
}

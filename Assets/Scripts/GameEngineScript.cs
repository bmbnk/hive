using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Hive
{
    public class GameEngineScript : MonoBehaviour
    {
        private GameBoardScript _gameBoard;
        private RulesValidator _rulesValidator;

        private GameState _gameState = GameState.NotStarted;
        public GameState GameState => _gameState;

        public int[,,] GameBoard => _gameBoard.GetGameBoard3D();

        private bool _isWhiteTurn = false;
        public bool IsWhiteTurn => _isWhiteTurn;

        private ISet<int> _whiteHexesOnBoardIds = new HashSet<int>();
        private ISet<int> _blackHexesOnBoardIds = new HashSet<int>();

        public ISet<int> WhiteHexesOnBoardIds => new HashSet<int>(_whiteHexesOnBoardIds);
        public ISet<int> BlackHexesOnBoardIds => new HashSet<int>(_blackHexesOnBoardIds);


        void Start()
        {
            Init();
        }

        private void Init()
        {
            GameObject gameBoardGameobject = GameObject.FindWithTag("GameBoard");
            _gameBoard = gameBoardGameobject.GetComponent<GameBoardScript>();

            GameObject moveValidatorGameObject = GameObject.FindWithTag("RulesValidator");
            _rulesValidator = moveValidatorGameObject.GetComponent<RulesValidator>();
        }

        public Dictionary<int, List<(int, int, int)>> GetAddingMovesForPlayer(bool white)
        {
            var moves = new Dictionary<int, List<(int, int, int)>>();

            var playerHexesOnBoardIds = white ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;

            var availablePositions = GetAvailableAddingPositions(white);
            if (availablePositions.Count > 0)
            {
                int idStartValue = white ? 1 : HexIdToPiecePropertyMapper.WhitePiecesBoundaryId + 1;
                for (int id = idStartValue; id < idStartValue + HexIdToPiecePropertyMapper.WhitePiecesBoundaryId; id++)
                    if (!playerHexesOnBoardIds.Contains(id))
                        moves.Add(id, availablePositions);
            }
            return moves;
        }

        public List<(int, int, int)> GetAvailableAddingPositions(bool white)
        {
            var hexesOnBoardIds = white ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
            var opponentHexesOnBoardIds = white ? _blackHexesOnBoardIds : _whiteHexesOnBoardIds;

            List<(int, int)> availablePositions2D;
            if (_blackHexesOnBoardIds.Count > 0 && _whiteHexesOnBoardIds.Count > 0)
            {
                availablePositions2D = PieceMovesTools.GetPositionsToAddHex(hexesOnBoardIds.ToList(), _gameBoard.GetGameBoard2D());
            }
            else if (_gameState == GameState.NotStarted)
            {
                availablePositions2D = new List<(int, int)>();
                availablePositions2D.Add((GameBoardScript.CenterPositionY, GameBoardScript.CenterPositionX));
            }
            else
            {
                var firstHexOnBoardPosition = _gameBoard.GetFirstHexOnBoardPosition();
                availablePositions2D = PieceMovesTools.GetFreePositionsAroundPosition(
                    (firstHexOnBoardPosition.Item1, firstHexOnBoardPosition.Item2),
                    _gameBoard.GetGameBoard2D());
            }

            List<(int, int, int)> availablePositions = new List<(int, int, int)>();
            availablePositions2D.ForEach(position => availablePositions.Add((position.Item1, position.Item2, 0)));

            return availablePositions;
        }

        public List<(int, int, int)> GetAvailableMovePositionsForHex(int hexId)
        {
            if (_rulesValidator.CanMoveHex(hexId))
            {
                //IPieceController pieceController = _pieceTypeToControllerMapping[_hexIdToPieceTypeMapping[hexId]];
                IPieceController pieceController = HexIdToPiecePropertyMapper.GetPieceController(hexId);

                return pieceController.GetPieceSpecificPositions(
                    _gameBoard.GetPositionByHexId(hexId),
                    _gameBoard.GetGameBoard3D());
            }
            return new List<(int, int, int)>();
        }

        public Dictionary<int, List<(int, int, int)>> GetMovesForPlayer()
        {
            var moves = GetMovingMovesForPlayer(_isWhiteTurn);

            foreach (var move in GetAddingMovesForPlayer(_isWhiteTurn))
                moves.Add(move.Key, move.Value);

            return moves;
        }

        public Dictionary<int, List<(int, int, int)>> GetMovingMovesForPlayer(bool white)
        {
            var moves = new Dictionary<int, List<(int, int, int)>>();

            var playerHexesOnBoardIds = white ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;

            foreach (var hexId in playerHexesOnBoardIds)
            {
                var availableMovePositions = GetAvailableMovePositionsForHex(hexId);
                if (availableMovePositions.Count > 0)
                {
                    moves.Add(hexId, availableMovePositions);
                }
            }

            return moves;
        }

        public (int, int, int) GetPositionByHexId(int hexId)
        {
            return _gameBoard.GetPositionByHexId(hexId);
        }

        public bool IsMoveValid(int hexId, (int, int, int) targetPosition)
        {
            return  CanAddPiece(hexId, targetPosition) || CanMovePiece(hexId, targetPosition);
        }

        public bool MakeMove(int hexId, (int, int, int) targetPosition)
        {
            if (CanMovePiece(hexId, targetPosition))
                MovePiece(hexId, targetPosition);
            else if (CanAddPiece(hexId, targetPosition))
                AddPiece(hexId, targetPosition);
            else
                return false;

            if (_rulesValidator.WhiteHexesWon())
                _gameState = GameState.WhiteWon;
            else if (_rulesValidator.BlackHexesWon())
                _gameState = GameState.BlackWon;
            else if (_rulesValidator.GameIsDrawn())
                _gameState = GameState.Draw;

            if (_rulesValidator.CanMakeMove(!_isWhiteTurn))
                _isWhiteTurn = !_isWhiteTurn;
            return true;
        }

        public void Reset()
        {
            _whiteHexesOnBoardIds.Clear();
            _blackHexesOnBoardIds.Clear();
            _gameBoard.ResetGameBoard();
            _gameState = GameState.NotStarted;
        }

        public void SetWhiteStarts(bool whiteStarts)
        {
            if (_gameState == GameState.NotStarted)
                _isWhiteTurn = whiteStarts;
        }

        private void AddPiece(int hexId, (int, int, int) targetPosition)
        {
            _gameBoard.AddElement(hexId, targetPosition);
            var hexesOnBoardIds = HexIdToPiecePropertyMapper.IsPieceWhite(hexId) ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
            if (_whiteHexesOnBoardIds.Count == 0 && _blackHexesOnBoardIds.Count == 0)
                _gameState = GameState.InProgress;
            hexesOnBoardIds.Add(hexId);
        }

        private bool CanAddPiece(int hexId, (int, int, int) targetPosition)
        {
            if (!_whiteHexesOnBoardIds.Contains(hexId) && !_blackHexesOnBoardIds.Contains(hexId))
            {
                var availablePositions = GetAvailableAddingPositions(HexIdToPiecePropertyMapper.IsPieceWhite(hexId));
                return availablePositions.Contains(targetPosition);
            }
            return false;
        }

        private bool CanMovePiece(int hexId, (int, int, int) targetPosition)
        {
            if ((_whiteHexesOnBoardIds.Contains(hexId) || _blackHexesOnBoardIds.Contains(hexId))
                && _rulesValidator.CanMoveHex(hexId)) {
                var availablePositions = GetAvailableMovePositionsForHex(hexId);
                return availablePositions.Contains(targetPosition);
            }
            return false;
        }

        private void MovePiece(int hexId, (int, int, int) targetPosition)
        {
            _gameBoard.MoveElement(hexId, targetPosition);
            //LogToFileAsync($"({hexId}, {targetPosition.Item1}, {targetPosition.Item2}, {targetPosition.Item3})");
            //LogToFileAsync(ArrayToString(GameBoardRepresentations.GetOriginalGameBoardFromPaper(_gameBoard.GetGameBoard3D())));
        }

        //private async Task LogToFileAsync(string text)
        //{
        //    using StreamWriter file = new("game.txt", append: true);
        //    await file.WriteLineAsync(text);
        //}

        //private string ArrayToString(int[,] array)
        //{
        //    string arrayString = "";

        //    for(int i = 0; i < array.GetLength(0); i++)
        //    {
        //        for (int j = 0; j < array.GetLength(1); j++)
        //            arrayString += (array[i, j] == 0 ? "_" : array[i, j].ToString()) + " ";
        //        arrayString += "\n";
        //    }
        //    return arrayString;
        //}
    }
}

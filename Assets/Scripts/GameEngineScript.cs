using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hive
{
    public class GameEngineScript : MonoBehaviour
    {
        private const int WhitePiecesBoundaryId = 11;

        private GameBoardScript _gameBoard;
        private RulesValidator _rulesValidator;

        private GameState _gameState = GameState.NotStarted;
        public GameState GameState => _gameState;

        private bool _isWhiteTurn = true;
        public bool IsWhiteTurn => _isWhiteTurn;

        private ISet<int> _whiteHexesOnBoardIds = new HashSet<int>();
        private ISet<int> _blackHexesOnBoardIds = new HashSet<int>();

        public ISet<int> WhiteHexesOnBoardIds => new HashSet<int>(_whiteHexesOnBoardIds);
        public ISet<int> BlackHexesOnBoardIds => new HashSet<int>(_blackHexesOnBoardIds);

        private Dictionary<int, PieceType> _hexIdToPieceTypeMapping =
            new Dictionary<int, PieceType>()
            {
                {1, PieceType.BEE},
                {2, PieceType.BEETLE},
                {3, PieceType.BEETLE},
                {4, PieceType.SPIDER},
                {5, PieceType.SPIDER},
                {6, PieceType.ANT},
                {7, PieceType.ANT},
                {8, PieceType.ANT},
                {9, PieceType.GRASSHOPPER},
                {10, PieceType.GRASSHOPPER},
                {11, PieceType.GRASSHOPPER},

                {12, PieceType.BEE},
                {13, PieceType.BEETLE},
                {14, PieceType.BEETLE},
                {15, PieceType.SPIDER},
                {16, PieceType.SPIDER},
                {17, PieceType.ANT},
                {18, PieceType.ANT},
                {19, PieceType.ANT},
                {20, PieceType.GRASSHOPPER},
                {21, PieceType.GRASSHOPPER},
                {22, PieceType.GRASSHOPPER}
            };

        private Dictionary<PieceType, IPieceController> _pieceTypeToControllerMapping =
            new Dictionary<PieceType, IPieceController>()
            {
                {PieceType.BEE, new BeePieceController()},
                {PieceType.BEETLE, new BeetlePieceController()},
                {PieceType.SPIDER, new SpiderPieceController()},
                {PieceType.ANT, new AntPieceController()},
                {PieceType.GRASSHOPPER, new GrasshopperPieceController()},
            };


        void Start()
        {
            Initialize();
        }

        private void Initialize()
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
                int idStartValue = white ? 1 : WhitePiecesBoundaryId + 1;
                for (int id = idStartValue; id < idStartValue + WhitePiecesBoundaryId; id++)
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
                IPieceController pieceController = _pieceTypeToControllerMapping[_hexIdToPieceTypeMapping[hexId]];

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

        // there should be some check and return default value
        public PieceType GetPieceType(int hexId)
        {
            return _hexIdToPieceTypeMapping[hexId];
        }

        public (int, int, int) GetPositionByHexId(int hexId)
        {
            return _gameBoard.GetPositionByHexId(hexId);
        }

        public bool IsMoveValid(int hexId, (int, int, int) targetPosition)
        {
            return  CanAddPiece(hexId, targetPosition) || CanMovePiece(hexId, targetPosition);
        }

        public bool IsPieceWhite(int hexId)
        {
            return hexId <= WhitePiecesBoundaryId;
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
            var hexesOnBoardIds = IsPieceWhite(hexId) ? _whiteHexesOnBoardIds : _blackHexesOnBoardIds;
            if (_whiteHexesOnBoardIds.Count == 0 && _blackHexesOnBoardIds.Count == 0)
                _gameState = GameState.InProgress;
            hexesOnBoardIds.Add(hexId);
        }

        private bool CanAddPiece(int hexId, (int, int, int) targetPosition)
        {
            if (!_whiteHexesOnBoardIds.Contains(hexId) && !_blackHexesOnBoardIds.Contains(hexId))
            {
                var availablePositions = GetAvailableAddingPositions(IsPieceWhite(hexId));
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
        }
    }
}

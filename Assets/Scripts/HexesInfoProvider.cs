using UnityEngine;

namespace Hive
{
    public class HexesInfoProvider : MonoBehaviour
    {
        private HexesStoreScript _hexesStore;
        private GameEngineScript _gameEngine;

        private void Start()
        {
            GameObject hexesStoreGameObject = GameObject.FindWithTag("HexesStore");
            _hexesStore = hexesStoreGameObject.GetComponent<HexesStoreScript>();

            GameObject gameEngineGameObject = GameObject.FindWithTag("GameEngine");
            _gameEngine = gameEngineGameObject.GetComponent<GameEngineScript>();
        }

        public bool FirstMovesWereMade()
        {
            return _gameEngine.BlackHexesOnBoardIds.Count > 0 && _gameEngine.WhiteHexesOnBoardIds.Count > 0;
        }

        public int GetRemainingHexCount(PieceType pieceType, bool white)
        {
            var hexes = white ? _hexesStore.whiteHexes : _hexesStore.blackHexes;
            var hexesOnBoardIds = white ? _gameEngine.WhiteHexesOnBoardIds : _gameEngine.BlackHexesOnBoardIds;
            int counter = 0;

            hexes.ForEach(hex =>
            {
                var hexScript = hex.GetComponent<HexWrapperController>();
                if (_gameEngine.GetPieceType(hexScript.HexId) == pieceType && !hexesOnBoardIds.Contains(hexScript.HexId))
                    counter++;
            });

            return counter;
        }

        public bool IsAnyHexLeftInHand()
        {
            return _gameEngine.WhiteHexesOnBoardIds.Count < _hexesStore.whiteHexes.Count
                || _gameEngine.BlackHexesOnBoardIds.Count < _hexesStore.blackHexes.Count;
        }

        public bool IsItCurrentPlayerHex(GameObject selectedHex, bool isWhiteTurn)
        {
            return !IsItPropositionHex(selectedHex)
                && _gameEngine.IsPieceWhite(selectedHex.GetComponent<HexWrapperController>().HexId) == isWhiteTurn;
        }

        public bool IsItFirstMove()
        {
            return _gameEngine.BlackHexesOnBoardIds.Count == 0 && _gameEngine.WhiteHexesOnBoardIds.Count == 0;
        }

        public bool IsItPropositionHex(GameObject selectedHex)
        {
            return selectedHex.GetComponent<HexWrapperController>().HexId == 0;
        }
    }
}

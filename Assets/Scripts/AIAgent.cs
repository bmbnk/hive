using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Sensors.Reflection;
using UnityEngine;

namespace Hive
{
    public class AIAgent : Agent
    {
        //[Observable]
        //int[,,] GameBoard
        //{
        //    get
        //    {
        //        return _gameBoard.GetGameBoard3D();
        //    }
        //}

        GameEngineScript _gameEngine;

        void Start()
        {
            Init();
        }

        private void Init()
        {
            GameObject gameEngineGameObject = GameObject.FindWithTag("GameEngine");
            _gameEngine = gameEngineGameObject.GetComponent<GameEngineScript>();
        }


        public override void OnEpisodeBegin()
        {
            _gameEngine.Reset();
            RequestDecision();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            ////    // you can only pass changes to the gameboard each time so that it
            ////    // can be updated in python script

            //List<float> allPiecesIdsInHexIdOrder = _hexIdToPieceTypeMapping.Select(entry =>
            //    (float)_colorAndTypeToPieceIdMapping[
            //        (_isWhiteTurn ? IsPieceWhite(entry.Key) : !IsPieceWhite(entry.Key), entry.Value)
            //        ]).ToList();

            //sensor.AddObservation(allPiecesIdsInHexIdOrder);

            //foreach (var hexId in _hexIdToPieceTypeMapping.Keys)
            //{
            //    var hexPosition = _gameBoard.GetPositionByHexId(hexId);
            //    sensor.AddObservation(new Vector3(hexPosition.Item1, hexPosition.Item2, hexPosition.Item3));
            //}

            //var availableMoves = GetAddingMovesForPlayer(_isWhiteTurn);

            //foreach (var movingMove in GetMovingMovesForPlayer(_isWhiteTurn))
            //{
            //    availableMoves[movingMove.Key] = movingMove.Value;
            //}

            //for (int hexId = 1; hexId <= WhitePiecesBoundaryId; hexId++)
            //{
            //    var movesForHexId = availableMoves.Where(move => move.Key == (_isWhiteTurn ? hexId : hexId + WhitePiecesBoundaryId));
            //    if (movesForHexId.Count() > 0)
            //    {
            //        //var 
            //        //sensor.AddObservation(new Vector)
            //    }
            //}
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            int hexId = actionBuffers.DiscreteActions[0];

            if (!_gameEngine.IsWhiteTurn)
                hexId += HexIdToPiecePropertyMapper.WhitePiecesBoundaryId;

            int row = actionBuffers.DiscreteActions[1];
            int col = actionBuffers.DiscreteActions[1];
            int height = actionBuffers.DiscreteActions[1];

            if (_gameEngine.MakeMove(hexId, (row, col, height)))
            {
                if (_gameEngine.GameState == GameState.WhiteWon || _gameEngine.GameState == GameState.BlackWon)
                {
                    SetReward((!_gameEngine.IsWhiteTurn && _gameEngine.GameState == GameState.WhiteWon)
                        || (_gameEngine.IsWhiteTurn && _gameEngine.GameState == GameState.BlackWon) ? 1 : -1);
                    EndEpisode();
                }
                else if (_gameEngine.GameState == GameState.Draw)
                {
                    EndEpisode();
                }
            }
            RequestDecision();
        }
    }
}

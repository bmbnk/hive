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
            int[,] gameBoard = GameBoardRepresentations.GetFlatGameboard(_gameEngine.GameBoard);

            for (int row = 0; row < gameBoard.GetLength(0); row++)
            {
                var gameBoardRow = Enumerable.Range(0, gameBoard.GetLength(1))
                    .Select(col => (float)gameBoard[row, col])
                    .ToArray();

                sensor.AddObservation(gameBoardRow);
            }
        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {
            if (actionBuffers.DiscreteActions[0] != 0)
            {
                //Debug.Log("#####");
                //Debug.Log(actionBuffers.DiscreteActions[0]);
                //Debug.Log(actionBuffers.DiscreteActions[1]);
                //Debug.Log(actionBuffers.DiscreteActions[2]);
                //Debug.Log(actionBuffers.DiscreteActions[3]);
                //Debug.Log("#####");
                int hexId = actionBuffers.DiscreteActions[0];

                if (!_gameEngine.IsWhiteTurn)
                    hexId += HexIdToPiecePropertyMapper.WhitePiecesBoundaryId;

                int row = actionBuffers.DiscreteActions[1];
                int col = actionBuffers.DiscreteActions[2];
                int height = actionBuffers.DiscreteActions[3];

                if (_gameEngine.MakeMove(hexId, (row, col, height)))
                {
                    if (_gameEngine.GameState == GameState.WhiteWon || _gameEngine.GameState == GameState.BlackWon)
                    {
                        float reward = (!_gameEngine.IsWhiteTurn && _gameEngine.GameState == GameState.WhiteWon)
                            || (_gameEngine.IsWhiteTurn && _gameEngine.GameState == GameState.BlackWon) ? 1 : -1;
                        Debug.Log($"Reward for move ({hexId}, {row}, {col}, {height}) is {reward}");
                        SetReward(reward);
                        EndEpisode();
                        return;
                    }
                    else if (_gameEngine.GameState == GameState.Draw)
                    {
                        SetReward(0);
                        EndEpisode();
                        return;
                    }
                }
            }
            RequestDecision();
        }   
    }
}

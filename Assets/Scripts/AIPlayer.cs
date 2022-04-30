using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Hive
{
    public class AIPlayer : IPlayer
    {
        private GameManagerController _gameManager;

        private bool _isWhite;
        public bool IsWhite() => _isWhite;

        void Start()
        {
            GameObject gameManagerGameObject = GameObject.FindWithTag("GameManager");
            _gameManager = gameManagerGameObject.GetComponent<GameManagerController>();
        }

        public AIPlayer(bool isWhite)
        {
            _isWhite = isWhite;
        }

        public void RequestMove()
        {
            MakeMove();
        }

        public void MakeMove()
        {

        }
    }
}

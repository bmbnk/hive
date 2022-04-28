using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace Hive
{
    public class AIPlayer : Agent, IPlayer
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

        public override void OnEpisodeBegin()
        {
            //_gameManager.StartGame(true);
        }

        public override void CollectObservations(VectorSensor sensor)
        {

        }

        public override void OnActionReceived(ActionBuffers actionBuffers)
        {

        }
    }
}

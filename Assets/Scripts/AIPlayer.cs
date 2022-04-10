using UnityEngine;
namespace Hive
{
    public class AIPlayer : IPlayer
    {
        private bool _myTurn = false;
        private bool _isWhite;
        public bool IsWhite() => _isWhite;


        public AIPlayer(bool isWhite)
        {
            _isWhite = isWhite;
        }

        public void RequestMove()
        {
            _myTurn = true;
            MakeMove();
        }

        public void MakeMove()
        {

        }
    }
}

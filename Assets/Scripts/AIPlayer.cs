using UnityEngine;
namespace Hive
{
    public class AIPlayer : IPlayer
    {
        private bool _isWhite;
        public bool IsWhite() => _isWhite;


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

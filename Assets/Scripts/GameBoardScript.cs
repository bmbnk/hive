using UnityEngine;

namespace Hive
{
    public class GameBoardScript : MonoBehaviour
    {
        public const int GameBoardSize = 100;
        public const int CenterPositionX = 50;
        public const int CenterPositionY = 50;
        public int[,] gameBoard = new int[GameBoardSize, GameBoardSize];
    }
}

using System;
using System.Collections.Generic;

namespace Hive
{
    public static class HexIdToPiecePropertyMapper
    {
        private const int _whitePiecesBoundaryId = 11;

        public static int WhitePiecesBoundaryId => _whitePiecesBoundaryId;

        private static Dictionary<int, PieceType> _hexIdToPieceTypeMapping =
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

        private static Dictionary<PieceType, IPieceController> _pieceTypeToControllerMapping =
            new Dictionary<PieceType, IPieceController>()
            {
                {PieceType.BEE, new BeePieceController()},
                {PieceType.BEETLE, new BeetlePieceController()},
                {PieceType.SPIDER, new SpiderPieceController()},
                {PieceType.ANT, new AntPieceController()},
                {PieceType.GRASSHOPPER, new GrasshopperPieceController()},
            };

        private static Dictionary<(bool, PieceType), int> _colorAndTypeToPieceIdMapping =
            new Dictionary<(bool, PieceType), int>()
            {
                {(true, PieceType.BEE), 1},
                {(true, PieceType.BEETLE), 2},
                {(true, PieceType.SPIDER), 3},
                {(true, PieceType.ANT), 4},
                {(true, PieceType.GRASSHOPPER), 5},

                //{(false, PieceType.BEE), -1},
                //{(false, PieceType.BEETLE), -2},
                //{(false, PieceType.SPIDER), -3},
                //{(false, PieceType.ANT), -4},
                //{(false, PieceType.GRASSHOPPER), -5}

                {(false, PieceType.BEE), 6},
                {(false, PieceType.BEETLE), 7},
                {(false, PieceType.SPIDER), 8},
                {(false, PieceType.ANT), 9},
                {(false, PieceType.GRASSHOPPER), 10}
            };

        public static IPieceController GetPieceController(int hexId) => _pieceTypeToControllerMapping[_hexIdToPieceTypeMapping[hexId]];
        public static int GetPieceId(int hexId) => _colorAndTypeToPieceIdMapping[(IsPieceWhite(hexId), _hexIdToPieceTypeMapping[hexId])];

        // there should be some check and return default value
        public static PieceType GetPieceType(int hexId) => _hexIdToPieceTypeMapping[hexId];
        public static bool IsPieceWhite(int hexId) => hexId <= _whitePiecesBoundaryId;

    }
}

using System;
namespace Hive
{
    public interface IPlayer
    {
        bool IsWhite();
        void RequestMove();
    }
}

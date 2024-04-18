namespace Map
{
    static public class RoomUtilities
    {
        public delegate void Enter();
        public delegate void Exit();
        public delegate void AllEnemiesDead();
        public delegate void AllChestsOpen();

        static public Enter EnterEvents;
        static public Exit ExitEvents;
        static public AllEnemiesDead allEnemiesDeadEvents;
        static public AllChestsOpen allChestOpenEvents;

        static public RoomData roomData;
    }
}
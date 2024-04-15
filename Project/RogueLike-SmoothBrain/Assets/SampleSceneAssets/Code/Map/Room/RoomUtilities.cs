namespace Map
{
    static public class RoomUtilities
    {
        public delegate void Enter();
        public delegate void Exit();
        public delegate void AllEnemiesDead();

        static public Enter EnterEvents;
        static public Exit ExitEvents;
        static public AllEnemiesDead allEnemiesDeadEvents;

        static public RoomData roomData;
    }
}
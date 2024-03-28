static public class RoomUtilities
{
    public delegate void Enter(ref RoomData mapData);
    public delegate void Exit(ref RoomData mapData);

    static public Enter EnterEvents;
    static public Exit ExitEvents;

    static public RoomData roomData;
}
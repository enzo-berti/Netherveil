using System;
using System.Collections.Generic;

namespace Map
{
    static public class RoomUtilities
    {
        static public Action EarlyEnterEvents;
        static public Action EnterEvents;
        static public Action ExitEvents;
        static public Action allEnemiesDeadEvents;
        static public Action allChestOpenEvents;

        static public RoomData roomData;

        static public Dictionary<RoomType, int> nbRoomByType = new Dictionary<RoomType, int>
        {
            { RoomType.Lobby, 0 },
            { RoomType.Normal, 0 },
            { RoomType.Treasure, 0 },
            { RoomType.Challenge, 0 },
            { RoomType.Merchant, 0 },
            { RoomType.Secret, 0 },
            { RoomType.MiniBoss, 0 },
            { RoomType.Boss, 0 },
        };

        static public int NbRoom
        {
            get
            {
                int totalCount = 0;
                foreach (int count in nbRoomByType.Values)
                {
                    totalCount += count;
                }

                return totalCount;
            }
        }

        static public Dictionary<RoomType, int> nbEnterRoomByType = new Dictionary<RoomType, int>
        {
            { RoomType.Lobby, 0 },
            { RoomType.Normal, 0 },
            { RoomType.Treasure, 0 },
            { RoomType.Challenge, 0 },
            { RoomType.Merchant, 0 },
            { RoomType.Secret, 0 },
            { RoomType.MiniBoss, 0 },
            { RoomType.Boss, 0 },
        };

        static public int NbEnterRoom
        {
            get
            {
                int totalCount = 0;
                foreach (int count in nbEnterRoomByType.Values)
                {
                    totalCount += count;
                }

                return totalCount;
            }
        }
    }
}
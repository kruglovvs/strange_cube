// Copyright kruglov.valentine@gmail.com KruglovVS.

namespace Network.Clients
{
    public static class Topics
    {
        public static readonly string[] Subscribed = { "/Instructions", "/Boot" };
        public static readonly string[] Publishing = { "/GameData" };
    }
}
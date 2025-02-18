using ItemHandler;

namespace Muzzles
{
    public class AK_105_545x39_muzzle_brake_compensator : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Muzzle",//typus azonosito
                ItemName = "AK-105_5.45x39_muzzle_brake-compensator",//nev azonosito
                Description = "",

                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Recoil = -2,
                Accturacy = 10,
                Ergonomy = 1,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
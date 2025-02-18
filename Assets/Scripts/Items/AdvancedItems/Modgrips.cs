using ItemHandler;

namespace Modgrips
{
    public class KAC_vertical_foregrip : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Modgrips",//typus azonosito
                ItemName = "KAC_vertical_foregrip",//nev azonosito
                Description = "",

                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 6,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
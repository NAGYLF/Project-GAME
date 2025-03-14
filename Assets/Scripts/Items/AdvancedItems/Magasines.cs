using ItemHandler;

namespace Magasines
{
    public class AK_74_545x39_6L20_30_round_magasine : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Magasine",//typus azonosito
                ItemName = "AK-74_5.45x39_6L20_30-round_magasine",//nev azonosito
                Description = "",

                MagasineSize = 30,
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                Ergonomy = 2,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.

                SizeChanger = new SizeChanger(1,2,"D")
            };
        }
    }
    public class AK_74_545x39_6L31_60_round_magasine : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Magasine",//typus azonosito
                ItemName = "AK-74_5.45x39_6L31_60-round_magasine",//nev azonosito
                Description = "",

                MagasineSize = 60,
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                Ergonomy = -5,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.

                SizeChanger = new SizeChanger(1,2,"D")
            };
        }
    }
}
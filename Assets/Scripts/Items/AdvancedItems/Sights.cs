using ItemHandler;

namespace Sights
{
    public class Walther_MRS_reflex_sight : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Sight",//typus azonosito
                ItemName = "Walther_MRS_reflex_sight",//nev azonosito
                Description = "",

                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Accturacy = 15,
                Ergonomy = 2,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
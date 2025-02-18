using ItemHandler;

namespace Dustcovers
{
    public class AKS_74U_dust_cover : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Dustcover",//typus azonosito
                ItemName = "AKS-74U_dust_cover",//nev azonosito
                Description = "",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 4,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
    public class AKS_74U_Legal_Arsenal_Pilgrim_railed_dust_cover : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Dustcover",//typus azonosito
                ItemName = "AKS-74U_Legal_Arsenal_Pilgrim_railed_dust_cover",//nev azonosito
                Description = "",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 5,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
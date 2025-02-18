using ItemHandler;

namespace Grips
{
    public class AK_Zenit_RK_3_pistol_grip : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Grip",//typus azonosito
                ItemName = "AK_Zenit_RK-3_pistol_grip",//nev azonosito
                Description = "",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 5,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
    public class AKS_74U_bakelite_pistol_grip : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Grip",//typus azonosito
                ItemName = "AKS-74U_bakelite_pistol_grip",//nev azonosito
                Description = "",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 2,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
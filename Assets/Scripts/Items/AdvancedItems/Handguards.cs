using ItemHandler;

namespace Handguards
{
    public class AKS_74U_Wooden_handguard : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Handguard",//typus azonosito
                ItemName = "AKS-74U_Wooden_handguard",//nev azonosito
                Description = "",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 10,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
    public class AKS_74U_Zenit_B_11_handguard : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Handguard",//typus azonosito
                ItemName = "AKS-74U_Zenit_B-11_handguard",//nev azonosito
                Description = "",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 11,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
            };
        }
    }
}
using ItemHandler;

namespace Stoks
{
    public class AKS_74U_Skeletonized_Stock : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "Stok",//typus azonosito
                ItemName = "AKS-74U_Skeletonized_Stock",//nev azonosito
                Description = "",

                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Ergonomy = 3,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.

                SizeChanger = new SizeChanger(2,4,"R")
            };
        }
    }
}
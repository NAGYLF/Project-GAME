using ItemHandler;

namespace Vests
{
    public class TestVest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/TestVest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "TestVest",//nev azonosito
                Description = "Ez egy Admin Vest, statjai a leheto legjobbak",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/TestVest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }
}


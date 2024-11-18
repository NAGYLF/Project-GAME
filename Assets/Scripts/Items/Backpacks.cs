using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Backpacks
{
    public class TestBackpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/TestBackpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "TestBackpack",//nev azonosito
                Description = "Ez egy Admin backpack, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/TestBackpack")//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }
}

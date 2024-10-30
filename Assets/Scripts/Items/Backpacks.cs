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
                ImgPath = "Textures/TestBackpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "TestBackpack",//nev azonosito
                Description = "Ez egy Admin backpack, statjai a legjobbak",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 6,//az item slotokban elfoglalt magassaga
                Container = new Container()//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
                {
                    //nem kötelező tartalom, ha ures akkor egy ures prefabot hoz letre.
                    PrefabPath = "GameElements/ItemContainers/TestBackpack"
                }
            };

        }
    }
}

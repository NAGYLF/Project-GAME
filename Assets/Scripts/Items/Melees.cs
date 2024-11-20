using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Melees
{
    public class TestMelee : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Melees/TestMelee",//az item képe
                ItemType = "Melee",//typus azonosito
                ItemName = "TestMelee",//nev azonosito
                Description = "Ez egy Admin Melee, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga
            };

        }
    }
}
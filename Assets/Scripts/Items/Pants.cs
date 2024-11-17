using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Pants
{
    public class TestPant : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Pants/TestPant",//az item képe
                ItemType = "Pant",//typus azonosito
                ItemName = "TestPant",//nev azonosito
                Description = "Ez egy Admin Pant, statjai a legjobbak",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Boots
{
    public class TestBoots : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Boots/TestBoots",//az item képe
                ItemType = "Boots",//typus azonosito
                ItemName = "TestBoots",//nev azonosito
                Description = "Ez egy Admin Boots, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}
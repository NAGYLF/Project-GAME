using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Helmets
{
    public class TestHelmet : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Helmets/TestHelmet",//az item képe
                ItemType = "Helmet",//typus azonosito
                ItemName = "TestHelmet",//nev azonosito
                Description = "Ez egy Admin Helmet, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}
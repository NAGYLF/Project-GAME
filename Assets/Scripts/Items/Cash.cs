using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Cash
{
    public class Dollar_1 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Cash/dollar_1",//az item képe
                ItemType = "Cash",//typus azonosito
                ItemName = "Dollar_1",//nev azonosito
                Description = "Ez itt 1 Amerikai dollár",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga
                MaxStackSize = 100000,
                


            };

        }
    }

    
    
}

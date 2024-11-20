using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Ammunition
{
    public class Ammunition762x39FMJ : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Ammunition/7.62x39FMJ",
                ItemType = "Ammunition",
                ItemName = "7.62x39FMJ",
                Description = "Real Szovjet power from 1980, worldwide used ammunition. Somewhere just an ammo, somewhere money, but somewhere The mean of the life",
                SizeX = 1,
                SizeY = 1,
                MaxStackSize = 60,
            };
        }
    }
}

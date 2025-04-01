using System.Collections.Generic;
using ItemHandler;
using static MainData.Main;
using System;

namespace Items
{
    public class Magasine : IItemComponent
    {
        public string SystemName { get; private set; }
        public int MagasineSize { get; set; }
        public Stack<Ammunition> ContainedAmmo = new Stack<Ammunition>();
        public double Ergonomy { get; set; }
        public float Caliber { get; set; }
        public float CartridgeSize { get; set; }

        public Magasine(AdvancedItemStruct advancedItemStruct)
        {
            SystemName = advancedItemStruct.SystemName;
            MagasineSize = advancedItemStruct.MagasineSize;
            Ergonomy = advancedItemStruct.Ergonomy;
            Caliber = advancedItemStruct.Caliber;
            CartridgeSize = advancedItemStruct.CartridgeSize;
        }

        public IItemComponent CloneComponent()
        {
            throw new NotImplementedException();
        }
    }
}

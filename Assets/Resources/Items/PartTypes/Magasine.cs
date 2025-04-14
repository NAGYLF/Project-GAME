using System.Collections.Generic;
using ItemHandler;
using static MainData.Main;
using System;
using System.Linq;

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

        public Magasine()
        {
        }

        public IItemComponent CloneComponent()
        {
            Magasine mag = new Magasine()
            {
                SystemName = this.SystemName,
                MagasineSize = this.MagasineSize,
                Ergonomy = this.Ergonomy,
                Caliber = this.Caliber,
                CartridgeSize = this.CartridgeSize,
            };

            if (this.ContainedAmmo.Count > 0)
            {
                mag.ContainedAmmo = new Stack<Ammunition>((IEnumerable<Ammunition>)this.ContainedAmmo.Select(ammo => ammo.CloneComponent()).Reverse());
            }
            else
            {
                mag.ContainedAmmo = new Stack<Ammunition>();
            }

            return mag;
        }
    }
}

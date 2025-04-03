using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;
using System.Linq;

using static CharacterHand;

namespace Items
{
    public class WeaponBody : IItemComponent
    {
        public int ChamberSize { get; set; }
        public Stack<Ammunition> Chamber { get; set; }
        public double Spread { get; set; }
        public int Fpm { get; set; }
        public double Recoil { get; set; }
        public double Accturacy { get; set; }
        public double Range { get; set; }
        public double Ergonomy { get; set; }
        public float Caliber { get; set; }
        public float CartridgeSize { get; set; }
        private WeaponBody()
        {
          
        }
        public WeaponBody(AdvancedItemStruct advancedItemStruct)
        {
            ChamberSize = advancedItemStruct.MagasineSize;
            Chamber = new Stack<Ammunition>();
            Spread = advancedItemStruct.Spread;
            Fpm = advancedItemStruct.Fpm;
            Recoil = advancedItemStruct.Recoil;
            Accturacy = advancedItemStruct.Accturacy;
            Range = advancedItemStruct.Range;
            Ergonomy = advancedItemStruct.Ergonomy;
            Caliber = advancedItemStruct.Caliber;
            CartridgeSize = advancedItemStruct.CartridgeSize;
        }
        public IItemComponent CloneComponent()
        {
            return new WeaponBody()
            {
                ChamberSize = this.ChamberSize,
                Chamber = new Stack<Ammunition>((IEnumerable<Ammunition>)this.Chamber.Select(ammo => ammo.CloneComponent()).Reverse()),
                Spread = this.Spread,
                Fpm = this.Fpm,
                Recoil = this.Recoil,
                Accturacy = this.Accturacy,
                Range = this.Range,
                Ergonomy = this.Ergonomy,
                Caliber= this.Caliber,
                CartridgeSize= this.CartridgeSize,
            };
        }
    }
}

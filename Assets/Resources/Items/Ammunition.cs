using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;

namespace Items
{
    public class Ammunition : IItemComponent
    {
        public float Caliber { get; set; }
        public float Dmg { get; set; }
        public float APPower { get; set; }
        public float Mass { get; set; }
        public float MuzzleVelocity { get; set; }
        private Ammunition()
        {

        }
        public Ammunition(AdvancedItemStruct advancedItemStruct)
        {
            Caliber = advancedItemStruct.Caliber;
            Dmg = advancedItemStruct.Dmg;
            APPower = advancedItemStruct.APPower;
            Mass = advancedItemStruct.Mass;
            MuzzleVelocity = advancedItemStruct.MuzzleVelocity;
        }
        public IItemComponent CloneComponent()
        {
            return new Ammunition()
            {
                Caliber = this.Caliber,
                Dmg = this.Dmg,
                APPower = this.APPower,
                Mass = this.Mass,
                MuzzleVelocity = this.MuzzleVelocity,
            };
        }
    }
}

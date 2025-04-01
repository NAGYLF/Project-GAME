using ItemHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static MainData.Main;

namespace Items
{
    internal class HandGun : IItemComponent
    {
        public string ShootSoundPath { get; set; }
        public string ReloadSoundPath { get; set; }
        public string UnloadSoundPath { get; set; }
        public string ChamberSoundPath { get; set; }

        public string BulletTexturePath { get; set; }

        private AdvancedItem AdvancedItem { get; set; }
        public HandGun()
        {

        }
        public HandGun(MainItem mainItem)
        {
            ShootSoundPath = mainItem.ShootSoundPath;
            ReloadSoundPath = mainItem.ReloadSoundPath;
            UnloadSoundPath = mainItem.UnloadSoundPath;
            ChamberSoundPath = mainItem.ChamberSoundPath;

            BulletTexturePath = mainItem.BulletTexturePath;
        }

        public IItemComponent CloneComponent()
        {
            return new HandGun()
            {
                ShootSoundPath = this.ShootSoundPath,
                ReloadSoundPath = this.ShootSoundPath,
                UnloadSoundPath = this.UnloadSoundPath,
                ChamberSoundPath = this.ChamberSoundPath,

                BulletTexturePath = this.BulletTexturePath,
            };
        }

        public IEnumerator Control(InputFrameData input)
        {
            throw new NotImplementedException();
        }

        public void Inicialisation(AdvancedItem advancedItem)
        {
            AdvancedItem = advancedItem;
        }
    }
}

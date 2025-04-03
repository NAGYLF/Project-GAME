using ItemHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MainData.Main;

namespace Items
{
    internal class SMG : IItemComponent
    {
        public string ShootSoundPath { get; set; }
        public string ReloadSoundPath { get; set; }
        public string UnloadSoundPath { get; set; }
        public string ChamberSoundPath { get; set; }

        public string BulletTexturePath { get; set; }
        public SMG(MainItem mainItem)
        {
            ShootSoundPath = mainItem.ShootSoundPath;
            ReloadSoundPath = mainItem.ReloadSoundPath;
            UnloadSoundPath = mainItem.UnloadSoundPath;
            ChamberSoundPath = mainItem.ChamberSoundPath;

            BulletTexturePath = mainItem.BulletTexturePath;
        }

        public IItemComponent CloneComponent()
        {
            throw new NotImplementedException();
        }

        public IEnumerator Control(InputFrameData input)
        {
            throw new NotImplementedException();
        }

        public void Inicialisation(AdvancedItem advancedItem)
        {
            throw new NotImplementedException();
        }
    }
}

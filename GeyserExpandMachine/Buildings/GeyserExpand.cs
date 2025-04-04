using System;
using PeterHan.PLib.Core;

namespace GeyserExpandMachine.Buildings {
    public class GeyserExpand : KMonoBehaviour{
        public static readonly Tag ComponentTag = "GeyserExpand";
        private Geyser geyser;

        protected override void OnSpawn() {
            base.OnSpawn();

            #region 初始化/添加标签   
            
            var cell = Grid.PosToCell(this);
            var geyserFeature = Grid.Objects[cell, (int)ObjectLayer.Building];
            if (!geyserFeature.TryGetComponent(out Geyser geyserComponent)) {
                var errorMsg = $"未能找到 {geyserFeature} 的间歇泉组件";
                PUtil.LogError(errorMsg);
                throw new Exception(errorMsg);
            }
            geyser = geyserComponent;
            geyser.gameObject.AddTag(ComponentTag);

            #endregion
            
            ModData.Instance.GeyserExpands.Add(cell, this);
            
        }

        protected override void OnCleanUp() {
            base.OnCleanUp();
            geyser.gameObject.RemoveTag(ComponentTag);
        }
    }
}
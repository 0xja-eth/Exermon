
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	/// <summary>
	/// 特训药品槽项显示
	/// </summary
	public class PotionSlotItemDisplay : 
        DraggableItemDisplay<ExerProSlotPotion> {

        /// <summary>
        /// 常量定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text name;
        public Image icon; // 图片
		
        #region 数据控制

        /// <summary>
        /// 是否可以选中
        /// </summary>
        /// <returns>可否选中</returns>
        public override bool isEnabled() {
            return base.isEnabled() && item != null && item.isNullItem();
        }

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <returns></returns>
		public new PotionSlotDisplay getContainer() {
			return container as PotionSlotDisplay;
		}

		/// <summary>
		/// 是否为空物品
		/// </summary>
		/// <param name="item">物品</param>
		/// <returns></returns>
		public override bool isNullItem(ExerProSlotPotion item) {
            return base.isNullItem(item) || item.isNullItem();
        }

		/// <summary>
		/// 使用药水
		/// </summary>
		public void use() {
			var container = getContainer();
			if (container == null) return;
			container.use(this);
		}

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="slotPotion">运行时物品</param>
        protected override void drawExactlyItem(ExerProSlotPotion slotPotion) {
            base.drawExactlyItem(slotPotion);
            var potion = slotPotion.packPotion.item();

            icon.gameObject.SetActive(true);
            icon.overrideSprite = potion.icon;

            name.text = potion.name;
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            icon.gameObject.SetActive(false);
            name.text = "";
        }

        #endregion
    }
}
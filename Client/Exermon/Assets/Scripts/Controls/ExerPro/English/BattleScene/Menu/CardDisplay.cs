using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Battler;

	/// <summary>
	/// 卡牌显示
	/// </summary>
	public class CardDisplay : PackContItemDisplay
		<ExerProPackCard, ExerProCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Text cost;
		public Text description;

		public CardDragger dragger;

		#region 数据控制

		/// <summary>
		/// 获取容器
		/// </summary>
		/// <returns></returns>
		public new HandCardGroupDisplay getContainer() {
			return container as HandCardGroupDisplay;
		}

		/// <summary>
		/// 是否处于拖拽状态
		/// </summary>
		public bool isDragging() {
			return dragger.isDragging;
		}

		/// <summary>
		/// 能否进行拖拽操作
		/// </summary>
		public bool isDraggable() {
			var container = getContainer();
			if (container == null) return true;
			return !container.isRotating();
		}

		/// <summary>
		/// 设置拖拽状态
		/// </summary>
		public void setDragging(bool value) {
			var container = getContainer();
			if (container == null) return;
			container.isDragging = value;
		}

		/// <summary>
		/// 使用卡牌
		/// </summary>
		public void use(EnemyDisplay enemy) {
			var container = getContainer();
			if (container == null) return;
			container.use(this, enemy);
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制卡牌
		/// </summary>
		/// <param name="card"></param>
		protected override void drawItem(ExerProCard card) {
			cost.text = card.cost.ToString();
			description.text = card.description;
            name.text = card.name;
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			cost.text = description.text = "";
		}
        
        #endregion
    }

}
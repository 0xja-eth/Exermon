using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.Data.Loaders;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Battler;

	/// <summary>
	/// 卡牌显示
	/// </summary>
	public class CardDetail : PackContItemDetail
		<ExerProPackCard, ExerProCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Text name;
		public Image icon;

		public Text cost;
		public Text description;

		public Text character;
		public Text type;

		public Image cardSkin; // 皮肤
		public Image charFrame; // 性质框
		public Image typeIcon; // 类型图标

		public BaseWindow window;

		#region 开启/结束控制

		/// <summary>
		/// 开启视窗
		/// </summary>
		public override void startView() {
			base.startView();
			window?.startWindow();
		}

		/// <summary>
		/// 结束视窗
		/// </summary>
		public override void terminateView() {
			//base.terminateView();
			window?.terminateWindow();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 物品改变回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			if (isNullItem(item)) terminateView();
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 绘制卡牌
		/// </summary>
		/// <param name="card"></param>
		protected override void drawItem(ExerProCard card) {
			base.drawItem(card);

			drawSkin(card);
			drawCharacter(card);
			drawType(card);

			name.text = card.name;
			cost.text = card.cost.ToString();
			description.text = card.description;

			icon.gameObject.SetActive(true);
			icon.overrideSprite = card.icon;
		}

		/// <summary>
		/// 绘制皮肤
		/// </summary>
		/// <param name="card"></param>
		void drawSkin(ExerProCard card) {
			var skin = AssetLoader.generateSprite(card.skin);

			cardSkin.enabled = true;
			cardSkin.overrideSprite = skin;
		}

		/// <summary>
		/// 绘制性质
		/// </summary>
		/// <param name="card"></param>
		void drawCharacter(ExerProCard card) {
			if (card.character != "") {
				var charFrame = AssetLoader.generateSprite(card.charFrame);
				this.charFrame.gameObject.SetActive(true);
				this.charFrame.overrideSprite = charFrame;
				character.text = card.character;
			} else drawEmptyCharacter();
		}

		/// <summary>
		/// 绘制类型
		/// </summary>
		/// <param name="card"></param>
		void drawType(ExerProCard card) {
			if (this.typeIcon) {
				var typeIcon = AssetLoader.generateSprite(card.typeIcon);
				this.typeIcon.gameObject.SetActive(false);
				this.typeIcon.overrideSprite = typeIcon;
			}

			type.text = card.typeText();
		}

		/// <summary>
		/// 绘制空性质
		/// </summary>
		void drawEmptyCharacter() {
			charFrame.gameObject.SetActive(false);
			character.text = "";
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			name.text = cost.text = type.text = description.text = "";

			icon.gameObject.SetActive(false);

			cardSkin.enabled = false;
			if (typeIcon) typeIcon.gameObject.SetActive(false);

			drawEmptyCharacter();
		}

		#endregion
	}

}
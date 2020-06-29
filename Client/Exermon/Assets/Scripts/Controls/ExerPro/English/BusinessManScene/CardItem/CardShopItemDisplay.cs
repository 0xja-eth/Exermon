
using UnityEngine.UI;

using GameModule.Services;

using Core.Data.Loaders;
using ExerPro.EnglishModule.Data;

namespace UI.ExerPro.EnglishPro.BusinessManScene.Controls.CardItem {
    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class CardShopItemDisplay : ExerProShopItemDisplay<ExerProCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Text cost;
		public Text description;

		public Text character;
		public Text type;

		public Image cardSkin; // 皮肤
		public Image charFrame; // 性质框
		public Image typeIcon; // 类型图标

		/// <summary>
		/// 绘制基本信息
		/// </summary>
		/// <param name="item">物品</param>
		protected override void drawBaseInfo(ExerProCard card) {
			base.drawBaseInfo(card);

			drawSkin(card);
			drawCharacter(card);
			drawType(card);

			cost.text = card.cost.ToString();
			description.text = card.description;
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
			if (card._character != "") {
				var charFrame = AssetLoader.generateSprite(card.charFrame);
				this.charFrame.gameObject.SetActive(true);
				this.charFrame.overrideSprite = charFrame;
				character.text = card._character;
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

	}
}
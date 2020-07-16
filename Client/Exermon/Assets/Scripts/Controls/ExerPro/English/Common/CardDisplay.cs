
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.Common.Controls {

    /// <summary>
    /// 装备背包显示
    /// </summary>
    public class CardDisplay : SelectableItemDisplay<ExerProCard> {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string ShowAnimation = "CardShow";
		static readonly Color NormalColor = new Color(1, 1, 1, 1);
		static readonly Color LockedColor = new Color(1, 1, 1, 0.5f);

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Image icon;

        public Text name;
		public Text cost;
		public Text description;

		public Text character;
		public Text type;

		public Image cardSkin; // 皮肤
		public Image charFrame; // 性质框
		public Image typeIcon; // 类型图标

		public Image back;

		public GameObject content;

		public CanvasGroup canvasGroup;

		public AnimationView animation;

		/// <summary>
		/// 是否开启
		/// </summary>
		[SerializeField]
		bool _isOpen = true;
		public bool isOpen {
			get { return _isOpen; }
			set { _isOpen = value; requestRefresh(); }
		}

		/// <summary>
		/// 是否锁定
		/// </summary>
		[SerializeField]
		bool _isLocked = true;
		public bool isLocked {
			get { return _isLocked; }
			set { _isLocked = value; requestRefresh(); }
		}

		#region 数据控制

		/// <summary>
		/// 卡牌开启
		/// </summary>
		public void open() {
			isOpen = true;
		}

		/// <summary>
		/// 物品变化回调
		/// </summary>
		protected override void onItemChanged() {
			base.onItemChanged();
			isLocked = base.isNullItem(item);
			if (animation && !isLocked)
				animation.addAnimation(ShowAnimation);
		}

		/// <summary>
		/// 物品是否为空
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public override bool isNullItem(ExerProCard item) {
			return !isOpen || base.isNullItem(item);
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="card"></param>
		protected override void drawExactlyItem(ExerProCard card) {
			base.drawExactlyItem(card);

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

			if (back) {
				back.color = NormalColor;
				back.gameObject.SetActive(false);
			}
			content.SetActive(true);

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

			content.SetActive(false);

			if (back) {
				back.color = isLocked ? LockedColor : NormalColor;
				back.gameObject.SetActive(true);
			}

			cardSkin.enabled = false;
			if (typeIcon) typeIcon.gameObject.SetActive(false);

			drawEmptyCharacter();
		}

		#endregion

	}
}
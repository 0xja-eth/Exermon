
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;
using Core.UI;

using GameModule.Data;

using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.MapScene.Controls {

	/// <summary>
	/// 休息据点显示
	/// </summary>
	public class RestNodeDisplay : 
		ItemDetailDisplay<GameTip>, IPointerClickHandler {

		/// <summary>
		/// 常量定义
		/// </summary>
		const string TipFormat = "{0}：{1}";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public BaseWindow window;
		
		public Image background;
		public Text tips;

		public Texture2D[] textures;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float lastTime = 10; // 持续时间（秒）

		/// <summary>
		/// 内部变量定义
		/// </summary>
		float sumTime = 0;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		EnglishService engSer;

		#region 初始化

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			engSer = EnglishService.get();
		}

		#endregion

		#region 开启/结束控制

		/// <summary>
		/// 开启视窗
		/// </summary>
		public override void startView() {
			base.startView();
			sumTime = 0;
			window?.startWindow();
		}

		/// <summary>
		/// 结束视窗
		/// </summary>
		public override void terminateView() {
			//base.terminateView();
			window?.terminateWindow();
			engSer.exitNode(false);
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateTerminate();
		}

		/// <summary>
		/// 更新结束
		/// </summary>
		void updateTerminate() {
			if ((sumTime += Time.deltaTime) >= lastTime) 
				terminateView();
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 绘制确切物品
		/// </summary>
		/// <param name="item">题目</param>
		protected override void drawExactlyItem(GameTip item) {
			base.drawExactlyItem(item);
			drawBaseInfo(item);
		}

		/// <summary>
		/// 绘制基本信息
		/// </summary>
		/// <param name="item"></param>
		void drawBaseInfo(GameTip item) {
			background.overrideSprite = generateRandomBackground();
			tips.text = string.Format(TipFormat, item.name, item.description);
		}

		/// <summary>
		/// 生成随机背景
		/// </summary>
		/// <returns></returns>
		Sprite generateRandomBackground() {
			var index = Random.Range(0, textures.Length);
			return AssetLoader.generateSprite(textures[index]);
		}

		/// <summary>
		/// 绘制空物品
		/// </summary>
		protected override void drawEmptyItem() {
			base.drawEmptyItem();
			tips.text = "";
		}

		#endregion

		#region 事件处理

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerClick(PointerEventData eventData) {
			if (!window.isBusy()) terminateView();
		}

		#endregion

	}
}


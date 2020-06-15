
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

namespace UI.ExerPro.EnglishPro.BattleScene.Windows {

	using Controls.Menu;

	/// <summary>
	/// 抽牌窗口
	/// </summary>
	public class DrawWindow : BaseWindow {

		/// <summary>
		/// 文本常量定义
		/// </summary>
		const string CountFOrmat = "获得 {0} 次抽牌机会！";

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text count;

		public DrawCardGroupDisplay drawCardGroup;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		BattleScene scene;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		BattleService battleSer;
		EnglishService engSer;

		#region 初始化

		/// <summary>
		/// 初始化场景
		/// </summary>
		protected override void initializeScene() {
			base.initializeScene();
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			battleSer = BattleService.get();
			engSer = EnglishService.get();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			var cards = battleSer.drawnCards();

			count.text = string.Format(CountFOrmat, cards.Length);

			drawCardGroup.setItems(cards);
		}

		#endregion

		#region 流程控制

		#endregion

	}
}

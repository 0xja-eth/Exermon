
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

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public Text count, max;

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

		/// <summary>
		/// 抽取卡牌
		/// </summary>
		ExerProPackCard[] _drawnCards = null;

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

		#region 开启/关闭

		/// <summary>
		/// 关闭窗口
		/// </summary>
		public override void terminateWindow() {
			base.terminateWindow();
			_drawnCards = null;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取抽取的卡牌
		/// </summary>
		/// <returns></returns>
		public ExerProPackCard[] drawnCards() {
			return _drawnCards = _drawnCards ?? battleSer.drawnCards();
		}

		#endregion

		#region 画面绘制

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			var cards = drawnCards();

			var handGroup = battleSer.handGroup();

			count.text = handGroup.items.Count.ToString();
			max.text = handGroup.capacity.ToString();

			drawCardGroup.setItems(cards);
		}

		#endregion

		#region 流程控制

		#endregion

	}
}

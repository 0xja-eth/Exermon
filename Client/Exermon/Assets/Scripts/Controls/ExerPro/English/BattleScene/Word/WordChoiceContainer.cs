
using ExerPro.EnglishModule.Services;

using WordData = ExerPro.EnglishModule.Data.Word;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Word {

	using Windows;

	/// <summary>
	/// 单词选项组显示控件
	/// </summary>
	public class WordChoiceContainer : 
		SelectableContainerDisplay<string>, 
		IItemDisplay<WordData> {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public WordWindow window;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		EnglishService engSer;

		/// <summary>
		/// 显示答案
		/// </summary>
		bool _showAnswer = false;
		public bool showAnswer {
			get { return _showAnswer; }
			set {
				_showAnswer = value;
				requestRefresh();
			}
		}

		#region 初始化

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			engSer = EnglishService.get();
		}

		#endregion

		#region 接口实现

		WordData word;

		/// <summary>
		/// 获取单词
		/// </summary>
		/// <returns></returns>
		public WordData getItem() {
			return word;
		}

		/// <summary>
		/// 设置单词
		/// </summary>
		/// <param name="item"></param>
		/// <param name="force"></param>
		public void setItem(WordData item, bool _ = false) {
			// TODO:生成选项：后期需要移到后台执行
			setItems(engSer.generateWordChoices(word = item));
		}

		/// <summary>
		/// 开始视窗
		/// </summary>
		/// <param name="item"></param>
		public void startView(WordData item) {
			setItem(item);
		}

		#endregion

		#region 事件回调

		/// <summary>
		/// 点击回调
		/// </summary>
		/// <param name="index"></param>
		public override void onClick(int index) {
			base.onClick(index);
			window.answer(getItem(index));
		}

		#endregion
	}
}

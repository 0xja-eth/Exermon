
using ExerPro.EnglishModule.Services;
using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Windows;

	/// <summary>
	/// 单词选项组显示控件
	/// </summary>
	public class WordChoiceContainer : SelectableContainerDisplay<string>, IItemDisplay<Word> {

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
				_showAnswer = true;
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

		Word word;

		/// <summary>
		/// 获取单词
		/// </summary>
		/// <returns></returns>
		public Word getItem() {
			return word;
		}

		/// <summary>
		/// 设置单词
		/// </summary>
		/// <param name="item"></param>
		/// <param name="force"></param>
		public void setItem(Word item, bool _ = false) {
			// TODO:生成选项：后期需要移到后台执行
			setItems(engSer.generateWordChoices(word = item));
		}

		/// <summary>
		/// 开始视窗
		/// </summary>
		/// <param name="item"></param>
		public void startView(Word item) {
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

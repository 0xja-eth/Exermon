
using System;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Windows;

	/// <summary>
	/// 手牌控件
	/// </summary
	public class DrawCardGroupDisplay : 
		SelectableContainerDisplay<ExerProPackCard> {

		/// <summary>
		/// 外部组件定义
		/// </summary>
		public Text count, max;
		public Animation plusAni;

		public AnimationController controller;

		/// <summary>
		/// 场景组件引用
		/// </summary>
		BattleScene scene;

		/// <summary>
		/// 外部系统设置
		/// </summary>
		BattleService battleSer;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		ExerProCardHandGroup handGroup;

		int incCount = 0;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			controller.onPlayed = onCardShown;
			scene = SceneUtils.getCurrentScene<BattleScene>();
		}

		/// <summary>
		/// 初始化外部系统
		/// </summary>
		protected override void initializeSystems() {
			base.initializeSystems();
			battleSer = BattleService.get();

			handGroup = battleSer.handGroup();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 实际个数
		/// </summary>
		/// <returns></returns>
		int realCount() {
			return handGroup.items.Count;
		}

		/// <summary>
		/// 原始个数
		/// </summary>
		/// <returns></returns>
		int oriCount() {
			return realCount() - items.Count;
		}

		/// <summary>
		/// 当前显示个数 
		/// </summary>
		/// <returns></returns>
		int curCount() {
			return Math.Min(oriCount() + incCount, realCount());
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 创建所有子视图
		/// </summary>
		protected override void createSubViews() {
			base.createSubViews();
			addToController();
			incCount = 0;
		}

		/// <summary>
		/// 添加所有子视图到动画控制器
		/// </summary>
		void addToController() {
			foreach(var sub in subViews) {
				var display = sub as CardDisplay;
				if (display == null) continue;
				controller.add(display.animation);
				display.isOpen = false;
			}
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			max.text = "/" + handGroup.capacity;
		}

		/// <summary>
		/// 绘制数量
		/// </summary>
		/// <param name="count"></param>
		public void drawCount(int count) {
			this.count.text = count.ToString();
			plusAni.Play();
		}

		#endregion

		#region 流程控制

		/// <summary>
		/// 关闭
		/// </summary>
		public void next() {
			scene.play();
			//if (controller.isPlaying()) controller.stop();
			//else scene.play();
		}

		#endregion

		#region 回调控制

		/// <summary>
		/// 卡牌显示回调
		/// </summary>
		void onCardShown(AnimationView _) {
			incCount++; drawCount(curCount());
		}

		#endregion

	}
}
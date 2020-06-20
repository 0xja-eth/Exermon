
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

using ExerPro.EnglishModule.Data;
using ExerPro.EnglishModule.Services;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Battler {

	/// <summary>
	/// 行动管理组件
	/// </summary
	public class ActionManager : BaseView {

		/// <summary>
		/// 行动项
		/// </summary>
		public class Item {

			/// <summary>
			/// 使用者，目标
			/// </summary>
			public BattlerDisplay subject, object_;

			/// <summary>
			/// 行动
			/// </summary>
			public RuntimeAction action;

			/// <summary>
			/// 构造函数
			/// </summary>
			public Item(RuntimeAction action, 
				BattlerDisplay subject, BattlerDisplay object_) {
				this.action = action;
				this.subject = subject;
				this.object_ = object_;
			}

		}

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public BattleGround battleGround;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public AnimationClip defaultStartAni, defaultTargetAni;

		/// <summary>
		/// 行动项
		/// </summary>
		List<Item> actionItems = new List<Item>();

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updateActions();
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		void updateActions() {
			var tmp = actionItems.ToArray();
			foreach (var item in tmp)
				updateAction(item);
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		/// <param name="item"></param>
		void updateAction(Item item) {
			Debug.Log("updateAction: " + item?.subject?.name);

			var subject = item.subject;
			var target = item.object_;

			if (subject.isHit()) hit(item);
			if (target.isResult()) end(item);
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 增加行动
		/// </summary>
		/// <param name="action">行动</param>
		public void add(RuntimeAction action) {
			var subject = battleGround.getBattlerDisplay(action.subject);
			var object_ = battleGround.getBattlerDisplay(action.object_);
			var startAni = action.startAni ?? defaultStartAni;

			Debug.Log("add: " + subject + ", " + object_);

			actionItems.Add(new Item(action, subject, object_));

			subject.setupStartAni(startAni);
		}

		/// <summary>
		/// 获取目标
		/// </summary>
		/// <param name="battler"></param>
		/// <returns></returns>
		public BattlerDisplay getTarget(BattlerDisplay battler) {
			var item = actionItems.Find(item_ => item_.subject == battler);
			return item.object_;
		}

		/// <summary>
		/// 击中
		/// </summary>
		/// <param name="item"></param>
		void hit(Item item) {
			Debug.Log("Hit: " + item.subject.name);
			var target = item.object_;
			var ani = item.action.targetAni ?? defaultTargetAni;
			target.setupTargetAni(ani); target.hurt();
		}

		/// <summary>
		/// 结束行动
		/// </summary>
		/// <param name="item"></param>
		void end(Item item) {
			Debug.Log("End: " + item.subject.name);
			item.action.subject.processAction(item.action);
			actionItems.Remove(item);
		}

		#endregion

	}
}
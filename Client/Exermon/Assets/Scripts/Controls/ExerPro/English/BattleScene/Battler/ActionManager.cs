
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
			public BattlerDisplay subject;
			public BattlerDisplay[] objects;

			/// <summary>
			/// 行动
			/// </summary>
			public RuntimeAction action;

			/// <summary>
			/// 状态变量
			/// </summary>
			public bool isStarted = false;
			public bool isHit = false;
			public bool isEnd = false;

			/// <summary>
			/// 构造函数
			/// </summary>
			public Item(RuntimeAction action, 
				BattlerDisplay subject, BattlerDisplay[] objects) {
				this.action = action;
				this.subject = subject;
				this.objects = objects;
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
		Queue<Item> actionQueue = new Queue<Item>();

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
			//var tmp = actionItems.ToArray();
			//foreach (var item in tmp)
			if (actionQueue.Count <= 0) return;
			updateAction(actionQueue.Peek());
		}

		/// <summary>
		/// 更新行动
		/// </summary>
		/// <param name="item"></param>
		void updateAction(Item item) {
			Debug.Log("updateAction: " + item?.subject?.name);

			var subject = item.subject;
			var targets = item.objects;

			if (!item.isStarted) start(item);
			if (subject.isHit()) hit(item);
			foreach(var target in targets)
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
			var objects = battleGround.getBattlerDisplays(action.objects);
			var startAni = action.startAni ?? defaultStartAni;

			Debug.Log("Enqueue: " + subject + ", " + objects);

			actionQueue.Enqueue(new Item(action, subject, objects));

			subject.setupStartAni(startAni);
		}

		/// <summary>
		/// 获取目标
		/// </summary>
		/// <param name="battler"></param>
		/// <returns></returns>
		public BattlerDisplay getTarget(BattlerDisplay battler) {
			var targets = getTargets(battler);
			if (targets.Length <= 0) return null;
			return targets[0];
		}
		public BattlerDisplay[] getTargets(BattlerDisplay battler) {
			var item = actionQueue.Peek();
			if (item.subject == battler) return item.objects;
			return new BattlerDisplay[0];
		}

		/// <summary>
		/// 获取攻击占位
		/// </summary>
		/// <param name="battler">战斗者</param>
		/// <returns></returns>
		public Vector2 getAttackPosition(BattlerDisplay battler) {
			var targets = getTargets(battler);
			var cnt = targets.Length;

			if (cnt <= 0) return battler.getOriPosition();

			var res = targets[0].beAttackedPosition();
			for (int i = 1; i < cnt; ++i)
				res += targets[i].beAttackedPosition();

			return res / cnt;
		}

		/// <summary>
		/// 开始行动
		/// </summary>
		/// <param name="item"></param>
		void start(Item item) {
			var subject = item.subject;

			if (item.action.moveToTarget)
				subject.moveToAttack();
			else subject.attack();

			item.isStarted = true;
		}

		/// <summary>
		/// 击中
		/// </summary>
		/// <param name="item"></param>
		void hit(Item item) {
			if (item.isHit) return;

			Debug.Log("Hit: " + item.subject.name);
			var targets = item.objects;
			var ani = item.action.targetAni ?? defaultTargetAni;

			item.isHit = true;

			foreach (var target in targets) {
				target.setupTargetAni(ani); target.hurt();
			}
		}

		/// <summary>
		/// 结束行动
		/// </summary>
		/// <param name="item"></param>
		void end(Item item) {
			if (item.isEnd) return;

			Debug.Log("End: " + item.subject.name);
			item.action.subject.processAction(item.action);
			item.action.subject.onActionEnd(item.action);
			item.isEnd = true;

			actionQueue.Dequeue();
		}

		#endregion

	}
}
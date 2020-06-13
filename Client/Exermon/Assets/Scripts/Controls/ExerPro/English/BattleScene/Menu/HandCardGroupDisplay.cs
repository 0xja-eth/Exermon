﻿
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Core.Data.Loaders;
using Core.UI.Utils;

using GameModule.Services;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;
using UI.Common.Controls.AnimationSystem;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Battler;
	using Windows;

	/// <summary>
	/// 战场控件
	/// </summary
	public class HandCardGroupDisplay : 
		SelectableContainerDisplay<ExerProPackCard>,
		IPointerDownHandler, IPointerUpHandler {

		/// <summary>
		/// 常量定义
		/// </summary>
		const int MaxDeltaAngle = 12; // 最大相差角度
		const int MaxRotateAngle = 90; // 最大旋转角度

		const float RotateSpeed = 0.5f; // 旋转速度
		const float RotateThreshold = 2.5f; // 旋转阈值

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public MenuWindow menu; // 菜单窗口

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public Vector2 cardPivot = new Vector2(0.5f, -2);

		public AnimationView animation; // 动画

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool isDown = false;
		bool firstDown = false;
		Vector2 lastPos;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
			animation = animation ?? SceneUtils.get<AnimationView>(container);
		}

		#endregion

		#region 更新控制

		/// <summary>
		/// 更新
		/// </summary>
		protected override void update() {
			base.update();
			updatePointerMove();
		}

		/// <summary>
		/// 更新指针移动
		/// </summary>
		void updatePointerMove() {
			if (!isDown) return;
			var pos = getPointerPosition();

			if (firstDown) firstDown = false;
			else onPointerMove(pos - lastPos);
			lastPos = pos;
		}

		/// <summary>
		/// 获取指针位置
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		Vector2 getPointerPosition() {
			if (Input.touchCount > 0) 
				return Input.touches[0].position;
			return Input.mousePosition;
		}

		/// <summary>
		/// 是否处于旋转状态
		/// </summary>
		/// <returns></returns>
		public bool isRotating() {
			if (!isDown) return false;
			var pos = SceneUtils.screen2Local(lastPos, container);
			return pos.y <= container.rect.height / 2;
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 拖拽状态
		/// </summary>
		public bool isDragging { get; set; } = false;

		/// <summary>
		/// 最大角度范围
		/// </summary>
		/// <returns></returns>
		float maxAngleRange() { return MaxRotateAngle*2; }

		/// <summary>
		/// 角度范围
		/// </summary>
		/// <returns></returns>
		float angleRange() { return deltaAngle() * itemsCount(); }

		/// <summary>
		/// 卡牌角度增量
		/// </summary>
		/// <returns></returns>
		float deltaAngle() {
			return Mathf.Min(maxAngleRange() / itemsCount(), MaxDeltaAngle);
		}
		
		/// <summary>
		/// 使用卡牌
		/// </summary>
		public void use(CardDisplay cardDisplay, EnemyDisplay enemyDisplay) {
			menu.useCard(cardDisplay, enemyDisplay);
			removeItem(cardDisplay.getItem());
		}

		#endregion

		#region 界面控制

		/// <summary>
		/// 创建子视图组
		/// </summary>
		protected override void createSubViews() {
			base.createSubViews();
			rotateTo(deltaAngle() / 2);
		}

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		/// <param name="sub">子视图</param>
		/// <param name="index">索引</param>
		protected override void onSubViewCreated(
			SelectableItemDisplay<ExerProPackCard> sub, int index) {
			base.onSubViewCreated(sub, index);
			var rt = sub.transform as RectTransform;
			if (rt == null) return;
			setupSubViewPosition(rt, index);
		}

		/// <summary>
		/// 配置
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="index"></param>
		void setupSubViewPosition(RectTransform rt, int index) {
			var angle = index * deltaAngle();
			rt.pivot = cardPivot;
		}

		#endregion

		#region 动画控制

		/// <summary>
		/// 旋转到指定角度
		/// </summary>
		/// <param name="angle"></param>
		void rotateTo(float angle) {
			var range = angleRange();
			angle = Mathf.Clamp(angle, -range, range);
			animation.rotateTo(new Vector3(0, 0, angle), play: true);
		}

		/// <summary>
		/// 旋转到指定角度增量
		/// </summary>
		/// <param name="angle"></param>
		void rotateDelta(float delta) {
			var angle = container.localEulerAngles.z;
			rotateTo(angle + delta);
		}

		#endregion

		#region 事件控制

		/// <summary>
		/// 按下事件
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerDown(PointerEventData eventData) {
			firstDown = isDown = true;
		}

		/// <summary>
		/// 释放事件
		/// </summary>
		/// <param name="eventData"></param>
		public void OnPointerUp(PointerEventData eventData) {
			isDown = false;
			lastPos = default;
		}

		/// <summary>
		/// 指针移动回调
		/// </summary>
		/// <param name="delta">移动量</param>
		void onPointerMove(Vector2 delta) {
			if (isDragging) return;
			Debug.Log("onPointerMove: " + delta);
			if (delta.x >= RotateThreshold)
				rotateDelta(delta.x * RotateSpeed);
		}

		#endregion
	}
}
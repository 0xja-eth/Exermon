using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using ExerPro.EnglishModule.Data;

using Core.UI;
using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.BattleScene.Controls.Menu {

	using Battler;

	/// <summary>
	/// 卡牌拖拽器
	/// </summary>
	public class CardDragger : BaseView,
		IBeginDragHandler, IDragHandler, IEndDragHandler {

		/// <summary>
		/// 外部组件设置
		/// </summary>
		public CardDisplay cardDisplay;

		/// <summary>
		/// 预制件设定
		/// </summary>
		public GameObject arrowPerfab;

		/// <summary>
		/// 内部变量定义
		/// </summary>
		bool _isDragging = false;
		public bool isDragging {
			get { return _isDragging; }
			private set {
				_isDragging = value;
				cardDisplay.setDragging(value);
			}
		}

		GameObject arrow;
		Vector2 startPos;

		#region 初始化

		/// <summary>
		/// 初始化
		/// </summary>
		protected override void initializeOnce() {
			base.initializeOnce();
		}

		#endregion

		#region 数据控制

		/// <summary>
		/// 获取卡牌
		/// </summary>
		/// <returns></returns>
		public ExerProPackCard getCard() {
			return cardDisplay.getItem();
		}
		/*
		/// <summary>
		/// 能否进行拖拽操作（超出container范围后才可进行）
		/// </summary>
		/// <returns></returns>
		public bool isDraggable() {
			return cardDisplay.isDraggable();
		}
		*/
		/// <summary>
		/// 使用卡牌
		/// </summary>
		public void use(EnemyDisplay enemy = null) {
			cardDisplay.use(enemy);
		}

		/// <summary>
		/// 箭头父变换
		/// </summary>
		/// <returns></returns>
		RectTransform arrowParent() {
			var container = cardDisplay.getContainer();
			if (container == null) return transform as RectTransform;
			return container.transform as RectTransform;
		}

		#endregion

		#region 事件控制

		/// <summary>
		/// 开始拖拽
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnBeginDrag(PointerEventData data) {
			Debug.Log(name + ": OnBeginDrag: " + data);
			isDragging = true;
			cardDisplay.setupDetail();
			startPos = arrowLocalPos(data);
			if (arrow == null) createArrow();
		}

		/// <summary>
		/// 拖拽中
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnDrag(PointerEventData data) {
			var pos = arrowLocalPos(data);
			Debug.Log(name + ": OnDrag: " + data.position + ", pos: " + pos);
			updateArrowTransform(pos);
		}

		/// <summary>
		/// 结束拖拽
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnEndDrag(PointerEventData data) {
			Debug.Log(name + ": OnEndDrag: " + data);
			isDragging = false;
			cardDisplay.terminateDetail();
			destroyArrow();
		}

		/// <summary>
		/// 获取本地坐标
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		Vector2 arrowLocalPos(PointerEventData data) {
			return SceneUtils.screen2Local(data.position, arrowParent(), data.pressEventCamera);
		}

		#endregion

		#region 拖拽控制

		/// <summary>
		/// 更新箭头变换
		/// </summary>
		/// <param name="pos"></param>
		void updateArrowTransform(Vector2 pos) {
			var delta = pos - startPos;
			var rt = arrow.transform as RectTransform;

			float angle = 0;
			if (delta.x != 0) angle = Mathf.Atan(delta.y / delta.x) / Mathf.PI * 180;
			if (delta.x < 0) angle = angle + 180;

			var dist = delta.magnitude;

			var size = rt.sizeDelta; size.x = dist;
			var rot = rt.eulerAngles; rot.z = angle;

			rt.anchoredPosition = startPos;
			rt.sizeDelta = size;
			rt.eulerAngles = rot;
		}

		/// <summary>
		/// 创建箭头
		/// </summary>
		void createArrow() {
			var arrow = Instantiate(arrowPerfab, arrowParent());
			var rt = arrow.transform as RectTransform;
			if (rt == null) return;

			setupArrowRectTransform(rt);
			this.arrow = arrow;
		}

		/// <summary>
		/// 配置箭头变换
		/// </summary>
		/// <param name="rt"></param>
		void setupArrowRectTransform(RectTransform rt) {
			rt.pivot = new Vector2(0, 0.5f);
		}

		/// <summary>
		/// 摧毁箭头
		/// </summary>
		void destroyArrow() {
			if (arrow == null) return;
			Destroy(arrow); arrow = null;
		}

		#endregion
	}

}
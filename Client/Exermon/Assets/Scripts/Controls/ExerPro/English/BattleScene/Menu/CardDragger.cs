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

		public RectTransform arrowParent;

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
			arrowParent = arrowParent ?? (transform as RectTransform);
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

		#endregion

		#region 事件控制

		/// <summary>
		/// 开始拖拽
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnBeginDrag(PointerEventData data) {
			isDragging = true;
			startPos = data.position;
			cardDisplay.setupDetail();
			if (arrow == null) createArrow();
		}

		/// <summary>
		/// 拖拽中
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnDrag(PointerEventData data) {
			var pos = arrowLocalPos(data.position);
			updateArrowTransform(pos);
		}

		/// <summary>
		/// 结束拖拽
		/// </summary>
		/// <param name="data">事件数据</param>
		public void OnEndDrag(PointerEventData data) {
			isDragging = false;
			cardDisplay.terminateDetail();
			destroyArrow();
		}

		/// <summary>
		/// 获取本地坐标
		/// </summary>
		/// <param name="pos"></param>
		/// <returns></returns>
		Vector2 arrowLocalPos(Vector2 pos) {
			return SceneUtils.screen2Local(pos, arrowParent);
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

			var angle = Mathf.Atan(delta.y / delta.x) / Mathf.PI * 180;
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
			var arrow = Instantiate(arrowPerfab, arrowParent);
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
			rt.pivot = new Vector2(0.5f, 0);
		}

		/// <summary>
		/// 摧毁箭头
		/// </summary>
		void destroyArrow() {
			if (arrow != null) return;
			Destroy(arrow); arrow = null;
		}

		#endregion
	}

}
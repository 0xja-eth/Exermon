using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI.Utils;

using UI.Common.Controls.ItemDisplays;
using ExerPro.EnglishModule.Data;

using UI.Common.Controls.AnimationSystem;

/// <summary>
/// 地图场景控件
/// </summary>
namespace UI.ExerPro.EnglishPro.MapScene.Controls {

	/// <summary>
	/// 地图显示组件
	/// </summary>
	public class MapDisplay : SelectableContainerDisplay<ExerProMapNode>,
		IItemDisplay<ExerProRecord> {
		/// <summary>
		/// 外部组件定义
		/// </summary>
		public GameObject linePerfab;
		public Transform lineContainer;

		public MapNodeDetail nodeDetail;

		public StageRecordDisplay recordDisplay;

		public Image mapBackground; // 地图背景 

		public AnimationView animation;

		/// <summary>
		/// 外部变量定义
		/// </summary>
		public float xPadding = 64;
		public float yPadding = 64;
		public float xSpacing = 192;
		public float maxYSpacing = 128;

		/// <summary>
		/// 线段储存池
		/// </summary>
		List<GameObject> lines;

		#region 数据控制

		/// <summary>
		/// 获取详情控件
		/// </summary>
		/// <returns></returns>
		public override IItemDetailDisplay<ExerProMapNode> getItemDetail() {
			return nodeDetail;
		}

		/// <summary>
		/// 是否有效
		/// </summary>
		/// <returns></returns>
		public override bool isEnabled(ExerProMapNode item) {
			return (!recordDisplay || !recordDisplay.isMoving()) &&
				base.isEnabled(item) && item.status == (int)ExerProMapNode.Status.Active;
		}

		/// <summary>
		/// 获取当前据点显示控件
		/// </summary>
		/// <returns></returns>
		public MapNodeDisplay currentNode() {
			return subViews.Find(node => node.getItem().isCurrent()) as MapNodeDisplay;
		}

		#endregion

		#region 界面绘制

		/// <summary>
		/// 滚动到指定位置
		/// </summary>
		/// <param name="x"></param>
		void scrollTo(float x) {
			x = Mathf.Max(0, x);
			var oriY = animation.rectTransform.anchoredPosition.y;
			animation.moveTo(new Vector2(-x, oriY), play: true);
		}

		/// <summary>
		/// 滚动到当前据点
		/// </summary>
		public void scrollToCurrent() {
			Debug.Log("scrollToCurrent");
			if (animation.isPlaying()) return;
			var rt = currentNode().transform as RectTransform;
			scrollTo(rt.anchoredPosition.x - xSpacing);
		}

		/// <summary>
		/// 根据结点获取具体显示位置
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		Vector2 getPosition(ExerProMapNode node) {
			int x = node.xOrder, y = node.yOrder;
			var maxY = record.stage().steps[x];

			double posX = x * xSpacing + node.xOffset + xPadding, posY;

			if (maxY == 1) posY = 0;
			else {
				var height = container.rect.height - yPadding * 2;
				var ySpacing = height / (maxY - 1);

				ySpacing = Mathf.Min(ySpacing, maxYSpacing);

				posY = -(y - (maxY - 1) / 2.0) * ySpacing + node.yOffset;
			}

			return new Vector2((float)posX, (float)posY);
			//Debug.Log("getPosition: x: " + x + ", y:" + y + " => " + pos);
		}

		/// <summary>
		/// 子节点创建回调
		/// </summary>
		/// <param name="sub"></param>
		/// <param name="index"></param>
		protected override void onSubViewCreated(SelectableItemDisplay<ExerProMapNode> sub, int index) {
			base.onSubViewCreated(sub, index);
			var node = items[index]; // var node = sub.getItem();
			var pos = getPosition(node);
			var rt = sub.transform as RectTransform;
			rt.anchoredPosition = pos;
			drawLines(node);
		}

		/// <summary>
		/// 子视图创建回调
		/// </summary>
		protected override void createSubViews() {
			base.createSubViews();
			scrollToCurrent();
		}

		#region 线段绘制

		/// <summary>
		/// 绘制线段
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		void drawLine(Vector2 start, Vector2 end) {
			var go = Instantiate(linePerfab, lineContainer);
			var rt = go.transform as RectTransform;

			var delta = end - start;
			var angle = Mathf.Atan(delta.y / delta.x) / Mathf.PI * 180;
			var dist = delta.magnitude;

			var size = rt.sizeDelta; size.x = dist;
			var rot = rt.eulerAngles; rot.z = angle;

			rt.anchoredPosition = start;
			rt.sizeDelta = size;
			rt.eulerAngles = rot;
		}
		void drawLine(ExerProMapNode start, ExerProMapNode end) {
			drawLine(getPosition(start), getPosition(end));
		}

		/// <summary>
		/// 绘制多个线条
		/// </summary>
		/// <param name="node"></param>
		void drawLines(ExerProMapNode node) {
			var nexts = node.getNexts();
			foreach (var next in nexts) drawLine(node, next);
		}

		#endregion

		/// <summary>
		/// 更新内容尺寸
		/// </summary>
		void updateContentSize() {
			if (record == null) return;
			var len = record.stage().steps.Length - 1;
			SceneUtils.setRectWidth(container, xSpacing * len + xPadding * 2);
		}

		/// <summary>
		/// 刷新
		/// </summary>
		protected override void refresh() {
			base.refresh();
			updateContentSize();
		}

		#endregion

		#region 接口实现

		/// <summary>
		/// 关卡记录
		/// </summary>
		ExerProRecord record = null;

		/// <summary>
		/// 获取物品
		/// </summary>
		/// <returns></returns>

		public ExerProRecord getItem() {
			return record;
		}

		/// <summary>
		/// 设置物品
		/// </summary>
		/// <param name="item"></param>
		/// <param name="force"></param>
		public void setItem(ExerProRecord item, bool _ = false) {
			record = item; setItems(item.nodes);
		}

		/// <summary>
		/// 开启视图
		/// </summary>
		/// <param name="item"></param>
		public void startView(ExerProRecord item) {
			setItem(item, true);
		}

		#endregion

	}
}
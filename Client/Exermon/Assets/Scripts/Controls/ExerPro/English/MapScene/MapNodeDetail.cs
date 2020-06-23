
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;
using Core.UI.Utils;

using ExerPro.EnglishModule.Data;

using UI.Common.Windows;
using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 据点显示控件
    /// </summary
    public class MapNodeDetail :
        ItemDetailDisplay<ExerProMapNode> {

        /// <summary>
        /// 常量定义
        /// </summary>
        const string NameFormat = "据点类型：{0}";
        const string PosFormat = "坐标：{0}, {1}";
        const string QuesTypeFormat = "可能的题型：{0}";

        const float XOffset = 24;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ToggleWindow window;
        public Text name, description, pos, quesTypes;

        public Button confirm;

        #region 开启/结束控制

        /// <summary>
        /// 开启视窗
        /// </summary>
        public override void startView() {
            base.startView();
            window?.startWindow();
        }

        /// <summary>
        /// 结束视窗
        /// </summary>
        public override void terminateView() {
            //base.terminateView();
            window?.terminateWindow();
        }

        #endregion

        #region 界面控制

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(ExerProMapNode item) {
            base.drawExactlyItem(item);
            drawBaseInfo(item);

        }

        /// <summary>
        /// 绘制基本信息
        /// </summary>
        /// <param name="item"></param>
        void drawBaseInfo(ExerProMapNode item) {
            var type = item.type();

            name.text = string.Format(NameFormat, type.name);
            pos.text = string.Format(PosFormat, item.xOrder, item.yOrder);
            quesTypes.text = string.Format(QuesTypeFormat, type.quesTypes);

            description.text = type.description;

            confirm.interactable = item.status == (int)ExerProMapNode.Status.Active;
        }

        /// <summary>
        /// 绘制空物品
        /// </summary>
        protected override void drawEmptyItem() {
            base.drawEmptyItem();
            name.text = description.text = 
                pos.text = quesTypes.text = "";
            confirm.interactable = false;
        }
		
		/// <summary>
		/// 是否需要更新位置
		/// </summary>
		/// <returns></returns>
		protected override bool needUpdatePosition() {
			return true;
		}

		/// <summary>
		/// 根据ItemDisplay计算一个位置
		/// </summary>
		/// <param name="rt"></param>
		/// <returns></returns>
		protected override Vector2 calcPosition(RectTransform rt) {
            var rect = (transform as RectTransform).rect;
            var pos = Camera.main.WorldToScreenPoint(rt.position);
            int maxW = Screen.width / 2, maxH = Screen.height / 2;
            var offset = XOffset + rect.width / 2;

            Vector2 outPos = SceneUtils.screen2Local(
				pos, transform.parent as RectTransform);

            outPos.x += offset;
            
            if (outPos.x + rect.width / 2 > maxW)
                outPos.x = outPos.x - offset * 2;
            if (outPos.x - rect.width / 2 < -maxW)
                outPos.x = rect.width / 2 - maxW;

            if (outPos.y + rect.height / 2 > maxH)
                outPos.y = maxH - rect.height / 2;
            if (outPos.y - rect.height / 2 < -maxH)
                outPos.y = rect.height / 2 - maxH;

            return outPos;
        }
		
        #endregion

    }
}
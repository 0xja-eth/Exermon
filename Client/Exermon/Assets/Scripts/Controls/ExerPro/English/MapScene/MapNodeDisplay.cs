
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;
using Core.UI;

using ExerPro.EnglishModule.Data;

using UI.Common.Controls.ItemDisplays;

namespace UI.ExerPro.EnglishPro.MapScene.Controls {

    /// <summary>
    /// 据点显示控件
    /// </summary
    public class MapNodeDisplay :
        SelectableItemDisplay<ExerProMapNode> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Image icon;

        public GameObject activedFlag; // 激活标志
        public GameObject deactivedFlag; // 未激活标志
        public GameObject currentFlag; // 当前标志
        public GameObject passedFlag; // 经过标志
        public GameObject overFlag; // 结束标志
        
        #region 数据控制

        #endregion

        #region 界面控制

        #region 状态控制

        /// <summary>
        /// 刷新状态
        /// </summary>
        protected override void refreshStatus() {
            if (item != null) {
                var status = (ExerProMapNode.Status)item.status;

                refreshActiveStatus(status == ExerProMapNode.Status.Active);
                refreshDeactiveStatus(status == ExerProMapNode.Status.Deactive);
                refreshCurrentStatus(status == ExerProMapNode.Status.Current);
                refreshPassedStatus(status == ExerProMapNode.Status.Passed);
                refreshOverStatus(status == ExerProMapNode.Status.Over);
            }
            base.refreshStatus();
        }

        /// <summary>
        /// 刷新激活状态
        /// </summary>
        void refreshActiveStatus(bool actived) {
            if (activedFlag) activedFlag.SetActive(actived);
        }

        /// <summary>
        /// 刷新非激活状态
        /// </summary>
        void refreshDeactiveStatus(bool deactived) {
            if (deactivedFlag) deactivedFlag.SetActive(deactived);
        }

        /// <summary>
        /// 刷新当前状态
        /// </summary>
        void refreshCurrentStatus(bool current) {
            if (currentFlag) currentFlag.SetActive(current);
        }

        /// <summary>
        /// 刷新经过状态
        /// </summary>
        void refreshPassedStatus(bool passed) {
            if (passedFlag) passedFlag.SetActive(passed);
        }

        /// <summary>
        /// 刷新结束状态
        /// </summary>
        void refreshOverStatus(bool over) {
            if (overFlag) overFlag.SetActive(over);
        }

        #endregion

        /// <summary>
        /// 绘制确切物品
        /// </summary>
        /// <param name="item">题目</param>
        protected override void drawExactlyItem(ExerProMapNode item) {
            base.drawExactlyItem(item);
            var type = item.type();

            icon.gameObject.SetActive(true);
            icon.overrideSprite = AssetLoader.generateSprite(type.icon);
        }

        /// <summary>
        /// 清除物品
        /// </summary>
        protected override void drawEmptyItem() {
            icon.gameObject.SetActive(false);
        }

        #endregion

    }
}
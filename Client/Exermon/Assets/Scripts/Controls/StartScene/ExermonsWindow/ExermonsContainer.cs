
using System;

using UnityEngine;
using UnityEngine.UI;

using GameModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.StartScene.Controls.Exermon {

    using ExermonModule.Data;

    /// <summary>
    /// 艾瑟萌卡片容器
    /// </summary>
    public class ExermonsContainer : ContainerDisplay<Exermon> {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string SelectionFormat = "<size=80><color=#ffea92>{0}</color></size>/{1}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public ExermonDetail detail; // 帮助界面

        public Text selectionDisplay; // 选择数目显示

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public bool inStartScene = false; // 是否在开始场景中

        /// <summary>
        /// 内部变量声明
        /// </summary>
        string[] enames; // 每一个艾瑟萌的昵称

        #region 数据控制

        /// <summary>
        /// 获取物品帮助组件
        /// </summary>
        /// <returns>帮助组件</returns>
        protected override IItemDetailDisplay<Exermon> getItemDetail() {
            return detail;
        }

        /// <summary>
        /// 最大选中数量
        /// </summary>
        /// <returns>最大选中数</returns>
        public override int maxCheckCount() {
            return DataService.get().staticData.configure.maxSubject;
        }

        #region 昵称控制

        /// <summary>
        /// 配置艾瑟萌名称
        /// </summary>
        void setupEnames() {
            var cnt = itemsCount();
            enames = new string[cnt];
            for (int i = 0; i < cnt; i++)
                enames[i] = items[i].name;
        }

        /// <summary>
        /// 更改昵称
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="name">名称</param>
        public void changeNickname(int index, string name) {
            if (enames == null) return;
            enames[index] = name;
        }

        /// <summary>
        /// 获取昵称
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>昵称</returns>
        public string getNickname(int index) {
            return enames[index];
        }

        #endregion

        /// <summary>
        /// 物品变更回调
        /// </summary>
        protected override void onItemsChanged() {
            base.onItemsChanged();
            setupEnames();
        }

        /// <summary>
        /// 校验选择数目
        /// </summary>
        /// <returns>选择数目是否正确</returns>
        public bool checkSelection() {
            return checkedIndices.Count == maxCheckCount();
        }

        /// <summary>
        /// 获取选择结果
        /// </summary>
        /// <param name="eids">选择的艾瑟萌ID数组</param>
        /// <param name="enames">选择的艾瑟萌昵称数组</param>
        /// <returns>选中数目</returns>
        public int getResult(out int[] eids, out string[] enames) {
            var cnt = checkedIndices.Count;
            eids = new int[cnt];
            enames = new string[cnt];
            for (int i = 0; i < cnt; ++i)
                eids[i] = checkedIndices[i];

            Array.Sort(eids);

            for (int i = 0; i < cnt; ++i) {
                eids[i] = items[eids[i]].getID();
                enames[i] = this.enames[eids[i]];
            }
            return cnt;
        }

        /// <summary>
        /// 获取选择结果
        /// </summary>
        /// <returns>选择的所有艾瑟萌</returns>
        public Exermon[] getResult() {
            var cnt = checkedIndices.Count;
            var eids = new int[cnt];
            var items = new Exermon[cnt];
            for (int i = 0; i < cnt; ++i) 
                eids[i] = checkedIndices[i];
            Array.Sort(eids);
            for (int i = 0; i < cnt; ++i) 
                items[i] = this.items[eids[i]];
            return items;
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制选择数量
        /// </summary>
        void refreshSelectionDisplay() {
            selectionDisplay.text = string.Format(
                SelectionFormat, checkedIndices.Count, maxCheckCount());
        }

        /// <summary>
        /// 刷新视窗
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshSelectionDisplay();
        }

        #endregion

    }
}
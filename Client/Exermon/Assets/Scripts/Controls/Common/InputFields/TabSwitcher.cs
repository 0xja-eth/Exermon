using System;
using System.Collections.Generic;

using UnityEngine;

using Core.UI;

namespace UI.Common.Controls.InputFields {

    /// <summary>
    /// Tab 切换器
    /// </summary>
    public class TabSwitcher : GroupView<BaseInputField> {

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BaseInputField default_ = null; // 默认选择

        bool next = false, prev = false;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeEvery() {
            base.initializeEvery();
            select(default_);
        }

        #endregion

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            updateTab();
        }

        /// <summary>
        /// 更新Tab键
        /// </summary>
        void updateTab() {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                var prev = Input.GetKey(KeyCode.LeftShift) ||
                    Input.GetKey(KeyCode.RightShift);
                if (prev) prevSelect();
                else nextSelect(); 

            }
        }

        /// <summary>
        /// 清空Tab按键状态
        /// </summary>
        void clearTabStatus() {
            prev = next = false;
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 选择一项
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="input">输入控件</param>
        public void select(int index) {
            if (index < 0 || index >= subViewsCount()) return;
            subViews[index].activate();
            /*
            for (int i = 0; i < subViewsCount(); ++i)
                if (i == index) subViews[i].activate();
                //else subViews[i].blurColor();
            */
        }
        public void select(BaseInputField input) {
            select(getIndex(input));
        }

        /// <summary>
        /// 获取当前选择
        /// </summary>
        /// <returns></returns>
        public int currentIndex() {
            return subViews.FindIndex(v => v.isRealFocused());
        }

        /// <summary>
        /// 选择下一个
        /// </summary>
        public void nextSelect() {
            var index = currentIndex();
            Debug.Log("current: " + index);
            index = (index + 1) % subViewsCount();
            select(index);
        }

        /// <summary>
        /// 选择上一个
        /// </summary>
        public void prevSelect() {
            var index = currentIndex();
            var cnt = subViewsCount();
            Debug.Log("current: " + index);
            index = (index - 1 + cnt) % cnt;
            select(index);
        }

        #endregion
    }
}

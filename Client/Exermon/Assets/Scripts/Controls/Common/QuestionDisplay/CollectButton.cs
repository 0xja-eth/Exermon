
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using QuestionModule.Data;
using RecordModule.Data;
using RecordModule.Services;

using UI.Common.Controls.ItemDisplays;

namespace UI.Common.Controls.QuestionDisplay {

    /// <summary>
    /// 收藏按钮显示
    /// </summary>
    public class CollectButton : SelectableItemDisplay<Question> {

        /// <summary>
        /// 常量设置
        /// </summary>
        const string CollectedText = "已收藏";
        const string UncollectedText = "收藏";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text collectText;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool collected;

        /// <summary>
        /// 收藏操作回调
        /// </summary>
        public UnityAction collectCallback { get; set; } = null;

        /// <summary>
        /// 外部系统设置
        /// </summary>
        RecordService recordSer;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            registerUpdateLayout(transform as RectTransform);
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            recordSer = RecordService.get();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 是否 Checked
        /// </summary>
        /// <returns></returns>
        public override bool isChecked() {
            return isChecked(item);
        }
        public bool isChecked(Question item) {
            if (item == null) return false;
            return recordSer.recordData.isQuestionCollected(item.getID());
        }

        /// <summary>
        /// 选中
        /// </summary>
        public override void check() {
            if (!isCheckable()) return;
            collect();
        }

        /// <summary>
        /// 取消选中
        /// </summary>
        public override void uncheck() {
            if (!isUncheckable()) return;
            collect();
        }

        /// <summary>
        /// 反转
        /// </summary>
        public override void toggle() {
            Debug.Log("toggle");
            if (isChecked()) uncheck();
            else check();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制项目
        /// </summary>
        /// <param name="item"></param>
        protected override void drawExactlyItem(Question item) {
            base.drawExactlyItem(item);
            if (isChecked(item)) collectText.text = CollectedText;
            else collectText.text = UncollectedText;
        }

        #endregion

        #region 流程控制
        
        /// <summary>
        /// 收藏题目
        /// </summary>
        public void collect() {
            if (item == null) return;
            recordSer.collect(item.getID(), onCollected);
        }

        /// <summary>
        /// 收藏回调
        /// </summary>
        void onCollected() {
            requestRefresh(true);
            collectCallback?.Invoke();
        }

        #endregion
    }
}
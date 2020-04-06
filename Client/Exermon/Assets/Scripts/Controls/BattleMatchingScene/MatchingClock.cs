using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

namespace UI.BattleMatchingScene.Controls {

    /// <summary>
    /// 匹配用时显示
    /// </summary>
    public class MatchingClock : BaseView{

        /// <summary>
        /// 常量定义
        /// </summary>
        public const string TimeFormat = "匹配用时\n{0}";

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public Text time;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        bool timming = false;
        DateTime startTime;

        #region 更新控制

        /// <summary>
        /// 更新
        /// </summary>
        protected override void update() {
            base.update();
            if (timming) refresh();
        }

        #endregion

        #region 数据控制
        
        /// <summary>
        /// 开始
        /// </summary>
        public void startTimer() {
            timming = true;
            startTime = DateTime.Now;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void stopTimer() {
            timming = false;
        }

        #endregion

        #region 界面绘制
        
        /// <summary>
        /// 刷新
        /// </summary>
        protected override void refresh() {
            base.refresh();
            var now = DateTime.Now;
            var delta = now - startTime;
            time.text = SceneUtils.time2Str(delta);
        }

        /// <summary>
        /// 清除
        /// </summary>
        protected override void clear() {
            base.clear();
            time.text = "";
        }

        #endregion
    }
}

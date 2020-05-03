using UnityEngine.UI;

using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using GameModule.Data;

using UI.RecordScene.Windows;

namespace UI.RecordScene.Controls {

    /// <summary>
    /// 科目控制组
    /// </summary>
    public class SubjectTabController : TabView<RecordWindow> {

        /// <summary>
        /// 玩家
        /// </summary>
        Subject[] subjects = null;

        #region 初始化

        /// <summary>
        /// 配置组件
        /// </summary>
        /// <param name="objs">对象数组</param>
        public void configure(Player player) {
            base.configure(); setPlayer(player);
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 设置玩家
        /// </summary>
        /// <param name="player">玩家</param>
        public void setPlayer(Player player) {
            setSubjects(player?.subjects());
        }

        /// <summary>
        /// 设置科目
        /// </summary>
        /// <param name="subjects">科目集</param>
        public void setSubjects(Subject[] subjects) {
            this.subjects = subjects;
            requestRefresh();
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void showContent(RecordWindow content, int index) {
            content.setSubjectId(subjects[index].getID());
        }

        /// <summary>
        /// 隐藏内容页
        /// </summary>
        /// <param name="content"></param>
        protected override void hideContent(RecordWindow content, int index) {
            content.terminateView();
        }

        /// <summary>
        /// 刷新子视图
        /// </summary>
        /// <param name="sub">子视图</param>
        protected override void refreshSubView(Toggle sub, int index) {
            var trans = sub.transform;
            var txt = SceneUtils.find<Text>(trans, "Label");
            txt.text = (subjects == null ? "" : subjects[index].name);
        }

        #endregion

    }
}
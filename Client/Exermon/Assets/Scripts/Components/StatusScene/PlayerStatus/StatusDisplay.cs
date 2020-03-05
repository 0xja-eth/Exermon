
using UnityEngine;
/// <summary>
/// 状态场景
/// </summary>
namespace StatusScene {

    /// <summary>
    /// 玩家状态
    /// </summary>
    namespace PlayerStatus {

        /// <summary>
        /// 状态窗口人物视图
        /// </summary>
        public class StatusDisplay : ItemDisplay<Player> {

            /// <summary>
            /// 外部组件设置
            /// </summary>
            public BaseInfoDisplay baseInfo; // 人物基本信息视图
            public DetailInfoDisplay detailInfo; // 人物详细信息视图

            public InfoEditWindow infoEditWindow;

            public GameObject confirm, equip, dequip;

            /// <summary>
            /// 内部变量设置
            /// </summary>

            #region 启动视窗

            /// <summary>
            /// 开始视窗
            /// </summary>
            public override void startView() {
                base.startView();
                switchButtons();
            }

            /// <summary>
            /// 切换按钮
            /// </summary>
            void switchButtons() {
                confirm.SetActive(true);
                equip.SetActive(false);
                dequip.SetActive(false);
            }
            
            #endregion

            #region 数据控制

            /// <summary>
            /// 物品改变回调
            /// </summary>
            protected override void onItemChanged() {
                base.onItemChanged();

                baseInfo.setItem(item);
                detailInfo.setItem(item);
            }

            #endregion

            #region 界面绘制

            /// <summary>
            /// 刷新视图
            /// </summary>
            protected override void refresh() {
                base.clear();

                baseInfo.requestRefresh(true);
                detailInfo.requestRefresh(true);
            }

            #endregion

        }
    }
}
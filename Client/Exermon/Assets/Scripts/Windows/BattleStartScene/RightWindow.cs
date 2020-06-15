
using Core.UI;
using Core.UI.Utils;

using PlayerModule.Data;

using PlayerModule.Services;

using UI.Common.Controls.ItemDisplays;

/// <summary>
/// 对战开始场景窗口
/// </summary>
namespace UI.BattleStartScene.Windows {
    
    using MainContent = Controls.Right.MainContent.MainContent;
    using ItemContent = Controls.Right.ItemContent.ItemContent;
    using RuleContent = Controls.Right.RuleContent.RuleContent;
    using RankContent = Controls.Right.RankContent.RankContent;

    /// <summary>
    /// 右窗口
    /// </summary>
    public class RightWindow : BaseWindow {

        /// <summary>
        /// 文本常量定义
        /// </summary>

        /// <summary>
        /// 界面类型
        /// </summary>
        public enum Type {
            Normal = -1, // 普通界面
            Item = 0, // 更换物资
            Rule = 1, // 查看规则
            Rank = 2 // 赛季榜位
        }

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public MainContent mainContent;
        public ItemContent itemContent;
        public RuleContent ruleContent;
        public RankContent rankContent;

        /// <summary>ck
        /// 界面类型
        /// </summary>
        private Type _type = Type.Normal;
        public Type type {
            get { return _type; }
            set {
                if (_type == value) return;
                _type = value; requestRefresh();
            }
        }

        /// <summary>
        /// 场景组件引用
        /// </summary>
        BattleStartScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        PlayerService playerSer = null;

        #region 初始化

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = SceneUtils.getCurrentScene<BattleStartScene>();
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            playerSer = PlayerService.get();
        }

        #endregion

        #region 开启控制
        
        #endregion

        #region 数据控制

        #endregion

        #region 界面控制

        /// <summary>
        /// 刷新类型
        /// </summary>
        void refreshType() {
            clearContents();
            ItemDetailDisplay<Player> content = mainContent;
            switch (type) {
                //case Type.Normal: content = mainContent; break;
                case Type.Item: content = itemContent; break;
                case Type.Rule: content = ruleContent; break;
                case Type.Rank: content = rankContent; break;
            }
            showContent(content);
        }

        /// <summary>
        /// 显示内容页
        /// </summary>
        /// <param name="content">内容页</param>
        void showContent(ItemDetailDisplay<Player> content) {
            content.startView(playerSer.player);
        }

        /// <summary>
        /// 刷新窗口
        /// </summary>
        protected override void refresh() {
            base.refresh();
            refreshType();
        }

        /// <summary>
        /// 清除内容页
        /// </summary>
        void clearContents() {
            mainContent.terminateView();
            itemContent.terminateView();
            ruleContent.terminateView();
            rankContent.terminateView();
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            clearContents();
        }

        #endregion
        
    }
}
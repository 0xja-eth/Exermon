
using UnityEngine;
using UnityEngine.UI;

using Core.Data.Loaders;

using Core.UI;
using Core.UI.Utils;

using Core.Systems;

using BattleModule.Services;

using BattleModule.Data;

using UI.Common.Controls.ParamDisplays;
using UI.Common.Controls.RadarDisplay;

using UI.BattleMatchingScene.Controls;

/// <summary>
/// 对战结算场景
/// </summary>
namespace UI.BattleResultScene {

    /// <summary>
    /// 对战匹配场景
    /// </summary>
    public class BattleResultScene : BaseScene {

        /// <summary>
        /// 文本定义
        /// </summary>

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public BattlerDisplay selfBattler;
        public BattlerDisplay oppoBattler;

        public MultParamsDisplay resultDetail, scoreDetail;
        public RadarDiagram radar;

        public Image radarTexture1, radarTexture2;

        /// <summary>
        /// 外部变量设置
        /// </summary>
        public Texture2D[] judgeTextures;

        /// <summary>
        /// 内部系统声明
        /// </summary>
        BattleService battleSer;

        /// <summary>
        /// 内部变量定义
        /// </summary>
        RuntimeBattle battle;
        BattlePlayer result;

        #region 初始化

        /// <summary>
        /// 场景名
        /// </summary>
        /// <returns>场景名</returns>
        public override string sceneName() {
            return SceneSystem.Scene.BattleResultScene;
        }

        /// <summary>
        /// 初始化外部系统
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            battleSer = BattleService.get();
        }

        /// <summary>
        /// 开始
        /// </summary>
        protected override void start() {
            base.start();
            battle = battleSer.battle;
            result = battle.record.self();

            setupBattlers();
            setupResultDetail();
            setupScoreDetail();
            setupRadarData();
        }

        #endregion

        #region 场景控制

        /// <summary>
        /// 配置对战玩家
        /// </summary>
        void setupBattlers() {
            selfBattler.setItem(battle.self());
            oppoBattler.setItem(battle.oppo());
        }

        /// <summary>
        /// 配置结果详情
        /// </summary>
        void setupResultDetail() {
            resultDetail.setValue(result, "result");
        }

        /// <summary>
        /// 配置分数详情
        /// </summary>
        void setupScoreDetail() {
            scoreDetail.setValue(result, "score");
        }

        /// <summary>
        /// 配置雷达图数据
        /// </summary>
        void setupRadarData() {
            radar.setValues(result);
            setupRadarTexture();
        }

        /// <summary>
        /// 配置雷达图纹理
        /// </summary>
        void setupRadarTexture() {
            var judge = result.judge();
            var jid = judge.id - 1;
            if (judgeTextures.Length > jid) {
                var texture = judgeTextures[jid];
                var sprite = AssetLoader.generateSprite(texture);
                radarTexture1.gameObject.SetActive(true);
                radarTexture2.gameObject.SetActive(true);
                radarTexture1.overrideSprite = sprite;
                radarTexture2.overrideSprite = sprite;
            } else {
                radarTexture1.gameObject.SetActive(false);
                radarTexture2.gameObject.SetActive(false);
            }
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 返回对战大厅
        /// </summary>
        public void back() {
            popScene();
        }

        /// <summary>
        /// 查看解析
        /// </summary>
        public void showResult() {
            sceneSys.pushScene(SceneSystem.Scene.BattleAnswerScene);
        }

        #region 状态回调处理

        #endregion

        #endregion

    }
}
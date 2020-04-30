using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Core.Systems;
using Core.UI;
using Core.UI.Utils;

using GameModule.Data;
using GameModule.Services;
using PlayerModule.Services;
using RecordModule.Services;

using UI.Common.Controls.InputFields;

namespace UI.MainScene.Windows {

    /// <summary>
    /// 刷题配置窗口
    /// </summary>
    public class ExerciseConfigWindow : BaseWindow {

        /// <summary>
        /// 常量定义
        /// </summary>
        const int MinExerciseCount = 1;
        const int MaxExerciseCount = 10;

        /// <summary>
        /// 外部组件设置
        /// </summary>
        public DropdownField subjectSelect, modeSelect;
        public ValueSlideField countSlider;

        public Text time;

        /// <summary>
        /// 场景组件引用
        /// </summary>
        MainScene scene;

        /// <summary>
        /// 外部系统引用
        /// </summary>
        GameSystem gameSys = null;
        SceneSystem sceneSys = null;
        DataService dataSer = null;
        PlayerService playerSer = null;
        RecordService recordSer = null;

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void initializeOnce() {
            base.initializeOnce();
            configureSubViews();
        }

        /// <summary>
        /// 初始化场景
        /// </summary>
        protected override void initializeScene() {
            scene = (MainScene)SceneUtils.getSceneObject("Scene");
        }

        /// <summary>
        /// 初始化系统/服务
        /// </summary>
        protected override void initializeSystems() {
            base.initializeSystems();
            gameSys = GameSystem.get();
            sceneSys = SceneSystem.get();
            playerSer = PlayerService.get();
            dataSer = DataService.get();
            recordSer = RecordService.get();
        }

        /// <summary>
        /// 配置子组件
        /// </summary>
        void configureSubViews() {
            var player = playerSer.player;
            var mmodes = dataSer.staticData.configure.exerciseGenTypes;
            var subjects = player.subjects();

            countSlider.onChanged = (_) => updateTimeEval();
            subjectSelect.onChanged = (_) => updateTimeEval();

            countSlider.configure(MinExerciseCount, MaxExerciseCount);
            subjectSelect.configure(subjects);
            modeSelect.configure(mmodes);

            clear();
        }

        #endregion

        #region 数据控制

        /// <summary>
        /// 更新用时预估
        /// </summary>
        void updateTimeEval() {
            var player = playerSer.player;
            var sid = subjectSelect.getValueId();
            var count = (int)countSlider.getValue();

            var star = CalcService.MaxStarCalc.calc(player, sid);
            drawTime(star.stdTime * count / 2);
        }

        #endregion

        #region 界面绘制

        /// <summary>
        /// 绘制预计时间
        /// </summary>
        void drawTime(int seconds) {
            time.text = SceneUtils.time2Str(seconds);
        }

        /// <summary>
        /// 清除窗口
        /// </summary>
        protected override void clear() {
            base.clear();
            time.text = "";
        }

        #endregion

        #region 流程控制

        /// <summary>
        /// 开始刷题
        /// </summary>
        public void onSubmit() {
            var sid = subjectSelect.getValueId();
            var mode = modeSelect.getValueId();
            var count = (int)countSlider.getValue();
            recordSer.generateExercise(sid, mode, count, () =>
                sceneSys.pushScene(SceneSystem.Scene.ExerciseScene));
        }

        #endregion
    }

}

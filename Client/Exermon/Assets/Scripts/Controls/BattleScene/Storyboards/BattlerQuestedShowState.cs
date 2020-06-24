using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

using Core.UI;
using Core.UI.Utils;

namespace UI.BattleScene.Controls.Storyboards {

    /// <summary>
    /// 战斗答题显示状态
    /// </summary>
    public class BattlerQuestedShowState : BaseStateBehaviour {

        /// <summary>
        /// 内部变量
        /// </summary>
        BattlerQuestedStoryboard status;

        GameObject humanBlackLine, exermonBlackLine;
        GameObject lightEffect;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="go"></param>
        protected override void setup(GameObject go) {
            base.setup(go);
            status = SceneUtils.get<BattlerQuestedStoryboard>(go);

            humanBlackLine = status ? status.humanBlackLine : null;
            exermonBlackLine = status ? status.exermonBlackLine : null;
            lightEffect = status ? status.lightEffect : null;
        }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected override void onStatusEnter() {
            if (!status) return;

            var correct = status.isCorrect();

            if (humanBlackLine) humanBlackLine.SetActive(!correct);
            if (exermonBlackLine) exermonBlackLine.SetActive(!correct);
            if (lightEffect) lightEffect.SetActive(correct);
        }

        /// <summary>
        /// 状态结束
        /// </summary>
        protected override void onStatusExit() {
            if (humanBlackLine) humanBlackLine.SetActive(false);
            if (exermonBlackLine) exermonBlackLine.SetActive(false);
            if (lightEffect) lightEffect.SetActive(false);
        }

    }

}

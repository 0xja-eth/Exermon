using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameModule.Services.Test;
using ExerPro.EnglishModule.Data;
using LitJson;
using Core.Data.Loaders;

namespace Tests {
    public class ActionResultGenerateTests {
        /// <summary>
        /// 内部变量
        /// </summary>
        protected RuntimeAction runtimeAction;
        protected RuntimeActor battler;

        /// <summary>
        /// 回血测试数据
        /// </summary>
        protected const int  hp = 100;
        protected const int recoverHp = 100;
        protected const int recoverRate = 50;

        #region 测试函数
        // A Test behaves as an ordinary method
        [Test] //通过
        public void generate_Check() {
            generateTest();

            var generation = CalcServiceTest.ExerProActionResultGenerator.generate(runtimeAction);

            var expected = recoverHp + hp * 1.0 * recoverRate / 100;
            Assert.IsNotEmpty(generation);
            Assert.AreEqual(expected, generation[0].hpRecover);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ActionResultGenerateTestsWithEnumeratorPasses() {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        #endregion

        #region 辅助函数
        protected void generateTest() {
            var actionData = new JsonData();

            battler = new RuntimeActor();
            var battlerData = new JsonData();
            battlerData["hp"] = hp;
            ExerProPotionSlot slot = new ExerProPotionSlot(battler);
            battlerData["potion_slot"] = DataLoader.convert(slot);
            battler.load(battlerData);

            actionData["subject"] = DataLoader.convert(battler);
            RuntimeActor[] objects = new RuntimeActor[1] { battler };
            actionData["objects"] = DataLoader.convert(objects);

            ExerProEffectData effect = new ExerProEffectData();
            var effectData = new JsonData();
            effectData["code"] = (int)ExerProEffectData.Code.Recover;
            int []params_ = new int[2] { recoverHp, recoverRate };
            effectData["params"] = DataLoader.convert(params_);
            effect.load(effectData);
            ExerProEffectData[] effects = new ExerProEffectData[1] { effect };
            actionData["effects"] = DataLoader.convert(effects);

            runtimeAction = new RuntimeAction(battler, battler, effects);

        }
        #endregion
    }
}

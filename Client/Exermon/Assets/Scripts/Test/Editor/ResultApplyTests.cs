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
    public class ResultApplyCalcTests : ActionResultGenerateTests {

        #region 测试函数
        // A Test behaves as an ordinary method
        [Test] //未通过，测试数据无法加载
        public void apply_Check() {
            generateTest();
            var generationResults = CalcServiceTest.ExerProActionResultGenerator.generate(runtimeAction);
            Assert.IsNotEmpty(generationResults);

            var ihp = battler.hp;
            CalcServiceTest.ResultApplyCalc.apply(battler, generationResults[0]);

            var expected = hp + recoverHp + hp * 1.0 * recoverRate / 100;
            Assert.AreEqual(expected, battler.hp);
        }


        #endregion

        #region 辅助函数
        #endregion
    }
}

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameModule.Services.Test;
using ExerPro.EnglishModule.Data;
using LitJson;
using Core.Data.Loaders;

namespace Tests
{
    public class EnemyNextCalTests
    {
        /// <summary>
        /// 内部变量
        /// </summary>
        ExerProEnemy.Action[] actions;
        ExerProEnemy enemy;

        #region 测试函数
        // A Test behaves as an ordinary method
        [Test]
        public void calc_Check()
        {
            generateTestData();
            int length = actions.Length;
            for(int i = 0; i < length; i++) {
                ExerProEnemy.Action action;
                CalcServiceTest.EnemyNextCalc.calc(i, enemy, out action);
                Assert.AreEqual(action.id, i);
            }
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator EnemyNextCalTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        #endregion

        #region 辅助函数

        void generateTestData() {
            actions = new ExerProEnemy.Action[10];
            for(int i = 0; i < actions.Length; i++) {
                actions[i] = new ExerProEnemy.Action();
                var data = new JsonData();
                data["id"] = i;
                data["type"] = 1;
                int[] rounds = new int[1] { i };
                data["rounds"] = DataLoader.convert(rounds);
                data["rate"] = 1;
                actions[i].load(data);
            }

            enemy = new ExerProEnemy();
            var data1 = new JsonData();
            data1["actions"] = DataLoader.convert(actions);
            enemy.load(data1);
            
        }
        #endregion
    }
}

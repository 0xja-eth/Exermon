using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameModule.Services.Test;
using ExerPro.EnglishModule.Data;

namespace Tests
{
    public class BattleEnemiesGeneratorTests {
        /// <summary>
        /// 测试变量
        /// </summary>
        ExerProMapNode.Type[] nodeTypes = {
            ExerProMapNode.Type.Enemy, ExerProMapNode.Type.Elite, ExerProMapNode.Type.Boss};
        const int DefaultMaxNumber = 10;
        int[] maxNumber = new int []{ 2, 5, 10 };

        /// <summary>
        /// 结果变量
        /// </summary>
        ExerProEnemy.EnemyType[] enemyTypes = {
            ExerProEnemy.EnemyType.Normal, ExerProEnemy.EnemyType.Elite, ExerProEnemy.EnemyType.Boss};

        #region 测试函数

        /// <summary>
        /// 通用的检查接口
        /// </summary>
        /// <param name="index">输入类型</param>
        /// <param name="maxNumber">输入最大敌人数</param>
        void generate_Check(int index, int maxNumber = DefaultMaxNumber) {
            if (nodeTypes.Length <= index) {
                Assert.Fail("测试类型不正确");
            }
            var generation = CalcServiceTest.BattleEnemiesGenerator.generate(nodeTypes[index], maxNumber);
            foreach (var enemy in generation) {
                Assert.AreEqual((int)enemyTypes[index], enemy.type_);
            }
            var posList = CalcServiceTest.BattleEnemiesGenerator.posList;
            posList.Sort();

            int maxPos = CalcServiceTest.BattleEnemiesGenerator.MaxEnemyCols
                * CalcServiceTest.BattleEnemiesGenerator.MaxEnemyRows - 1;
            int last = -1;
            foreach(var pos in posList) {
                if (pos == last)
                    Assert.Fail("产生相同位置！");
                last = pos;
                if(pos < 0 || pos > maxPos)
                    Assert.Fail("产生错误位置！");
            }
        }

        [Test]
        public void generate_Normal() {
            generate_Check(0);
        }

        [Test]
        public void generate_Elite() {
            generate_Check(1);

        }

        [Test]//发现bug
        public void generate_Boss()
        {
            generate_Check(2);
        }

        [Test]
        public void generate_MaxNumber() {
            foreach (var max in maxNumber)
                generate_Check(0, max);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator EnemyGenerateTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        #endregion

    }
}

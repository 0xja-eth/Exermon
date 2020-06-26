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
    public class RewardGeneratorTests : ExerProItemGeneratorTests {
        /// <summary>
        /// 内部变量
        /// </summary>
        const int EpicIndex = 3;

        #region 测试函数
        // A Test behaves as an ordinary method
        [Test]
        public void getGoldReward_Enemy() {
            ExerProMapNode.Type type = ExerProMapNode.Type.Enemy;
            for (int i = 0; i < 100; i++) {
                int layer = Random.Range(1, 10);
                int enmey = Random.Range(1, 10);
                var gold = CalcServiceTest.RewardGenerator.getGoldReward(type, layer, enmey);

                int expectedMin = layer * 0 + enmey * 15;
                int expectedMax = layer * 5 + enmey * 20;

                Assert.AreEqual(expectedMin, gold, expectedMax - expectedMin);
            }
        }

        [Test]
        public void getGoldReward_Boss() {
            for (int i = 0; i < 100; i++) {
                int stage = Random.Range(1, 10);
                var gold = CalcServiceTest.RewardGenerator.getBossGoldReward(stage);

                int expectedMin = stage * 35;
                int expectedMax = stage * 40;

                Assert.AreEqual(expectedMin, gold, expectedMax - expectedMin);
            }
        }

        [Test]
        public void getCardReward_Boss() {
            generateTestData();
            for (int i = 0; i < 100; i++) {
                var cards = CalcServiceTest.RewardGenerator.getCardRewards(ExerProMapNode.Type.Boss);
                int epicNumber = cards.FindAll(e => e.starId == EpicIndex).Count;
                Assert.AreEqual(cards.Count, epicNumber);
            }
        }

        [Test]
        public void generateScore_Check() {
            for (int i = 0; i < 100; i++) {
                int accEnemy = Random.Range(50, 10000), accBoss = Random.Range(5, 100), 
                    boss = Random.Range(1, 10),
                     accPerfect = Random.Range(1, 10), gold = Random.Range(0, 1000), 
                     cards = Random.Range(1, 1000),
                     accLayer = Random.Range(1, 1000), layer = Random.Range(1, 100);
                bool isPerfect = Random.Range(0, 1) == 0;

                var score = CalcServiceTest.RewardGenerator.generateScore(accEnemy, accBoss, boss, accPerfect,
                    isPerfect, gold, cards, accLayer, layer);

                int expected = accEnemy * 2 + (accBoss + boss) * 50 + (accPerfect + (isPerfect ? 1 : 0)) * 50
                    + gold / 100 * 25 + (cards > 30 ? (cards - 30) / 5 * 10 : 0) + (accLayer + layer) * 5;

                Assert.AreEqual(expected, score);
            }
        }
        #endregion

        #region 辅助函数

        #endregion
    }
}

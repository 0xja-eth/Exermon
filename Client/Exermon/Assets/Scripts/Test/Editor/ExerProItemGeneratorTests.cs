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
    public class ExerProItemGeneratorTests {
        /// <summary>
        /// 内部变量
        /// 测试数据
        /// </summary>
        const int totalItemNumber = 10;
        protected List<ExerProCard> cards;
        protected List<ExerProPotion> potions;


        #region 测试函数
        // A Test behaves as an ordinary method
        [Test]
        public void generateRandomItem_Normal() {
            generateTestData();
            var itemGenerator = CalcServiceTest.ExerProItemGenerator.itemGenerator;

            var card = itemGenerator.generateRandomItem(1, 0) as ExerProCard;


            Assert.IsNotNull(card);
            int expectedStar = 1;
            Assert.AreEqual(expectedStar, card.starId);
        }

        [Test]
        public void generateRandomItem_Rare() {
            generateTestData();
            var itemGenerator = CalcServiceTest.ExerProItemGenerator.itemGenerator;

            var card = itemGenerator.generateRandomItem(0, 1) as ExerProCard;


            Assert.IsNotNull(card);
            int expectedStar = 2;
            Assert.AreEqual(expectedStar, card.starId);
        }

        [Test]
        public void generateRandomItem_Epic() {
            generateTestData();
            var itemGenerator = CalcServiceTest.ExerProItemGenerator.itemGenerator;

            var card = itemGenerator.generateRandomItem(0, 0) as ExerProCard;


            Assert.IsNotNull(card);
            int expectedStar = 3;
            Assert.AreEqual(expectedStar, card.starId);
        }

        [Test]
        public void generateRandomItem_RandomCards() {
            generateTestData();
            List<ExerProCard> cards = new List<ExerProCard>();
            for (int i = 0; i < 100; i++) {
                var temp = CalcServiceTest.ExerProItemGenerator.generateCards(5, 0.33, 0.33);
                cards.AddRange(temp);
            }
            int nSum = cards.FindAll(e => e.starId == 1).Count;
            int rSum = cards.FindAll(e => e.starId == 2).Count;
            int eSum = cards.FindAll(e => e.starId == 3).Count;
            double nRate = nSum * 1.0 / cards.Count;
            double rRate = rSum * 1.0 / cards.Count;
            double eRate = eSum * 1.0 / cards.Count;

            double expected = 0.33;
            Assert.AreEqual(expected, nRate, 0.1);
            Assert.AreEqual(expected, rRate, 0.1);
            Assert.AreEqual(expected, eRate, 0.1);

        }

        [Test]
        public void generateRandomItem_RandomPotions() {
            generateTestData();
            List<ExerProPotion> potions = new List<ExerProPotion>();
            for (int i = 0; i < 100; i++) {
                var temp = CalcServiceTest.ExerProItemGenerator.generatePotions(5, 0.5, 0.5);
                potions.AddRange(temp);
            }
            int nSum = potions.FindAll(e => e.starId == 1).Count;
            int rSum = potions.FindAll(e => e.starId == 2).Count;
            double nRate = nSum * 1.0 / potions.Count;
            double rRate = rSum * 1.0 / potions.Count;

            double expected = 0.5;
            Assert.AreEqual(expected, nRate, 0.1);
            Assert.AreEqual(expected, rRate, 0.1);

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExerProItemGeneratorTestsWithEnumeratorPasses() {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        #endregion

        #region 辅助函数
        protected void generateTestData() {
            cards = new List<ExerProCard>();
            for(int i = 0; i < totalItemNumber; i++) {
                ExerProCard card = new ExerProCard();
                var data = new JsonData();
                data["star_id"] = Mathf.Clamp(i / 3 + 1, 1, 3);

                card.load(data);
                cards.Add(card);
            }
            potions = new List<ExerProPotion>();
            for (int i = 0; i < totalItemNumber; i++) {
                ExerProPotion potion = new ExerProPotion();
                var data = new JsonData();
                data["star_id"] = Mathf.Clamp(i / 3 + 1, 1, 2);

                potion.load(data);
                potions.Add(potion);
            }
            CalcServiceTest.ExerProItemGenerator.cards = cards.ToArray();
            CalcServiceTest.ExerProItemGenerator.potions = potions.ToArray();
        }

        #endregion
    }
}

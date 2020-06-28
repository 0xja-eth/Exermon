using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using GameModule.Services.Test;
using ExerPro.EnglishModule.Data;
using LitJson;
using System.IO;

namespace Tests
{
    public class WordChoicesGeneratorTests
    {
        /// <summary>
        /// 内部变量
        /// </summary>
        const string smallData = "F:/words.json";//2000
        const string bigData = "F:/Bigwords.json";//5000
        const string soBigData = "F:/SoBigwords.json";//10000

        List<Word> words = null;

        List<Word> testWords = null;
        List<string> testEnglish = new List<string> { "forbid", "give", "hold", "lay", "leaf" };
        List<string> testChinese = new List<string> { "禁止;不许", "给，给予，赠给，引起",
            "拿，抱，握;举行;容纳;持续n.(用单数)掌握，把握" , "放，摆;使处于某种状态;产卵",
            "n.[C](树)叶;(书刊等的)张(包括正反两面，相当于twopages)" };

        Dictionary<string, List<string>> calDistanceTests = null;

        #region 测试函数

        [Test] //通过
        public void datasetTest() {
            // Use the Assert class to test conditions
            if (words == null)
                ReadData(soBigData);
            if (testWords == null)
                generateTestWords();
            Assert.Pass();

        }
        // A Test behaves as an ordinary method
        [Test] //通过
        public void generate_FunctionTest()
        {
            // Use the Assert class to test conditions
            if (words == null)
                ReadData();
            if (testWords == null)
                generateTestWords();
            foreach (var word in testWords) {
                var generateList = CalcServiceTest.WordChoicesGenerator.generate(word, words);
                Assert.Contains(word.chinese, generateList);
            }

        }

        [Test] //通过
        public void generate_BigDataset() {
            // Use the Assert class to test conditions
            if (words == null)
                ReadData(bigData);
            if (testWords == null)
                generateTestWords();
            foreach (var word in testWords) {
                var generateList = CalcServiceTest.WordChoicesGenerator.generate(word, words);
                Assert.Contains(word.chinese, generateList);
            }

        }

        [Test] //通过
        public void generate_SoBigDataset() {
            // Use the Assert class to test conditions
            if (words == null)
                ReadData(soBigData);
            if (testWords == null)
                generateTestWords();
            foreach (var word in testWords) {
                var generateList = CalcServiceTest.WordChoicesGenerator.generate(word, words);
                Assert.Contains(word.chinese, generateList);
            }
        }

        [Test]
        public void calcDistance_Check() {
            generateDisTests();
            foreach(var tests in calDistanceTests) {
                List<double> distances = new List<double>();
                foreach(var word in tests.Value) {
                    var dis = CalcServiceTest.WordChoicesGenerator.calcDistance(tests.Key, word);
                    distances.Add(dis);
                }
                for(int i = 1; i < distances.Count; i++) {
                    if (distances[i] <= distances[i - 1])
                        Assert.Fail();
                }
                Assert.Pass();
            }
        }


        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator WordChoicesGeneratorTestsWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
        #endregion

        #region 辅助函数
        //读取数据
        public void ReadData(string FileName = smallData) {
            StreamReader json = File.OpenText(FileName);
            string input = json.ReadToEnd();
            Dictionary<string, List<Dictionary<string, object>>> jsonObject
                = JsonMapper.ToObject<Dictionary<string, List<Dictionary<string, object>>>>(input);
            var a = jsonObject["objects"];
            List<string> engs = new List<string>();
            List<string> chis = new List<string>();

            foreach (var i in a) {
                object eng;
                var ok = i.TryGetValue("英文", out eng);
                if (ok)
                    engs.Add(eng as string);

                object chi;
                ok = i.TryGetValue("中文", out chi);
                if (ok)
                    chis.Add(chi as string);
            }

            words = new List<Word>();
            for (int i = 0; i < Mathf.Max(engs.Count, chis.Count); i++) {
                var data = new JsonData();
                data["english"] = engs[i];
                data["chinese"] = chis[i];

                var word = new Word();
                word.load(data);
                words.Add(word);
            }
        }

        void generateTestWords() {
            testWords = new List<Word>();
            for(int i = 0; i < testEnglish.Count; i++) {
                Word word = new Word();
                var data = new JsonData();
                data["english"] = testEnglish[i];
                data["chinese"] = testChinese[i];
            }
        }
        #endregion

        void generateDisTests() {
            calDistanceTests = new Dictionary<string, List<string>>();
            //calDistanceTests.Add("a", new List<string> { "aa", "aaa", "aaaa" });
            //calDistanceTests.Add("a", new List<string> { "ab", "abc", "abcd" });
            //calDistanceTests.Add("a", new List<string> { "b", "bc", "bcd" });
            //calDistanceTests.Add("Africa", new List<string> { "African", "America", "American" });
            calDistanceTests.Add("do", new List<string> { "dig", "leaf", "chairman" });

        }
    }
}

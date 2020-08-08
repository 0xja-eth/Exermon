import random, time, re

from english_pro_module.models import CorrectType

from ..exception import GameException, ErrorType
from ..view_utils import Common as ViewUtils


# ===================================================
# 生成当前轮单词
# ===================================================
class NewWordsGenerator:

    WORD_CNT = 50
    NEW_WORD_PERCENT = 0.2

    # 从上一轮单词列表中拿出80%的单词和20%的新单词组成当前轮单词
    @classmethod
    def generate(cls, level=1, last_words=[], words=None):
        """
        执行生成
        Args:
            level (int): 单词等级
            last_words (list): 上一轮单词ID数组
            words (list): 新单词生成范围
        Returns:
            返回生成的单词ID数组
        """
        from english_pro_module.models import Word
        random.seed(int(time.time()))

        # all_words 为空则自动生成
        if words is None:
            words = ViewUtils.getObjects(Word, level__lte=level)
            words = words.exclude(id__in=last_words)
            words = [word.id for word in words]

        # 没有旧单词&8
        if len(last_words) == 0:
            words = cls.sample(words, cls.WORD_CNT)
        # 已经没有新单词了
        elif len(words) == 0:
            words = cls.sample(last_words, cls.WORD_CNT)
        else:
            old_words = cls.sample(last_words, cls.WORD_CNT * (1 - cls.NEW_WORD_PERCENT))
            new_words = cls.sample(words, cls.WORD_CNT * cls.NEW_WORD_PERCENT)
            old_words.extend(new_words)
            words = old_words

        random.shuffle(words)

        return words

    @classmethod
    def sample(cls, words, num):
        num = int(num)
        cnt = len(words)
        if cnt < num: return words
        return random.sample(words, num)


# ===================================================
# 改错题答案对比
# ===================================================
class CorrectionCompute:

    # 格式化字符串
    @classmethod
    def formatStr(cls, rep, str):
        rep = dict((re.escape(k), v) for k, v in rep.items())
        pattern = re.compile("|".join(rep.keys()))
        str_format = pattern.sub(lambda m: rep[re.escape(m.group(0))], str)

        return str_format

    # 划分句子
    @classmethod
    def splitArticle(cls, origin_article):
        """
        执行生成
        Args:
            origin_article (str): 原改错题文章
        Returns:
            文章句子列表
        """
        # 将文章中的'"'等符号去掉
        rep = {'"': '', '“': '', '”': ''}
        article = cls.formatStr(rep, origin_article)

        # 将文章按 ！？.划分句子
        sentences_list = re.split('[.!?]', article)
        # 去掉最后一句空字符
        sentences = sentences_list[0: len(sentences_list) - 1]
        return sentences

    # 格式化句子
    @classmethod
    def formatSentence(cls, origin_article):
        sentences = cls.splitArticle(origin_article)
        rep = {',': '', '，': '', '；': '', ';': '', ' ': ''}
        sentence_list = []
        for sentence in sentences:
            sentence = sentence.strip()
            words = sentence.split(' ')
            new_sentence = []
            for word in words:
                new_sentence.append(cls.formatStr(rep, word))
            sentence_list.append(new_sentence)

        return sentence_list

    # 判断增加类型改错前一个单词是否是原文里的单词
    @classmethod
    def isAddValid(cls, answer_font, wid, sentence):
        if answer_font != sentence[wid-1]:
            raise GameException(ErrorType.InvalidAnswer)

    # 判断改错类型
    @classmethod
    def correctType(cls, answer):
        answer = answer.split(' ')
        if len(answer) > 1:
            return CorrectType.Add.value
        elif len(answer) == 1:
            return CorrectType.Edit.value
        else:
            return CorrectType.Delete.value

    # 单词是否改对
    @classmethod
    def answer(cls, answer, right_answer, wid, sentence):

        answer_type = cls.correctType(answer)

        if answer_type == CorrectType.Delete.value:
            return answer == right_answer

        elif answer_type == CorrectType.Edit.value:

            # 判断是否只有一个正确答案
            if "/" in right_answer:
                answers = right_answer.split('/')
                if answer in answers:
                    return True
                else:
                    return False
            else:
                return answer == right_answer

        elif answer_type == CorrectType.Add.value:

            # 判断增加单词前面的那个词是否来自原文
            answer_font = answer.split(' ')[0]
            cls.isAddValid(answer_font, wid, sentence)
            answer_back = answer.split(' ')[1]

            # 判断是否只有一个正确答案
            if "/" in right_answer:
                # 只要答对一个正确答案即可
                right_answers = right_answer.split('/')
                if answer_back in right_answers:
                    return True
                else:
                    return False
            else:
                return answer_back == right_answer

    # 计算改错题答对几个
    @classmethod
    def computeRightAnswer(cls, origin_article, wrong_items_frontend, wrong_items_backend):
        sentences = cls.formatSentence(origin_article)
        num = 0

        for wrong_item in wrong_items_backend:
            for wrong in wrong_items_frontend:
                if wrong_item.sentence_index == wrong['sid'] \
                        and wrong_item.word_index == wrong['wid'] \
                        and cls.answer(wrong['word'], wrong_item.word,
                                       wrong['wid'], sentences[wrong_item.sentence_index-1]):
                    print(cls.answer(wrong['word'], wrong_item.word,
                                       wrong['wid'], sentences[wrong_item.sentence_index-1]))
                    num += 1

        return num


# ===================================================
# 商品生成
# ===================================================
class ShopItemGenerator:

    # 计算常量
    RATIOS = [0.8, 0.15, 0.05]  # 每个星级的概率

    CARD_COUNT = 10
    POTION_COUNT = 3
    ITEM_COUNT = 3

    def __init__(self, shop):
        from english_pro_module.runtimes import RuntimeShop
        from english_pro_module.models import \
            ExerProItem, ExerProPotion, ExerProCard
        from item_module.models import ItemType

        self.shop: RuntimeShop = shop

        if self.shop.type_ == ItemType.ExerProItem.value:
            self._generate(ExerProItem, self.ITEM_COUNT)
        if self.shop.type_ == ItemType.ExerProPotion.value:
            self._generate(ExerProPotion, self.POTION_COUNT)
        if self.shop.type_ == ItemType.ExerProCard.value:
            self._generate(ExerProCard, self.CARD_COUNT,
                           self._generateCardRates())

    def _generateCardRates(self):
        stage_order = self.shop.pro_record.stage.order - 1
        rates = self.RATIOS.copy()

        rates[0] -= stage_order * 0.15
        rates[1] += stage_order * 0.1
        rates[2] += stage_order * 0.05

        return rates

    def _generate(self, cla, cnt, rates=None):

        if rates is None: rates = self.RATIOS

        items = ViewUtils.getObjects(cla)
        item_dict = {}

        # 生成 星级 - 物品集 字典
        for i in range(len(rates)):
            item_dict[i+1] = list(items.filter(star_id=i+1))

        for i in range(cnt):
            index = 1  # 星级ID
            rand = random.random()

            for rate in rates:
                rand -= rate
                if rand <= 0:
                    item = self.__generateRandomShopItem(item_dict[index])

                    # 如果物品无法生成，尝试降低星级
                    while item is None and index > 0:
                        index -= 1
                        item = self.__generateRandomShopItem(item_dict[index])

                    if item is not None: self.shop.addShopItem(item)
                    break
                else: index += 1

    def __generateRandomShopItem(self, items):
        from english_pro_module.models import ExerProCard

        if len(items) <= 0: return None

        cnt = 0
        item: ExerProCard = random.choice(items)
        # 如果商店已经包含该物品或者物品不是可购买的，同时循环次数 <= 10000，则重新选择
        while (self.shop.contains(item) or not item.isBoughtable()) \
                and cnt <= 10000:
            item = random.choice(items)
            cnt += 1

        return item

    # 价格计算
    CARD_PRICES = [20, 50, 100]  # 卡牌星级价格数组
    POTION_PRICES = [[30, 45], [55, 75], [0, 0]]
    ITEM_PRICES = [[30, 45], [55, 75], [0, 0]]

    @classmethod
    def generatePrice(cls, item):
        from item_module.models import ItemType

        val = 0

        if item.TYPE == ItemType.ExerProItem:
            val = cls.generateExerProItemPrice(item)
        if item.TYPE == ItemType.ExerProPotion:
            val = cls.generateExerProPotionPrice(item)
        if item.TYPE == ItemType.ExerProCard:
            val = cls.generateExerProCardPrice(item)

        return int(max(0, round(val)))

    @classmethod
    def generateExerProItemPrice(cls, item):
        return random.randint(*cls.ITEM_PRICES[item.star_id - 1])

    @classmethod
    def generateExerProPotionPrice(cls, potion):
        return random.randint(*cls.POTION_PRICES[potion.star_id - 1])

    @classmethod
    def generateExerProCardPrice(cls, card):
        from english_pro_module.models import ExerProCardTarget

        res = card.cost * random.randint(5, 10)
        res += cls.CARD_PRICES[card.star_id - 1] * random.randint(7, 10) / 10
        if card.target == ExerProCardTarget.All.value: res += 10

        return res

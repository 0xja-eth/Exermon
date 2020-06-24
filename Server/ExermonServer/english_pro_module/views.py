from typing import Any

from .models import *
from .runtimes import RuntimeShop

from player_module.models import Player
from item_module.views import Common as ItemCommon
from item_module.models import ItemType
from utils.view_utils import Common as ViewUtils
from utils.calc_utils import NewWordsGenerator, CorrectionCompute
from utils.exception import ErrorType, GameException
from utils.runtime_manager import RuntimeManager

import time
import random
from random import choice


# Create your views here.


# =======================
# 英语模块服务类，封装管理英语模块的业务处理函数
# =======================
class Service:

    # 开始英语特训
    @classmethod
    async def startRecord(cls, consumer, player: Player, mid: int, ):
        # 返回数据：
        # record: 特训记录数据 => 特训记录数据

        pro_record: ExerProRecord = player.exerProRecrod()

        map: ExerProMap = Common.getMap(mid)

        # 没有玩过的记录
        if pro_record is None:
            pro_record = ExerProRecord.create(player)

        if not pro_record.started:
            pro_record.setupMap(map)

        return {'record': pro_record.convertToDict()}

    # 保存英语特训
    @classmethod
    async def saveRecord(cls, consumer, player: Player, record: dict, terminate: bool, ):
        # 返回数据：
        # result: 特训结果数据 => 特训结果（可选）

        # TODO: 在这里加入每个据点之后的校验代码

        pro_record = Common.getExerProRecord(player)

        pro_record.loadFromDict(record)

        # pro_record.save()

        if terminate:
            # TODO: 加入奖励计算，并返回
            pro_record.terminate()

    # 生成英语特训题目
    @classmethod
    async def generateQuestions(cls, consumer, player: Player, type: int, count: int, ):
        # 返回数据：getMap
        # qids: int[] => 生成的题目ID集

        # 检验数量是否合法
        Check.ensureQuestionCount(count)

        # 返回对应类型的题目ID集（里面已经进行了类型判断）
        qids = Common.generateQuestions(count, type)

        return {'qids': qids}

    # 查询英语特训题目
    @classmethod
    async def getQuestions(cls, consumer, player: Player, type: int, qids: list, ):
        # 返回数据：
        # questions: 英语特训题目数据（数组） => 获取的题目数据（听力/阅读/改错）
        # 检验类型是否在枚举类型里面

        # get 的时候同时也已经对类型进行了判断
        questions = Common.getQuestions(ids=qids, type_=type)
        questions = ModelUtils.objectsToDict(questions)

        return {'questions': questions}

    # 生成当前轮单词
    @classmethod
    async def generateWords(cls, consumer, player: Player, ):
        # 返回数据：
        # words: 单词数据（数组） => 单词数据集

        pro_record = Common.getExerProRecord(player)

        Common.ensureFinishLastWords(pro_record)

        pro_record.upgrade()

        return pro_record.convertToDict("words")

    # 查询单词
    @classmethod
    async def getWords(cls, consumer, player: Player, wids: list, ):
        # 返回数据：
        # words: 单词数据（数组） => 单词数据集
        words = Common.getWords(wids)
        words = ModelUtils.objectsToDict(words)

        return {'words': words}

    # 查询单词记录
    @classmethod
    async def getRecords(cls, consumer, player: Player, ):
        # 返回数据：
        # records: 单词记录数据（数组） => 单词记录数据集

        pro_record = Common.getExerProRecord(player)

        return {'word_records': pro_record.convertToDict("records")}

    # 回答当前轮单词
    @classmethod
    async def answerWord(cls, consumer, player: Player, wid: int, chinese: str):
        # 返回数据：
        # correct: bool => 回答是否正确, new: bool => 是否进入下一轮, next: int => 下一个单词ID(可选）

        record = Common.getCurrentWordRecord(player, wid)
        correct = record.answer(chinese)

        pro_record = Common.getExerProRecord(player)
        next = pro_record.nextWord()

        if next is None:
            return {
                'new': True,
                'correct': correct
            }

        else:
            return {
                'new': False,
                'next': next.word_id,
                'correct': correct
            }

    # 回答短语题目
    @classmethod
    async def answerPhrase(cls, consumer, player: Player, qids: list, answers: list):
        # 返回数据
        # correct_num: int => 答对题目数
        correct_num = Common.answerPhrase(pids=qids, options=answers)

        return {'correct_num': correct_num}

    # 回答改错题目
    @classmethod
    async def answerCorrection(cls, consumer, player: Player, qid: int, answers: dict):
        # 返回数据
        # correct_num: int => 答对题目数
        correct_num = Common.answerCorrect(qid=qid, wrongItems=answers)

        return {'correct_num': correct_num}

    # 回答听力题目
    @classmethod
    async def answerListening(cls, consumer, player: Player, qid: int, answers: list):
        # 返回数据
        # correct_num: int => 答对题目数
        correct_num = Common.answerCorrect(qid=qid, wrongItems=answers)

        return {'correct_num': correct_num}

    # # 查询当前轮单词
    # @classmethod
    # async def queryWords(cls, consumer, player: Player, ):
    #     # 返回数据：
    #     # level: int => 当前轮单词等级
    #     # sum: int => 当前轮总单词数
    #     # correct: int => 当前轮正确单词数
    #     # wrong: int => 当前轮错误单词数
    #
    #     pro_record = Common.getExerProRecord(player)
    #
    #     return pro_record.convertToDict("status")

    # 商品生成
    @classmethod
    async def shopGenerate(cls, consumer, player: Player, type: int):
        # 不返回数据
        # 只需检查金钱不足就抛出异常即可，若一切正常就扣取对应的金钱
        Check.ensureExerProItemType(type)

        pro_record = Common.getExerProRecord(player)

        shop = Common.generateShop(pro_record, type)

        return shop.convertToDict()

    # 购买物品校验
    @classmethod
    async def shopBuy(cls, consumer, player: Player, type: int, order: int, num: int):
        # 不返回数据
        # 只需检查金钱不足就抛出异常即可
        Check.ensureExerProItemType(type)

        pro_record = Common.getExerProRecord(player)

        shop = Common.getShop(pro_record, type)
        shop.buy(order, num)


# ======================
# 英语模块校验类，封装英语模块业务数据格式校验的函数
# =======================
class Check:

    # 校验要生成题目的数量>=1
    @classmethod
    def ensureQuestionCount(cls, count: int):
        if count < 1:
            raise GameException(ErrorType.InvalidQuestionCount)

    # 校验要生成题目的类型是否合法
    @classmethod
    def ensureQuestionType(cls, type: int):
        ViewUtils.ensureEnumData(type, QuestionType, ErrorType.InvalidQuestionType, True)

    # 校验购买物品是否在ExerProItem, ExerProPotion, ExerProCard类型中
    @classmethod
    def ensureExerProItemType(cls, type: int):

        item_list = [ItemType.ExerProItem.value, ItemType.ExerProPotion.value, ItemType.ExerProCard.value]

        if type not in item_list:
            raise GameException(ErrorType.IncorrectItemType)


# =======================
# 英语模块公用类，封装关于英语模块的公用函数
# =======================
class Common:
    @classmethod
    def getMap(cls, id) -> ExerProMap:
        """
        获取地图
        Args:
            id (int): 地图ID
        Returns:
            返回地图对象
        """
        return ViewUtils.getObject(ExerProMap, ErrorType.MapNotFound, id=id)

    @classmethod
    def getMapStage(cls, id=None, mid=None, order=None) -> ExerProMapStage:
        """
        获取关卡
        Args:
            id (int): 关卡ID
            mid (int): 地图ID
            order (int): 关卡序号
        Returns:
            返回地图对象
        """
        if id is not None:
            return ViewUtils.getObject(ExerProMapStage, ErrorType.StageNotFound, id=id)

        if mid is not None and order is not None:
            return ViewUtils.getObject(ExerProMapStage, ErrorType.StageNotFound, map_id=mid, order=order)

    @classmethod
    def getItemClass(cls, type_: int):
        """
        获取物品类型
        Args:
            type_ (int): 物品类型（枚举值）
        Raises:
            ErrorType.InvalidQuestionType: 题目类型不正确
        Returns:
            返回题目类型变量
        """
        if type_ == ItemType.ExerProItem.value:
            return ExerProItem
        elif type_ == ItemType.ExerProPotion.value:
            return ExerProPotion
        elif type_ == ItemType.ExerProCard.value:
            return ExerProCard

        raise GameException(ErrorType.IncorrectItemType)

    @classmethod
    def getQuestionClass(cls, type_: int):
        """
        获取题目类型
        Args:
            type_ (int): 题目类型（枚举值）
        Raises:
            ErrorType.InvalidQuestionType: 题目类型不正确
        Returns:
            返回题目类型变量
        """
        if type_ == QuestionType.Listening.value:
            return ListeningQuestion
        elif type_ == QuestionType.Phrase.value:
            return PhraseQuestion
        elif type_ == QuestionType.Correction.value:
            return CorrectionQuestion
        elif type_ == QuestionType.Plot.value:
            return PlotQuestion

        raise GameException(ErrorType.InvalidQuestionType)

    @classmethod
    def generateQuestions(cls, count: int, type_: int = None, cla: type = None, ):
        """
        从数据库中随机选出题目
        Args:
            type_ (int): 题目类型（枚举值）
            cla (type): 题目类型
            count (int) 题目数量
        Returns:
            返回对应类型题目的ID集，若超过题库数量抛出设置好的异常
        """
        if cla is None: cla = cls.getQuestionClass(type_)

        question_all = ViewUtils.getObjects(cla)
        question_all = [question.id for question in question_all]

        if len(question_all) < count:
            raise GameException(ErrorType.InvalidQuestionDatabaseCount)

        return random.sample(question_all, count)

    # 获取多个题目
    @classmethod
    def getQuestions(cls, ids=None, error: ErrorType = ErrorType.QuestionNotExist,
                     type_: int = None, cla: type = None, **kwargs) -> list:
        """
        获取多个题目
        Args:
            ids (list): 题目ID集
            type_ (int): 题目类型（枚举值）
            cla (type): 题目类型
            error (ErrorType): 抛出异常
            **kwargs (**dict): 查询参数
        Returns:
            当 ids 不为 None 时，返回指定 ID 的题目
            否则只返回满足条件的题目
        """
        if cla is None: cla = cls.getQuestionClass(type_)

        if ids is None:
            questions = ViewUtils.getObjects(cla, **kwargs)
            return questions

        unique_ids = list(set(ids))

        res = ViewUtils.getObjects(cla, id__in=ids)

        # 数量不一致，说明获取出现问题
        if res.count() != len(unique_ids):
            raise GameException(error)

        return res

    # 获取单个题目
    @classmethod
    def getQuestion(cls, id=None, error: ErrorType = ErrorType.QuestionNotExist,
                     type_: int = None, cla: type = None, **kwargs):
        """
        获取单个题目
        Args:
            ids (list): 题目ID集
            type_ (int): 题目类型（枚举值）
            cla (type): 题目类型
            error (ErrorType): 抛出异常
            **kwargs (**dict): 查询参数
        Returns:
            否则指定类型的单个题目
        """
        if cla is None:
            cla = cls.getQuestionClass(type_)
        res = ViewUtils.getObject(cla, error=error, id=id)

        return res

    # 获取指定列表的单词集
    @classmethod
    def getWords(cls, wids=None, error: ErrorType = ErrorType.WordNotExit, **kwargs) -> list:
        """
        获取多个单词
        Args:
            wids (list): 单词ID集
            error (ErrorType): 抛出异常
        Returns:
            当 wids 不为 None 时，返回指定 ID 的单词
            否则只返回满足条件的题目
        """
        if wids is None:
            words = ViewUtils.getObjects(Word, **kwargs)
            return words

        unique_ids = list(set(wids))

        res = ViewUtils.getObjects(Word, id__in=wids)

        # 数量不一致，说明获取出现问题
        if res.count() != len(unique_ids):
            raise GameException(error)

        return res

    @classmethod
    def getExerProRecord(cls, player: Player,
                         error: ErrorType = ErrorType.ExerProRecordNotExist) -> ExerProRecord:
        """
        获取特训记录
        Args:
            player (Player): 玩家
            error (ErrorType): 抛出的错误类型
        Returns:
            返回玩家对应的特训记录
        """
        pro_record: ExerProRecord = player.exerProRecrod()
        if pro_record is None: raise GameException(error)

        return pro_record

    @classmethod
    def getWordRecords(cls, player: Player) -> list:
        """
        获取所有单词记录
        Returns:
            返回当前玩家的所有单词记录
        """
        return cls.getExerProRecord(player).wordRecords()

    @classmethod
    def getWordRecord(cls, player: Player, wid,
                      error: ErrorType = ErrorType.WordRecordNotExit,
                      **kwargs) -> WordRecord:
        """
        获取单个单词记录
        Args:
            player (Player): 玩家
            wid (int): 单词ID
            error (ErrorType): 异常类型
            **kwargs (**dict): 查询参数
        Returns:
            返回当前玩家的指定单词的单词记录
        """
        pro_record = cls.getExerProRecord(player)

        record = pro_record.wordRecord(wid, **kwargs)
        if record is None: raise GameException(error)

        return record

    @classmethod
    def getCurrentWordRecord(cls, player: Player, wid,
                             error: ErrorType = ErrorType.NoInCurrentWords) -> WordRecord:
        """
        获取单个当前单词记录
        Args:
            player (Player): 玩家
            wid (int): 单词ID
            error (ErrorType): 异常类型
        Returns:
            返回当前玩家的指定单词的当前单词记录
        """
        pro_record = cls.getExerProRecord(player)

        record = pro_record.wordRecord(wid, current=True)
        if record is None: raise GameException(error)

        return record

    # 检验答词ID是否在当前轮中
    @classmethod
    def ensureWordInCurrentWords(cls, wid: int, player: Player, words: list):
        """
        保证指定单词为当前轮单词
        Args:
            player (Player): 用户
            wid (int): 单词
            words (list)：单词ID集
        Returns:
            如果单词不在当前轮中则报错
        """
        pro_record = cls.getExerProRecord(player)

        record = pro_record.wordRecord(wid, current=True)
        if record is None: raise GameException(ErrorType.NoInCurrentWords)

    @classmethod
    def ensureWordNotCorrect(cls, word_rec: WordRecord):
        if word_rec.current_correct: raise GameException()

    @classmethod
    def ensureFinishLastWords(cls, pro_record: ExerProRecord,
                              error: ErrorType = ErrorType.AnswerNotFinish):
        """
        确保当前轮单词全部通过
        Args:
            pro_record (ExerProRecord): 特训记录
            error (ErrorType): 异常
        """
        if not pro_record.isFinished(): raise GameException(error)

    @classmethod
    def ensureMapEnable(cls, pro_record: ExerProRecord, map: ExerProMap):
        """
        确保指定地图可用
        Args:
            pro_record (ExerProRecord): 特训记录
            map (ExerProMap): 特训地图
        """

        if pro_record.started and pro_record.stage \
                and pro_record.stage.map != map:
            raise GameException(ErrorType.ExerProStarted)

        # TODO: 增加等级判断代码

    @classmethod
    def generateShop(cls, pro_record: ExerProRecord, type_: int) -> RuntimeShop:
        """
        生成商品校验
        Args:
            pro_record (): 特训记录
            type_ (int): 物品枚举
        Returns:
            返回生成的商品对象
        """
        shop = RuntimeShop(pro_record, type_)

        RuntimeManager.add(RuntimeShop, shop)

        return shop

    @classmethod
    def getShop(cls, pro_record: ExerProRecord, type_: int,
                error: ErrorType = ErrorType.ShopNotGenerated) -> RuntimeShop:
        """
        获取商店
        Args:
            pro_record (ExerProRecord): 特训记录
            type_ (int): 物品类型（枚举值）
            error (ErrorType): 异常
        Returns:
            返回对应的商店
        """
        key = "%d-%d" % (pro_record.id, type_)

        shop = RuntimeManager.get(RuntimeShop, key)

        if shop is None and error is not None:
            raise GameException(error)

        return shop

    @classmethod
    def answerPhrase(cls, pids: list, options: list, **kwargs):
        """
        回答短语题目
        Args:
            pids (list): 短语题目ID集
            options (list): 短语题目回答集
            **kwargs (**dict): 查询参数
        """
        phrases = Common.getQuestions(ids=pids, type_=QuestionType.Phrase.value)
        correct_num = 0
        for i in range(len(options)):
            if phrases[i].phrase == options[i]:
                correct_num += 1

        return correct_num

    @classmethod
    def answerCorrect(cls, qid: int, wrongItems: list, **kwargs):
        """
        回答短语题目
        Args:
            pids (list): 短语题目ID集
            options (list): 短语题目回答集
            **kwargs (**dict): 查询参数
        """
        question = cls.getQuestion(id=qid, type_=QuestionType.Correction.value)
        wrong_item_backend = question.wrongItems()
        wrong_item_frontend = wrongItems

        num = CorrectionCompute.compute_right_answer(question.article, wrong_item_frontend, wrong_item_backend)

        return num

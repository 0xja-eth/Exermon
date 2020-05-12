from question_module.models import Question
from record_module.models import QuestionRecord
import datetime
from utils.model_utils import Common as ModelUtils
from django.db.models import Q


# =======================================
#生成某个题目记录的各项统计资料-lgy5.1
# =======================================
class QuesRecordData:

    # 刷新最短间隔
    MIN_DELTA = datetime.timedelta(0, 300)

    # 更新时间（分钟数）
    UPDATE_MINUTES = [0, 30]

    update_time : datetime.datetime = None

    record_dict: dict = {}

    def __init__(self, question):
        self.ques_id = question
        self.update()

    def update(self):

        record = QuestionRecord.objects.filter(question = self.ques_id)

        self.sum_player = len(record)
        self.sum_collect = len(i for i in record if i.collect == True)
        self.all_corr_rate = sum(i.corr_rate for i in record)/self.sum_player
        self.update_time = datetime.datetime.now()

        dict[self.ques_id] = self.convertToDict(self.ques_id, type= "overall")


    def getData(self, question,player):
        '''
        获得该题目的做题信息
        '''
        if question in self.record_dict:
            if self.isUpdateRequired():
                self.update()
            return self.record_dict[question].update(self.convertToDict(question,player,'personal'))
        else:
            self.update()
            return self.convertToDict(question, player,'detail')


    def isUpdateRequired(self):
        now = datetime.datetime.now()
        return now.minute in self.UPDATE_MINUTES and \
               now - self.update_time >= self.MIN_DELTA

    def convertToDict(self, question, player = None, type=''):

        update_time = ModelUtils.timeToStr(self.update_time)

        if type == "overall":
            return {
                'id': question,
                'sum_player': self.sum_player,
                'sum_collect': self.sum_collect,
                'all_corr_rate': self.all_corr_rate,
                'all_avg_time': ModelUtils.timeToStr(self.all_avg_time),
                'update_time': update_time
            }

        # 网上看的，使用Q方法同时匹配多个关键字
        record = QuestionRecord.objects.filter(Q(question = self.ques_id)| Q(player= player))

        if type == "detail":
            return {
                'id': question,
                'sum_player': self.sum_player,
                'count': record.count,
                'sum_collect': self.sum_collect,
                'corr_rate': record.corr_rate(),
                'all_corr_rate': self.all_corr_rate,
                'first_time': ModelUtils.timeToStr(record.first_time),
                'last_time' : ModelUtils.timeToStr(record.last_time),
                'all_avg_time': ModelUtils.timeToStr(self.all_avg_time),
                'first_date': ModelUtils.dateToStr(record.first_date),
                'update_time': update_time
            }

        if type == "personal":
            return {
                'id': question,
                'count': record.count,
                'corr_rate': record.corr_rate(),
                'first_time': ModelUtils.timeToStr(record.first_time),
                'last_time': ModelUtils.timeToStr(record.last_time),
                'first_date': ModelUtils.dateToStr(record.first_date),
            }
from django.db import models
from utils.model_utils import Common as ModelUtils
from game_module.models import GroupConfigure
from game_module import consumer
from player_module.views import Common
from player_module.models import Player
import datetime

# Create your models here.

#对战评星表，记录对战结果与扣除/增加星星数的关系
class CombatResult(GroupConfigure):
    class Meta:
        verbose_name = verbose_name_plural = "对战评星"

    #对战评价
    result = models.CharField(max_length=3,verbose_name="对战评价")

    #胜利增加星星
    win = models.SmallIntegerField(verbose_name="胜利增星数")

    #失败减少星星
    lose = models.SmallIntegerField(verbose_name="失败减星数")

# =======================
# 赛季记录表
# =======================
class SeasonRecord(models.Model):

    class Meta:
        verbose_name = verbose_name_plural = "赛季记录"

    # 玩家
    player = models.ForeignKey('player_module.Player', on_delete=models.CASCADE, verbose_name="玩家")

    # 赛季
    season = models.ForeignKey('CompSeason', on_delete=models.CASCADE, verbose_name="赛季")

    # 赛季积分
    point = models.PositiveSmallIntegerField(default=0, verbose_name="赛季积分")

    # 段位星星
    star_num = models.PositiveSmallIntegerField(default=0, verbose_name="段位星星")

    def convertToDict(self):
        suspensions = ModelUtils.objectsToDict(self.suspensions())

        return {
            'id': self.id,
            'player_id': self.player,
            'season_id': self.season,
            'star_num': self.star_num,
            'point': self.point,

            'suspensions': suspensions,
        }

    #增减赛季积分
    def adjustPoint(self,*args):
        self.point+=args
        if self.point < 0:
            self.point =0
        self.save()

    #增减星星数量
    def adjustStarNum(self,*args):
        rank, sub_rank = self.rank()

        #星星减少
        while args<0 :
            #当前段位赛季积分和抵消因子
            self.point -= rank.offset_factor
            ++args
            rank, sub_rank = self.rank()

            # 情况1.赛季积分不够完全抵消，point用完，此时args<0，段位可能改变
            if self.point < 0 :
                self.point += rank.offset_factor
                --args
                break

        #情况2，赛季积分可以完全抵消星星减少数，args提前变为0，段位不变
        #情况3，星星数增加，段位可能改变

        if args != 0:
            self.star_num += args
            new_rank, new_sub_rank = self.rank()
            if new_rank != rank or new_sub_rank!= sub_rank:
                player = Common.getOnlinePlayer(self.player)
                player.comsumer.emit("段位改变为"+new_rank+new_sub_rank)

        self.save()

    def adjustCredit(self,uid,credit):
        temp = Player.objects.get(id=uid)
        temp.credit += credit

        #针对第1，2，3次禁赛，分别设置3，7，30天的禁赛期
        if temp.credit <50 and len(self.suspensions())==2:

            sus = SuspensionRecord(self.id, datetime.datetime.now(),30)
            sus.save()

        elif temp.credit < 50 and len(self.suspensions())==1:

            sus = SuspensionRecord(self.id, datetime.datetime.now(),7)
            sus.save()

        elif temp.credit < 50 and len(self.suspensions())==0:

            sus = SuspensionRecord(self.id, datetime.datetime.now(), 3)
            sus.save()

        temp.save()

    # 所有的禁赛纪录
    def suspensions(self):
        return self.suspensionrecord_set.all()

    # 计算实际段位
    def rank(self):
        # ranks 储存了段位列表中的每一个段位的详细信息
        ranks = CompRank.objs()

        # 每个段位需要的星星数量相加
        sum = 0

        for rank in ranks:
            rank_stars = rank.rankStars()

            # 如果段位是无限累计 或 星星数不足以进入下一段位
            if rank_stars == 0 or self.star_num < sum+rank_stars:
                # 计算子段位（还是从 0 开始计算吧）
                sub_rank = (self.star_num-sum) / CompRank.STARS_PER_SUBRANK

                return rank, sub_rank

            sum += rank_stars

        return None, 0


# =======================
# 禁赛记录表
# =======================
class SuspensionRecord(models.Model):

    class Meta:
        verbose_name = verbose_name_plural = "禁赛记录"

    def __init__(self, seaon_rec, start_time, time_length):
        self.season_rec = seaon_rec
        self.start_time = start_time
        self.end_time = start_time+ datetime.timedelta(days=time_length)

    # 赛季记录,一对多的关系，多个禁赛记录对应一个赛季记录
    season_rec = models.ForeignKey('SeasonRecord', on_delete=models.CASCADE,
                                   verbose_name="赛季记录")

    # 开始时间
    start_time = models.DateTimeField(auto_now_add=True, verbose_name="开始时间")

    # 结束时间
    end_time = models.DateTimeField(verbose_name="结束时间")

    def convertToDict(self):
        start_time = ModelUtils.timeToStr(self.start_time)
        end_time = ModelUtils.timeToStr(self.end_time)

        return {
            'start_time': start_time,
            'end_time': end_time
        }


# =======================
# 赛季配置表
# =======================
class CompSeason(GroupConfigure):

    class Meta:
        verbose_name = verbose_name_plural = "赛季信息"

    #赛季名称
    season_name = models.CharField(max_length=10,verbose_name="赛季名称")

    # 开始时间
    start_time = models.DateTimeField(verbose_name="开始时间")

    # 结束时间
    end_time = models.DateTimeField(verbose_name="结束时间")

    def convertToDict(self):
        res = super().convertToDict()

        start_time = ModelUtils.timeToStr(self.start_time)
        end_time = ModelUtils.timeToStr(self.end_time)

        res['start_time'] = start_time
        res['end_time'] = end_time

        return res


# =======================
# 段位配置表
# =======================
class CompRank(GroupConfigure):

    class Meta:
        verbose_name = verbose_name_plural = "段位信息"

    # 每个小段位所需星星数
    STARS_PER_SUBRANK = 3

    # 颜色
    color = models.CharField(max_length=7, null=False,
                             default='#000000', verbose_name="颜色")

    #段位名称
    rank_name = models.CharField(max_length=2,verbose_name="段位名称")

    # 子段位数（0 为无限）
    sub_rank_num = models.PositiveSmallIntegerField(default=3, verbose_name="小段位数")

    # 积分因子
    score_factor = models.PositiveSmallIntegerField(default=80, verbose_name="积分因子")

    # 抵消积分
    offset_factor = models.PositiveSmallIntegerField(default=60, verbose_name="抵消使用积分")

    def convertToDict(self):
        res = super().convertToDict()

        res['rank_name'] = self.rank_name
        res['color'] = self.color
        res['sub_rank_num'] = self.sub_rank_num
        res['score_factor'] = self.score_factor
        res['offset_factor'] = self.offset_factor

        return res

    # 计算每个段位需要的星星数量
    def rankStars(self):
        return self.sub_rank_num * self.STARS_PER_SUBRANK

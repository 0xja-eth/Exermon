from django.shortcuts import render
from .models import *
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, GameException
from player_module.models import Player


class Service:
    # 查询赛季记录-lgy
    @classmethod
    async def getRecord(cls, consumer, player: Player, sid: int):

        rec = Common.getSeasonRecord(player, sid)

        return {"record": rec.convertToDict()}

    # 查询赛季排行
    @classmethod
    async def getRanks(cls, consumer, player: Player, sid: int, count: int, ):
        # 返回数据：
        # ranks: 赛季排行数据（数组） => 赛季排行
        season = CompSeason.get(id=sid)

        return {'ranks': season.convertToDict('ranks', count, player)}


class Common:

    @classmethod
    def getSeasonRecord(cls, player: Player, sid: int, error: ErrorType = ErrorType.SeasonRecordNotExist) -> SeasonRecord:
        """
        获取赛季记录
        Args:
            player (Player): 玩家
            sid (int): 赛季ID
            error(ErrorType): 找不到时抛出的异常
        Returns:
            返回赛季记录
        """
        return ViewUtils.getObject(SeasonRecord, error, player=player, season_id=sid)

    # 确保赛季存在-lgy
    @classmethod
    def ensureSeasonExist(cls, sid: int):
        """
        确保赛季存在
        Args:
            sid (int): 赛季ID
        Raises:
            ErrorType.SeasonNotExist: 赛季不存在
        """
        ViewUtils.ensureObjectExist(CompSeason, ErrorType.SeasonNotExist, id=sid)



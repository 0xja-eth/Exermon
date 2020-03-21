from django.shortcuts import render
from .models import *
from utils.view_utils import Common as ViewUtils
from utils.exception import ErrorType, GameException


class Service:
    # 查询赛季记录-lgy
    @classmethod
    async def getRecords(cls, consumer, player: Player, sid: int):

        common.ensureSeasonExist(sid)
        return ViewUtils.getObjects(SeasonRecord, player=player,season = sid)

class common:

    # 确保赛季存在-lgy
    @classmethod
    def ensureSeasonExist(cls, sid:CompSeason):
        ViewUtils.ensureObjectExist(sid, ErrorType.SeasonNotExist)


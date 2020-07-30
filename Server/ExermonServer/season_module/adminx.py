import xadmin

from .models import *
from utils.model_utils import AdminXHelper


@AdminXHelper.relatedModel(SeasonRecord)
class SeasonRecordAdmin(object): pass


@AdminXHelper.relatedModel(SuspensionRecord)
class SuspensionRecordAdmin(object): pass


@AdminXHelper.relatedModel(CompSeason)
class CompSeasonAdmin(object): pass


@AdminXHelper.relatedModel(CompRank)
class CompRankAdmin(object): pass

import math


# ================================
# 通用计算
# ================================
class Common:

    # Sigmoid 函数
    @classmethod
    def sigmoid(cls, x):
        return 1 / (1 + math.exp(-x))

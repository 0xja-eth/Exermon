
from django.contrib import admin
from django.conf import settings
from django.urls import path, include

from game_module import views

urlpatterns = [
    path('backend', views.backend)
]
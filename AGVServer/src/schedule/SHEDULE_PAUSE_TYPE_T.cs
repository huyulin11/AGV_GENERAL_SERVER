namespace AGV.schedule {
	public enum SHEDULE_PAUSE_TYPE_T
    {
        SHEDULE_PAUSE_TYPE_MIN = 0,  //一般叉获任务，优先级最低
        SHEDULE_PAUSE_SYSTEM_WITH_START, //设置系统暂停，暂停所有的车子，清除标志之后，所有的车重新被启动  用于系统暂停功能
        SHEDULE_PAUSE_SYSTEM_WITHOUT_START, // 设置系统暂停，暂停所有的车子，清除标志后，所有的车子不被启动，需要手动启动
        SHEDULE_PAUSE_UP_WITH_START, // 暂停楼上的车子，清除标志后，楼上的车子重新被启动  用于 下货期间 楼上楼下都有货
        SHEDULE_PAUSE_UP_WITHOUT_START,  //暂停楼上的车，清除标志后，楼上的车不被重新启动，需要手动启动    用于检测中断

        SHEDULE_PAUSE_UP_MAX, //区分楼上暂停和楼下暂停指令
        SHEDULE_PAUSE_DOWN_WITH_START, // 暂停楼下的车子，清除标志后，楼下的车子重新被启动  用于上货期间楼上楼下都有货
        SHEDULE_PAUSE_DOWN_WITHOUT_START,  //暂停楼下的车，清除标志后，楼下的车不被重新启动，需要手动启动
        SHEDULE_PAUSE_TYPE_MAX
    };
}

namespace AGV.init {

	public enum ENV_ERR_TYPE {
		ENV_ERR_OK = 0,
		ENV_LIFT_COM_ERR = 1,  //升降机串口错误
		ENV_CACHE_TASKRECORD_WARNING = 2,  //缓存的任务记录
		ENV_CACHE_UPTASKRECORD_WARNING = 3, //缓存的上货任务

		ENV_ERR_MAX
	};

}

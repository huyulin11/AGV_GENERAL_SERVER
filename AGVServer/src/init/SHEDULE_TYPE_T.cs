namespace AGV.init {

	/// <summary>
	/// 需要车子需要调度的几个临界点
	/// </summary>
	public enum SHEDULE_TYPE_T {
		SHEDULE_TYPE_MIN,

		SHEDULE_TYPE_1TO2,  //从区域1到区域2

		SHEDULE_TYPE_2TO3,  //从区域2到区域3

		SHEDULE_TYPE_3TO2,  //从区域3到区域2

		SHEDULE_TYPE_2TO1,  //从区域2到区域1
	};
}
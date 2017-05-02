namespace AGV.elevator {

	/// <summary>
	/// 从升降机发送到服务端的指令集合
	/// </summary>
	public enum COMMAND_FROME2S {
		LIFT_OUT_COMMAND_MIN = 0,  //无任务
		LIFT_OUT_COMMAND_UP = 0x1, //楼下货已经到位，提醒楼下3号AGV去取货
		LIFT_OUT_COMMAND_DOWN = 0x2, //楼上货已经到位，提醒楼上AGV去取货
		LIFT_OUT_COMMAND_UP_DOWN = 0x3, //楼上楼下都有货，这个时候需要弹出信号，提示框提示
		LIFT_OUT_COMMAND_louxia_baojing = 0x4,
		LIFT_OUT_COMMAND_loushang_baojing = 0x8,
		LIFT_OUT_COMMAND_MAX,
	};
}

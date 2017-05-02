namespace AGV.elevator {

	/// <summary>
	/// 从服务端发送到升降机的指令集合
	/// </summary>
	public enum COMMAND_FROMS2E {
		LIFT_IN_COMMAND_MIN = 0,  //无任务
		LIFT_IN_COMMAND_UP = 0x1, //楼下取货，送到楼上来
		LIFT_IN_COMMAND_DOWN = 0x2, //楼上取货，送到楼下去
		LIFT_IN_COMMAND_MAX,
	};
}

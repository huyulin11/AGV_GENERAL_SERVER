namespace AGV.elevator {
	public interface ElevatorOperator {

		/// <summary>
		/// 初始化升降机
		/// </summary>
		ElevatorOperator initSerialPort();

		/// <summary>
		/// 设置升降机控制命令
		/// </summary>
		void setDataCommand(COMMAND_FROMS2E command);

		/// <summary>
		///获取升降机输出命令
		/// </summary>
		COMMAND_FROME2S getOutCommand();

		/// <summary>
		/// 创建并开始运行一个读取串口命令的线程
		/// </summary>
		void startReadSerialPortThread();

		/// <summary>
		/// 重新启动升降机初始化，可能存在错误，如果存在错误，read的时候会返回错误
		/// </summary>
		void reStart();

		/// <summary>
		/// 获取升降机的状态
		/// </summary>
		bool getStat();
	}
}

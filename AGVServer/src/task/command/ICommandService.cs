using AGV.bean;
using AGV.forklift;

namespace AGV.command {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public interface ICommandService {

		void setLatestMsgFromClient(string receiveStr);

		void sendCommand();

		void resolveTaskCommand();

		void resolveSYSCtrlCommand();
	}
}

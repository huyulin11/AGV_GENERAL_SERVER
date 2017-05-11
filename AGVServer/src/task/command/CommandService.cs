using AGV.bean;
using AGV.dao;
using AGV.forklift;
using System.Collections.Generic;

namespace AGV.command {

	/// <summary>
	/// 描述任务从用户下达到发送AGV执行前的逻辑
	/// </summary>
	public class CommandService : ICommandService {

		private static CommandService commandService = null;

		public static ICommandService getInstance() {
			if (commandService == null) {
				commandService = new CommandService();
			}
			return commandService;
		}

		public bool isSystemRunning() {
			List<TaskexeBean> taskexeBeanList = TaskexeDao.getDao().getTaskexeStartOrPauseList();
			if (taskexeBeanList == null || taskexeBeanList.Count <= 0) {
				return true;
			}
			return "sysContinue".Equals(taskexeBeanList[0].getTaskid());
		}
	}
}

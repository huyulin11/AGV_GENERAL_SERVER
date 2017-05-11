namespace AGV.bean {
	public class CommandBean {
		private string uuid;
		private string time;
		private string taskid;
		private string opflag;

		public void setUuid(string uuid) {
			this.uuid =  uuid;
		}

		public string getUuid() {
			return uuid;
		}

		public void setTime(string time) {
			this.time = time;
		}

		public string getTime() {
			return time;
		}

		public void setTaskid(string taskid) {
			this.taskid = taskid;
		}

		public string getTaskid() {
			return taskid;
		}

		public void setOpflag(string opflag) {
			this.opflag= opflag;
		}
		public string getOpflag() {
			return opflag;
		}
	}
}

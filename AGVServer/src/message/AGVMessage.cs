using System;
namespace AGV.message {
	public class AGVMessage {
		private AGVMessageHandler_TYPE_T message_type = AGVMessageHandler_TYPE_T.AGVMessageHandler_MIN;
		private string message_str = String.Empty;

		public static AGVMessage newMessage(AGVMessageHandler_TYPE_T type, string str) {
			AGVMessage message = new AGVMessage();
			message.message_type = type;
			message.message_str = str;
			return message;
		}

		public void clear() {
			this.message_str = String.Empty;
			this.message_type = AGVMessageHandler_TYPE_T.AGVMessageHandler_MIN;
		}

		public AGVMessageHandler_TYPE_T getMessageType() {
			return this.message_type;
		}

		public string getMessageStr() {
			return this.message_str;
		}

		public void setMessageType(AGVMessageHandler_TYPE_T type) {
			this.message_type = type;
		}

		public void setMessageStr(string str) {
			this.message_str = str;
		}
	}
}

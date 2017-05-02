namespace AGV.schedule {
	public class ScheduleFactory {
		private static ScheduleOperator schedule = null;

		public static ScheduleOperator newSchedule() {
			return new ScheduleProduction();
		}

		public static ScheduleOperator getSchedule() {
			if (schedule == null) {
				schedule = ScheduleFactory.newSchedule();
			}
			return schedule;
		}
	}
}

namespace AGV.elevator {
	public class ElevatorFactory {
		private static ElevatorOperator elevator = null;
		public static ElevatorOperator newElevator() {
			return new ElevatorProduction();
		}

		public static ElevatorOperator getElevator() {
			if (elevator == null) {
				elevator = ElevatorFactory.newElevator();
			}
			return elevator;
		}
	}
}

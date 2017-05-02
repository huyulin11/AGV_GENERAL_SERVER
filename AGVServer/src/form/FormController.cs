namespace AGV.form {
	public class FormController {
		private static FormController formController = null;
		private MainFrm mainFrm = null;
		private LoginFrm loginFrm = null;
		private AGVConfigureForm fcForm = null;
		private AGVManualCtrlForm amcForm = null;

		public static FormController getFormController() {
			if (formController == null) {
				formController = new FormController();
			}
			return formController;
		}

		private FormController() {
		}

		public MainFrm getMainFrm() {
			if (mainFrm == null) {
				mainFrm = new MainFrm();
			}
			return mainFrm;
		}

		public LoginFrm getLoginFrm() {
			if (loginFrm == null) {
				loginFrm = new LoginFrm();
			}
			return loginFrm;
		}

		public AGVConfigureForm getAGVConfigureForm() {
			if (fcForm == null) {
				fcForm = new AGVConfigureForm();
			}
			return fcForm;
		}

		public AGVManualCtrlForm getAGVManualCtrlForm() {
			if (amcForm == null) {
				amcForm = new AGVManualCtrlForm();
				amcForm.initAGVMCForm();
			}
			return amcForm;
		}
	}
}

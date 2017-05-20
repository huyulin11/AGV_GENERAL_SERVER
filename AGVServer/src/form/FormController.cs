namespace AGV.form {
	public class FormController {
		private static FormController formController = null;
		private MainFrm mainFrm = null;
		private LoginFrm loginFrm = null;
		private AGVConfigureForm fcForm = null;
		private AGVManualCtrlForm amcForm = null; 
		private InfoFrm infoFrm = null;

		private static bool needLogin = false;//系统是否需要登录界面

		private static bool needMain = true;//系统是否需要使用C#控制界面

		public static FormController getFormController() {
			if (formController == null) {
				formController = new FormController();
			}
			return formController;
		}

		private FormController() {
		}

		public static bool isNeedLogin() {
			return needLogin;
		}

		public static bool isNeedMain() {
			return needMain;
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

		public InfoFrm getInfoFrm() {
			if (infoFrm == null) {
				infoFrm = new InfoFrm();
			}
			return infoFrm;
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

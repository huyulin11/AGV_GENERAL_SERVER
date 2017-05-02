using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using AGV.forklift;
using AGV.dao;

namespace AGV.form {
	public partial class AGVManualCtrlForm : Form {
		Hashtable pausePanelHash = new Hashtable();

		public AGVManualCtrlForm() {
			InitializeComponent();
		}

		public void initAGVMCForm() {
			int tmp = 0;
			foreach (ForkLiftWrapper fl in AGVCacheData.getForkLiftWrapperList()) {
				if (fl.getForkLift().forklift_number == 3) {
					//continue;
				}
				PauseCtrlPanel pcp = new PauseCtrlPanel();
				pcp.initPanel(fl);
				pcp.Location = new Point(40, 30 + tmp * 60);
				pcp.Size = new Size(200, 50);

				this.Controls.Add(pcp);

				pausePanelHash.Add(fl.getForkLift().forklift_number, pcp);
				tmp++;
			}
		}

		public void updateAGVMCForm() {
			foreach (DictionaryEntry de in pausePanelHash) {
				PauseCtrlPanel pcp = (PauseCtrlPanel)de.Value;
				pcp.updatePanel();
			}
		}
	}
}

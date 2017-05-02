using AGV.init;
namespace AGV.forklift {
	public class Position  //描述位置
	{
		private int px = 0;  //位置横坐标
		private int py = 0;  //位置纵坐标
		private int area = 1;   //默认在区域1  位置区域   1代表区域1：  x1>x>x2 && y1<y<y3   (正常情况车子不会出现在x1<x<x2 && y2<y<y3的位置，所以这段位置不单独考虑) 2代表区域2：x<x2 || y<y1l
		private int startPx = 0;
		private int startPy = 0;

		public Position() {
		}

		public Position(int px, int py, int area) {
			this.px = px;
			this.py = py;
		}

		public void setPx(int px) {
			this.px = px;
		}
		public int getPx() {
			return this.px;
		}

		public void setPy(int py) {
			this.py = py;
		}

		public int getPy() {
			return this.py;
		}

		public void setArea(int area) {
			this.area = area;
		}

		public int getArea() {
			return this.area;
		}

		public int getStartPx() {
			return this.startPx;
		}

		public void calcPositionArea() {
			if (this.getPx() > AGVConstant.BORDER_X_2)
				this.setArea(1);
			else if (this.getPx() > AGVConstant.BORDER_X_3)
				this.setArea(2);
			else
				this.setArea(3);
		}

		public int calcArea(int px, int py) {
			int area = 0;

			if (px > AGVConstant.BORDER_X_2 && px < AGVConstant.BORDER_X_1 && py < AGVConstant.BORDER_Y_1 && py < AGVConstant.BORDER_Y_3)
				area = 1;
			else if (px < AGVConstant.BORDER_X_2 && py < AGVConstant.BORDER_Y_1)
				area = 2;
			return area;
		}

		public void setStartPosition(int px, int py) {
			this.startPx = px;
			this.startPy = py;
		}

		public void updateStartPosition() {//用当前的位置，更新起始位置的坐标
			this.startPx = this.px;
			this.startPy = this.py;
		}

	}
}

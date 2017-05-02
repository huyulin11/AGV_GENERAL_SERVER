namespace AGV.forklift {

	/// <summary>
	/// 描述车子的集合及其管理逻辑
	/// </summary>
	public interface IForkLiftWrappersService {

		void connectForks();

		ForkLiftWrapper getForkLiftByNunber(int number);
	}
}

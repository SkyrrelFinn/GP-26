using UnityEngine;

namespace AA0000
{
	public interface IRollable
	{
		void RollObject();

		float GetSideFacingTowardDirection(Vector3? direction = default);
	} 
}

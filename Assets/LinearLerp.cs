using UnityEngine;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class LinearLerp : MonoBehaviour
	{
		public Transform startPosition;
		public Transform endPosition;
		public LinearMapping linearMapping;

		protected virtual void Update()
		{
			transform.position = Vector3.Lerp(startPosition.position, endPosition.position, linearMapping.value);

		}
	}
}

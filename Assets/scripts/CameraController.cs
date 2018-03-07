using UnityEngine;

namespace DistributionPrototype
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] private float _speed;

		private void Update()
		{
			float horizontalAxis = Input.GetAxis("Horizontal");
			float verticalAxis = Input.GetAxis("Vertical");

			var movement = new Vector3(_speed * horizontalAxis, 0f, _speed * verticalAxis);
			transform.position = transform.position + movement;
		}
	}
}

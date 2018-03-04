using UnityEngine;
using UnityEngine.SceneManagement;

namespace DistributionPrototype
{
	public class MainController : MonoBehaviour
	{
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}
	}
}

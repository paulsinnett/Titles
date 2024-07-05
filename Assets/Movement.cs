using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
	public float speed = 10f;

	class FakeInput
	{
		float current = 0f;
		float target = 0f;
		float sensitivity = 3f;

		public void SetAxis(string _, float value)
		{
			target = value;
		}

		public void Update()
		{
			current = Mathf.MoveTowards(current, target, Time.deltaTime * sensitivity);
		}

		public float GetAxis(string _)
		{
			return current;
		}
	}

	FakeInput input = new FakeInput();

	IEnumerator Start()
	{
		Application.targetFrameRate = 60;
		input.SetAxis("Horizontal", 1f);
		yield return new WaitForSeconds(1f);
		for (;;)
		{
			input.SetAxis("Horizontal", 0f);
			yield return new WaitForSeconds(1f);
			input.SetAxis("Horizontal", -1f);
			yield return new WaitForSeconds(2f);
			input.SetAxis("Horizontal", 0f);
			yield return new WaitForSeconds(1f);
			input.SetAxis("Horizontal", 1f);
			yield return new WaitForSeconds(2f);
		}
	}

	void Update()
	{
		input.Update();
		Vector3 position = transform.position;
		float h = input.GetAxis("Horizontal");
		position.x += h * Time.deltaTime;
		transform.position = position;
	}
}

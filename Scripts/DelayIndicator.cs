using UnityEngine;
using UnityEngine.UI;

public class DelayIndicator : MonoBehaviour
{
	[SerializeField] Image LoadingBar;
	[SerializeField] bool showText;
	[SerializeField] Text TextIndicator;
	float givenTime;
	float timeLeft;
	bool onDelay = false;

	void Start()
	{
		if (!showText)
		{
			TextIndicator.transform.gameObject.SetActive(false);
		}
	}

    void Update()
    {
		if (!onDelay) { return; }
		if (timeLeft <= 0)
		{
			LoadingBar.fillAmount = 1f;
			onDelay = false;
			TextIndicator.text = "0";
			EndDelay();
		}
		else 
		{
			timeLeft -= Time.deltaTime;
			LoadingBar.fillAmount = 1 - timeLeft / givenTime;
			TextIndicator.text = ((int)Mathf.Ceil(timeLeft)).ToString();
		}
    }

	public void SetDelay(float time)
	{
		givenTime = time;
		timeLeft = time;
		onDelay = true;
	}

	protected virtual void EndDelay()
	{
		Debug.Log("Delay ended");
	}
}

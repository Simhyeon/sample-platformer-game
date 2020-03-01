using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class NpcDialogue : MonoBehaviour
{
	[SerializeField] GameObject dialogue;
	[SerializeField] Text dialogueText;
	string[] dialogueArray;
	int arrayIndex = 0;
	bool onDialgoue= false;

	void Start()
	{
		dialogue.SetActive(false);
	}

	void Update()
	{
		if (onDialgoue && CrossPlatformInputManager.GetButtonDown("Submit"))
		{
			NextDialogue();
		}
	}

	public void StartDialogue(string[] originalDialogue)
	{
		onDialgoue = true;
		dialogueArray = originalDialogue;
		arrayIndex = 0;
		dialogue.SetActive(true);
		dialogueText.text = dialogueArray[0];
		Time.timeScale = 0;
	}

	void NextDialogue()
	{
		arrayIndex +=1;
		if (arrayIndex >= dialogueArray.Length)
		{
			dialogue.SetActive(false);
			Time.timeScale = 1;
			onDialgoue = false;
			FindObjectOfType<PlayerController>().DialogueExit();
		}
		else 
		{
			dialogueText.text = dialogueArray[arrayIndex];
		}
	}
}

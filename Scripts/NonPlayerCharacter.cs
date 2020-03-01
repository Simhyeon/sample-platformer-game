using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
	[SerializeField] string[] dialogueArray;

	public string[] GetDialogue()
	{
		return dialogueArray;
	}
}

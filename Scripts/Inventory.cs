using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
	[SerializeField] GameObject indicatorPrefab;
	[SerializeField] float indicatorOffset;
	Dictionary<string, GameObject> items = new Dictionary<string, GameObject>();
	
	public void Add(string name, Sprite image)
	{
		GameObject spanwed = Instantiate(indicatorPrefab, new Vector2(
					GetComponent<RectTransform>().anchoredPosition.x + (items.Count + 1) * indicatorOffset, GetComponent<RectTransform>().anchoredPosition.y), 
				Quaternion.identity,
				transform);
		spanwed.GetComponent<Image>().sprite = image;
		items.Add(name, spanwed);
	}

	public bool UseItem(string itemName)
	{
		bool detected = false;
		foreach (var item in items.Keys)
		{
			if (item == itemName) 
			{
				detected = true;
				break;
			}
		}
		if (detected) { items.Remove(itemName);  return true;}
		else { return false; }
	}
}

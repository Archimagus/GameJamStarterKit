using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonAudioHandler : MonoBehaviour
{
	[SerializeField] private AudioClip _clickClip = null;
	[SerializeField] private AudioClip _hoverClip = null;

	void Start()
	{
		EventTrigger trigger = GetComponent<EventTrigger>();
		var entry = new EventTrigger.Entry{eventID = EventTriggerType.PointerClick};
		entry.callback.AddListener((data) => AudioManager.PlaySound(null, _clickClip, SoundType.Interface));
		trigger.triggers.Add(entry);
		entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
		entry.callback.AddListener((data) => AudioManager.PlaySound(null, _hoverClip, SoundType.Interface));
		trigger.triggers.Add(entry);
	}
}

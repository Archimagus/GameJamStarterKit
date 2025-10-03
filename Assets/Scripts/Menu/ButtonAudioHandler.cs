using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ButtonAudioHandler : MonoBehaviour
{
	[Tooltip("Clip played when the button is clicked.")]
	[SerializeField] private AudioClip _clickClip = null;
	[Tooltip("Clip played when the pointer hovers over the button.")]
	[SerializeField] private AudioClip _hoverClip = null;

	/// <summary>
	/// Registers UI event triggers for hover and click to play interface sounds.
	/// </summary>
	void Start()
	{
		EventTrigger trigger = GetComponent<EventTrigger>();
		var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
		entry.callback.AddListener((data) => AudioManager.PlaySound(null, _clickClip, SoundType.Interface));
		trigger.triggers.Add(entry);
		entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
		entry.callback.AddListener((data) => AudioManager.PlaySound(null, _hoverClip, SoundType.Interface));
		trigger.triggers.Add(entry);
	}
}

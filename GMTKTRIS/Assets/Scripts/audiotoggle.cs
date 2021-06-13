using UnityEngine;

public class audiotoggle : MonoBehaviour
{
    private AudioSource source;

    private void Start() => source = GetComponent<AudioSource>();

    public void Toggle() => source.mute = !source.mute;
}
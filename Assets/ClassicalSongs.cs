using UnityEngine;
using System.Collections;

[System.Serializable]
public class ClassicalSongs {
	[System.Serializable]
	public class Segment
	{
		public int Tampo;
		public string startingTone;
		public string endingTone;
		public AudioClip Clip;
	}

	public enum RhythmType
	{
		Adagio, Moderato,  Allegro 
	}

	public string Name;
	public int AvargeTampo;
	public string Tone;
	public Segment[] SongSegment;
}

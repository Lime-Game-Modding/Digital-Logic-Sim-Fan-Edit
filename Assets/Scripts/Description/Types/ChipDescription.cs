using System;
using UnityEngine;

//FanEdit
using System.ComponentModel;

namespace DLS.Description
{
	public class ChipDescription
	{
		// ---- Name Comparion ----
		public const StringComparison NameComparison = StringComparison.OrdinalIgnoreCase;
		public static readonly StringComparer NameComparer = StringComparer.OrdinalIgnoreCase;
		public ChipType ChipType;
		public Color Colour;
		public DisplayDescription[] Displays;

		public PinDescription[] InputPins;

		// ---- Data ----
		public string Name;
		public NameDisplayLocation NameLocation;
		public PinDescription[] OutputPins;
		public Vector2 Size;
		public SubChipDescription[] SubChips;
		public WireDescription[] Wires;

		// ---- Convenience Functions ----
		public bool HasDisplay() => Displays != null && Displays.Length > 0;
		public bool NameMatch(string otherName) => NameMatch(Name, otherName);
		public static bool NameMatch(string a, string b) => string.Equals(a, b, NameComparison);

		// --- MODDED : Fan Edit --- //

		// -- Data -- //
		[DefaultValue(NameAlignment.Centre)]
		public NameAlignment NameAlignment;


		// ------------------------- //
	}

	public enum NameDisplayLocation
	{
		Centre,
		Top,
		Hidden
	}

	// --- MODDED : Fan Edit --- //
	public enum NameAlignment
	{
		Centre,
		Right,
		Left
	}
	// ------------------------- //
}
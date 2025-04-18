using System;
using Seb.Helpers;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Seb.Vis.UI
{
	public class WheelSelectorState
	{
		public int index;
	}

	public class ScrollBarState
	{
		public float dragScrollOffset;
		public bool isDragging;
		public float scrollY;
	}

	public struct SliderState
	{
		public float progressT;
		public bool handleSelected;
	}

	public class ColourPickerState
	{
		public float hue;
		public float sat;
		public float val;
		public bool satValHandleSelected;
		public bool hueHandleSelected;

		public Color GetRGB() => Color.HSVToRGB(hue, sat, val);

		public void SetRGB(Color col)
		{
			(hue, sat, val) = ColHelper.GetHSV(col);
		}
	}


	public class ButtonState
	{
		int buttonPressFrame;
		int buttonReleaseFrame;
		public bool isDown;
		public bool toggleState;

		public bool ButtonPressedThisFrame => buttonPressFrame == Time.frameCount;
		public bool ButtonReleasedThisFrame => buttonReleaseFrame == Time.frameCount;

		public void NotifyPressed()
		{
			isDown = true;
			buttonPressFrame = Time.frameCount;
		}

		public void NotifyReleased()
		{
			isDown = false;
			buttonReleaseFrame = Time.frameCount;
		}

		public void NotifyCancelled()
		{
			isDown = false;
		}
	}

	public class InputFieldState
	{
		public TriggerState arrowKeyTrigger;
		public TriggerState backspaceTrigger;
		public int cursorBeforeCharIndex;
		public TriggerState deleteTrigger;
		public bool isMouseDownInBounds;
		public bool isSelecting;
		public float lastInputTime;
		public int selectionStartIndex;
		public string text { get; private set; } = string.Empty;
		public bool focused { get; private set; }

		public int SelectionMinIndex => Mathf.Min(selectionStartIndex, cursorBeforeCharIndex);
		public int SelectionMaxIndex => Mathf.Max(selectionStartIndex, cursorBeforeCharIndex);

		public void ClearText()
		{
			text = string.Empty;
			cursorBeforeCharIndex = 0;
			isSelecting = false;
		}

		public void SetFocus(bool newFocusState)
		{
			if (newFocusState != focused)
			{
				focused = newFocusState;
				lastInputTime = Time.time;

				if (newFocusState == false)
				{
					isSelecting = false;
				}
			}
		}

		public void Delete(bool deleteLeft, Func<string, bool> validation = null)
		{
			if (text.Length == 0) return;

			if (isSelecting)
			{
				isSelecting = false;
				int deleteStartIndex = Math.Min(cursorBeforeCharIndex, selectionStartIndex);
				int deleteCount = Math.Abs(cursorBeforeCharIndex - selectionStartIndex);
				string newText = text.Remove(deleteStartIndex, deleteCount);

				if (validation == null || validation(newText))
				{
					text = newText;
					if (cursorBeforeCharIndex > selectionStartIndex)
					{
						SetCursorIndex(selectionStartIndex);
					}
				}
			}
			else
			{
				if (deleteLeft && cursorBeforeCharIndex > 0)
				{
					string newText = text.Remove(cursorBeforeCharIndex - 1, 1);
					if (validation == null || validation(newText))
					{
						text = newText;
						DecrementCursor();
					}
				}
				else if (!deleteLeft && cursorBeforeCharIndex < text.Length)
				{
					string newText = text.Remove(cursorBeforeCharIndex, 1);
					if (validation == null || validation(newText)) text = newText;
				}
			}

			UpdateLastInputTime();
		}

		public void NewLine()
		{
			if (isSelecting) Delete(true);
			TryInsertText("\n");
		}

		public int CountLines()
		{
			return text.Count(c => c == '\n');
		}

		public void WrapText(int maxCharsPerLine)
		{
			if (maxCharsPerLine <= 0 || string.IsNullOrEmpty(text))
				return;

			string[] lines = text.Split('\n');
			string wrappedText = "";

			foreach (string line in lines)
			{
				string unwrappedLine = line.Replace("\n", "");  // Remove any newlines inside the line itself
				string lineWithWrap = "";

				for (int i = 0; i < unwrappedLine.Length; i++)
				{	
					if (i > 0 && i % maxCharsPerLine == 0)
						lineWithWrap += "\n";

					lineWithWrap += unwrappedLine[i];
				}

				if (wrappedText.Length > 0)
					wrappedText += "\n";
				
				wrappedText += lineWithWrap;
			}

			text = wrappedText;
			SetCursorIndex(text.Length); // move cursor to the end
		}

		public void EnforceTextLimit(int lineLength, int maxLines)
		{
			// Split the text into lines
			string[] lines = text.Split('\n');
			List<string> limitedLines = new();

			foreach (string line in lines)
			{
				// Wrap the line if it exceeds the lineLength
				for (int i = 0; i < line.Length; i += lineLength)
				{
					if (limitedLines.Count >= maxLines)
						break;

					string wrappedLine = line.Substring(i, Math.Min(lineLength, line.Length - i));
					limitedLines.Add(wrappedLine);
				}

				if (limitedLines.Count >= maxLines)
					break;

				// Preserve manual newlines
				if (line.Length == 0 && limitedLines.Count < maxLines)
				{
					limitedLines.Add(string.Empty);
				}
			}

			// Join the limited lines back into a single string
			text = string.Join("\n", limitedLines);

			// Adjust the cursor position if it exceeds the new text length
			cursorBeforeCharIndex = Mathf.Clamp(cursorBeforeCharIndex, 0, text.Length);
		}

		public void SelectAll()
		{
			SetCursorIndex(0);
			SetCursorIndex(text.Length, true);
		}

		public void SetCursorIndex(int i, bool select = false)
		{
			if (select && !isSelecting)
			{
				isSelecting = true;
				selectionStartIndex = cursorBeforeCharIndex;
			}

			cursorBeforeCharIndex = i;
			cursorBeforeCharIndex = Mathf.Clamp(cursorBeforeCharIndex, 0, text.Length);
			UpdateLastInputTime();

			if (cursorBeforeCharIndex == selectionStartIndex || !select)
			{
				isSelecting = false;
			}
		}

		public void UpdateLastInputTime()
		{
			lastInputTime = Time.time;
		}

		public void SetText(string newText, bool focus = true)
		{
			if (string.IsNullOrEmpty(newText)) text = string.Empty;
			else text = newText;

			SetCursorIndex(text.Length);
			SetFocus(focus);
		}

		public void TryInsertText(string textToAdd, Func<string, bool> validation = null)
		{
			string originalText = text;

			if (isSelecting) Delete(true);

			string newText = text;
			if (cursorBeforeCharIndex == text.Length) newText += textToAdd;
			else newText = newText.Insert(cursorBeforeCharIndex, textToAdd);

			if (validation == null || validation(newText))
			{
				text = newText;
				SetCursorIndex(cursorBeforeCharIndex + textToAdd.Length);
			}
			else text = originalText;
		}

		public void IncrementCursor(bool select = false)
		{
			if (isSelecting && !select) SetCursorIndex(SelectionMaxIndex); // jump to end of active selection
			else SetCursorIndex(cursorBeforeCharIndex + 1, select);
		}

		public void DecrementCursor(bool select = false)
		{
			if (isSelecting && !select) SetCursorIndex(SelectionMinIndex); // jump to start of active selection
			else SetCursorIndex(cursorBeforeCharIndex - 1, select);
		}

		public int NextWordEndIndex()
		{
			bool hasEncounteredNonSpaceChar = false;

			for (int i = cursorBeforeCharIndex; i < text.Length; i++)
			{
				if (char.IsWhiteSpace(text[i]) && hasEncounteredNonSpaceChar) return i;
				if (!char.IsWhiteSpace(text[i])) hasEncounteredNonSpaceChar = true;
			}

			return text.Length;
		}

		public int PrevWordIndex()
		{
			bool hasEncounteredNonSpaceChar = false;

			for (int i = cursorBeforeCharIndex - 1; i >= 0; i--)
			{
				if (char.IsWhiteSpace(text[i]) && hasEncounteredNonSpaceChar) return i + 1;
				if (!char.IsWhiteSpace(text[i])) hasEncounteredNonSpaceChar = true;
			}

			return 0;
		}

		public struct TriggerState
		{
			public float lastManualTime;
			public float lastAutoTiggerTime;
		}
	}
}
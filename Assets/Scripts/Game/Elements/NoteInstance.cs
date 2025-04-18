using System;
using DLS.Graphics;
using Seb.Helpers;
using Seb.Types;
using UnityEngine;
using static DLS.Graphics.DrawSettings;
using DLS.Description;

namespace DLS.Game
{
    public class NoteInstance : IMoveable
    {
        public readonly NoteDescription Description;
        // Position of the note in the simulation
        public Vector2 Position { get; set; }

        // Position when the move operation started
        public Vector2 MoveStartPosition { get; set; }

        // Reference point for straight-line movement
        public Vector2 StraightLineReferencePoint { get; set; }

        // Indicates if a reference point for straight-line movement exists
        public bool HasReferencePointForStraightLineMovement { get; set; }

        // Indicates if the note is currently selected
        public bool IsSelected { get; set; }

        // Indicates if the current position is valid for movement
        public bool IsValidMovePos { get; set; }

        // Snap point for grid alignment (can be overridden if needed)
        public virtual Vector2 SnapPoint => Position;

        // Bounding box for selection

        // Bounding box for the note
        public Bounds2D BoundingBox => Bounds2D.CreateFromCentreAndSize(Position + Size / 2, Size);
        public virtual Bounds2D SelectionBoundingBox => Bounds2D.CreateFromCentreAndSize(Position + Size / 2, Size + new Vector2(DrawSettings.ChipOutlineWidth + DrawSettings.SelectionBoundsPadding, DrawSettings.ChipOutlineWidth + DrawSettings.SelectionBoundsPadding));

        // Unique identifier for the note
        public int ID { get; private set; }

        // Text content of the note
        public string Text { get; set; }

        // Dimensions of the note
        public Vector2 Size { get; set; }
        public NoteColour Colour;

        // Constructor
        public NoteInstance(NoteDescription desc)
        {
            Description = desc;
            ID = desc.ID;
            Position = desc.Position;
            Text = desc.Text;
            Size = desc.Size;
            IsSelected = false;
            IsValidMovePos = true;
        }

        // Determines if the note should be included in a selection box
        public bool ShouldBeIncludedInSelectionBox(Vector2 selectionCentre, Vector2 selectionSize)
        {
            var halfSelectionSize = selectionSize / 2;
            var halfNoteSize = Size / 2;

            return Math.Abs(Position.x - selectionCentre.x) <= (halfSelectionSize.x + halfNoteSize.x) &&
                Math.Abs(Position.y - selectionCentre.y) <= (halfSelectionSize.y + halfNoteSize.y);
        }
    }
}
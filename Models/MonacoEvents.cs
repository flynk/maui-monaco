using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flynk.Apps.Maui.Monaco.Models
{
    /// <summary>
    /// Base event args for Monaco editor events
    /// </summary>
    public class MonacoEventArgs : EventArgs
    {
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Event args for content change events
    /// </summary>
    public class ContentChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("changes")]
        public List<ContentChange> Changes { get; set; }

        [JsonPropertyName("eol")]
        public string Eol { get; set; }

        [JsonPropertyName("versionId")]
        public int VersionId { get; set; }

        [JsonPropertyName("isFlush")]
        public bool IsFlush { get; set; }

        [JsonPropertyName("isRedoing")]
        public bool IsRedoing { get; set; }

        [JsonPropertyName("isUndoing")]
        public bool IsUndoing { get; set; }
    }

    public class ContentChange
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("rangeLength")]
        public int RangeLength { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("rangeOffset")]
        public int RangeOffset { get; set; }

        [JsonPropertyName("forceMoveMarkers")]
        public bool ForceMoveMarkers { get; set; }
    }

    /// <summary>
    /// Event args for cursor position change
    /// </summary>
    public class CursorPositionChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("secondaryPositions")]
        public List<Position> SecondaryPositions { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }
    }

    /// <summary>
    /// Event args for cursor selection change
    /// </summary>
    public class CursorSelectionChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("selection")]
        public Selection Selection { get; set; }

        [JsonPropertyName("secondarySelections")]
        public List<Selection> SecondarySelections { get; set; }

        [JsonPropertyName("modelVersionId")]
        public int ModelVersionId { get; set; }

        [JsonPropertyName("oldSelections")]
        public List<Selection> OldSelections { get; set; }

        [JsonPropertyName("oldModelVersionId")]
        public int OldModelVersionId { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }

    /// <summary>
    /// Event args for model change
    /// </summary>
    public class ModelChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("oldModelUri")]
        public string OldModelUri { get; set; }

        [JsonPropertyName("newModelUri")]
        public string NewModelUri { get; set; }

        [JsonPropertyName("oldLanguage")]
        public string OldLanguage { get; set; }

        [JsonPropertyName("newLanguage")]
        public string NewLanguage { get; set; }
    }

    /// <summary>
    /// Event args for model decorations change
    /// </summary>
    public class ModelDecorationsChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("affectsMinimap")]
        public bool AffectsMinimap { get; set; }

        [JsonPropertyName("affectsOverviewRuler")]
        public bool AffectsOverviewRuler { get; set; }
    }

    /// <summary>
    /// Event args for model language change
    /// </summary>
    public class ModelLanguageChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("oldLanguage")]
        public string OldLanguage { get; set; }

        [JsonPropertyName("newLanguage")]
        public string NewLanguage { get; set; }
    }

    /// <summary>
    /// Event args for model options change
    /// </summary>
    public class ModelOptionsChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("tabSize")]
        public bool TabSize { get; set; }

        [JsonPropertyName("indentSize")]
        public bool IndentSize { get; set; }

        [JsonPropertyName("insertSpaces")]
        public bool InsertSpaces { get; set; }

        [JsonPropertyName("trimAutoWhitespace")]
        public bool TrimAutoWhitespace { get; set; }
    }

    /// <summary>
    /// Event args for configuration change
    /// </summary>
    public class ConfigurationChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("hasChanged")]
        public Dictionary<string, bool> HasChanged { get; set; }
    }

    /// <summary>
    /// Event args for layout change
    /// </summary>
    public class LayoutChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("glyphMarginLeft")]
        public int GlyphMarginLeft { get; set; }

        [JsonPropertyName("glyphMarginWidth")]
        public int GlyphMarginWidth { get; set; }

        [JsonPropertyName("lineNumbersLeft")]
        public int LineNumbersLeft { get; set; }

        [JsonPropertyName("lineNumbersWidth")]
        public int LineNumbersWidth { get; set; }

        [JsonPropertyName("decorationsLeft")]
        public int DecorationsLeft { get; set; }

        [JsonPropertyName("decorationsWidth")]
        public int DecorationsWidth { get; set; }

        [JsonPropertyName("contentLeft")]
        public int ContentLeft { get; set; }

        [JsonPropertyName("contentWidth")]
        public int ContentWidth { get; set; }

        [JsonPropertyName("renderMinimap")]
        public int RenderMinimap { get; set; }

        [JsonPropertyName("minimapLeft")]
        public int MinimapLeft { get; set; }

        [JsonPropertyName("minimapWidth")]
        public int MinimapWidth { get; set; }

        [JsonPropertyName("viewportColumn")]
        public int ViewportColumn { get; set; }

        [JsonPropertyName("isWordWrapMinified")]
        public bool IsWordWrapMinified { get; set; }

        [JsonPropertyName("isViewportWrapping")]
        public bool IsViewportWrapping { get; set; }

        [JsonPropertyName("wrappingColumn")]
        public int WrappingColumn { get; set; }

        [JsonPropertyName("verticalScrollbarWidth")]
        public int VerticalScrollbarWidth { get; set; }

        [JsonPropertyName("horizontalScrollbarHeight")]
        public int HorizontalScrollbarHeight { get; set; }

        [JsonPropertyName("overviewRuler")]
        public OverviewRulerPosition OverviewRuler { get; set; }
    }

    public class OverviewRulerPosition
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("top")]
        public int Top { get; set; }

        [JsonPropertyName("right")]
        public int Right { get; set; }
    }

    /// <summary>
    /// Event args for paste events
    /// </summary>
    public class PasteEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("languageId")]
        public string LanguageId { get; set; }
    }

    /// <summary>
    /// Event args for scroll change
    /// </summary>
    public class ScrollChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("scrollTop")]
        public double ScrollTop { get; set; }

        [JsonPropertyName("scrollLeft")]
        public double ScrollLeft { get; set; }

        [JsonPropertyName("scrollWidth")]
        public double ScrollWidth { get; set; }

        [JsonPropertyName("scrollHeight")]
        public double ScrollHeight { get; set; }

        [JsonPropertyName("scrollTopChanged")]
        public bool ScrollTopChanged { get; set; }

        [JsonPropertyName("scrollLeftChanged")]
        public bool ScrollLeftChanged { get; set; }

        [JsonPropertyName("scrollWidthChanged")]
        public bool ScrollWidthChanged { get; set; }

        [JsonPropertyName("scrollHeightChanged")]
        public bool ScrollHeightChanged { get; set; }
    }

    /// <summary>
    /// Event args for keyboard events
    /// </summary>
    public class KeyboardEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("browserEvent")]
        public BrowserKeyboardEvent BrowserEvent { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("ctrlKey")]
        public bool CtrlKey { get; set; }

        [JsonPropertyName("shiftKey")]
        public bool ShiftKey { get; set; }

        [JsonPropertyName("altKey")]
        public bool AltKey { get; set; }

        [JsonPropertyName("metaKey")]
        public bool MetaKey { get; set; }

        [JsonPropertyName("keyCode")]
        public int KeyCode { get; set; }
    }

    public class BrowserKeyboardEvent
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("location")]
        public int Location { get; set; }

        [JsonPropertyName("repeat")]
        public bool Repeat { get; set; }
    }

    /// <summary>
    /// Event args for mouse events
    /// </summary>
    public class MouseEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("event")]
        public BrowserMouseEvent Event { get; set; }

        [JsonPropertyName("target")]
        public MouseTarget Target { get; set; }
    }

    public class BrowserMouseEvent
    {
        [JsonPropertyName("buttons")]
        public int Buttons { get; set; }

        [JsonPropertyName("detail")]
        public int Detail { get; set; }

        [JsonPropertyName("posx")]
        public int PosX { get; set; }

        [JsonPropertyName("posy")]
        public int PosY { get; set; }

        [JsonPropertyName("ctrlKey")]
        public bool CtrlKey { get; set; }

        [JsonPropertyName("shiftKey")]
        public bool ShiftKey { get; set; }

        [JsonPropertyName("altKey")]
        public bool AltKey { get; set; }

        [JsonPropertyName("metaKey")]
        public bool MetaKey { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }

    public class MouseTarget
    {
        [JsonPropertyName("element")]
        public string Element { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("position")]
        public Position Position { get; set; }

        [JsonPropertyName("mouseColumn")]
        public int MouseColumn { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }
    }

    /// <summary>
    /// Event args for context menu
    /// </summary>
    public class ContextMenuEventArgs : MouseEventArgs
    {
    }

    /// <summary>
    /// Event args for content size change
    /// </summary>
    public class ContentSizeChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("contentWidth")]
        public int ContentWidth { get; set; }

        [JsonPropertyName("contentHeight")]
        public int ContentHeight { get; set; }

        [JsonPropertyName("contentWidthChanged")]
        public bool ContentWidthChanged { get; set; }

        [JsonPropertyName("contentHeightChanged")]
        public bool ContentHeightChanged { get; set; }
    }

    /// <summary>
    /// Event args for visible ranges change
    /// </summary>
    public class VisibleRangesChangedEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("visibleRanges")]
        public List<Range> VisibleRanges { get; set; }
    }

    /// <summary>
    /// Event args for composition events
    /// </summary>
    public class CompositionEventArgs : MonacoEventArgs
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
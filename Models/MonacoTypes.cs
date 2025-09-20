using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flynk.Apps.Maui.Monaco.Models
{
    /// <summary>
    /// A position in the editor. This interface is suitable for serialization.
    /// </summary>
    public class Position
    {
        [JsonPropertyName("lineNumber")]
        public int LineNumber { get; set; }

        [JsonPropertyName("column")]
        public int Column { get; set; }

        public Position() { }

        public Position(int lineNumber, int column)
        {
            LineNumber = lineNumber;
            Column = column;
        }

        public override string ToString()
        {
            return $"({LineNumber},{Column})";
        }
    }

    /// <summary>
    /// A range in the editor. This interface is suitable for serialization.
    /// </summary>
    public class Range
    {
        [JsonPropertyName("startLineNumber")]
        public int StartLineNumber { get; set; }

        [JsonPropertyName("startColumn")]
        public int StartColumn { get; set; }

        [JsonPropertyName("endLineNumber")]
        public int EndLineNumber { get; set; }

        [JsonPropertyName("endColumn")]
        public int EndColumn { get; set; }

        public Range() { }

        public Range(int startLineNumber, int startColumn, int endLineNumber, int endColumn)
        {
            StartLineNumber = startLineNumber;
            StartColumn = startColumn;
            EndLineNumber = endLineNumber;
            EndColumn = endColumn;
        }

        public override string ToString()
        {
            return $"[({StartLineNumber},{StartColumn})-({EndLineNumber},{EndColumn})]";
        }
    }

    /// <summary>
    /// A selection in the editor. This interface is suitable for serialization.
    /// </summary>
    public class Selection : Range
    {
        [JsonPropertyName("selectionStartLineNumber")]
        public int SelectionStartLineNumber { get; set; }

        [JsonPropertyName("selectionStartColumn")]
        public int SelectionStartColumn { get; set; }

        [JsonPropertyName("positionLineNumber")]
        public int PositionLineNumber { get; set; }

        [JsonPropertyName("positionColumn")]
        public int PositionColumn { get; set; }

        public Selection() { }

        public Selection(int selectionStartLineNumber, int selectionStartColumn, int positionLineNumber, int positionColumn)
            : base(
                Math.Min(selectionStartLineNumber, positionLineNumber),
                selectionStartLineNumber < positionLineNumber ? selectionStartColumn : (selectionStartLineNumber > positionLineNumber ? positionColumn : Math.Min(selectionStartColumn, positionColumn)),
                Math.Max(selectionStartLineNumber, positionLineNumber),
                selectionStartLineNumber < positionLineNumber ? positionColumn : (selectionStartLineNumber > positionLineNumber ? selectionStartColumn : Math.Max(selectionStartColumn, positionColumn)))
        {
            SelectionStartLineNumber = selectionStartLineNumber;
            SelectionStartColumn = selectionStartColumn;
            PositionLineNumber = positionLineNumber;
            PositionColumn = positionColumn;
        }
    }

    /// <summary>
    /// A text edit operation.
    /// </summary>
    public class TextEdit
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("forceMoveMarkers")]
        public bool? ForceMoveMarkers { get; set; }
    }

    /// <summary>
    /// A single edit operation, that has an identifier.
    /// </summary>
    public class IdentifiedSingleEditOperation : TextEdit
    {
    }

    /// <summary>
    /// Word range in the editor
    /// </summary>
    public class WordAtPosition
    {
        [JsonPropertyName("word")]
        public string Word { get; set; }

        [JsonPropertyName("startColumn")]
        public int StartColumn { get; set; }

        [JsonPropertyName("endColumn")]
        public int EndColumn { get; set; }
    }

    /// <summary>
    /// Describes the behavior of decorations when typing/editing near their edges.
    /// </summary>
    public enum TrackedRangeStickiness
    {
        AlwaysGrowsWhenTypingAtEdges = 0,
        NeverGrowsWhenTypingAtEdges = 1,
        GrowsOnlyWhenTypingBefore = 2,
        GrowsOnlyWhenTypingAfter = 3
    }

    /// <summary>
    /// Options for a model decoration.
    /// </summary>
    public class ModelDecorationOptions
    {
        [JsonPropertyName("stickiness")]
        public TrackedRangeStickiness? Stickiness { get; set; }

        [JsonPropertyName("className")]
        public string ClassName { get; set; }

        [JsonPropertyName("glyphMarginClassName")]
        public string GlyphMarginClassName { get; set; }

        [JsonPropertyName("hoverMessage")]
        public object HoverMessage { get; set; }

        [JsonPropertyName("isWholeLine")]
        public bool? IsWholeLine { get; set; }

        [JsonPropertyName("showIfCollapsed")]
        public bool? ShowIfCollapsed { get; set; }

        [JsonPropertyName("afterContentClassName")]
        public string AfterContentClassName { get; set; }

        [JsonPropertyName("beforeContentClassName")]
        public string BeforeContentClassName { get; set; }

        [JsonPropertyName("inlineClassName")]
        public string InlineClassName { get; set; }

        [JsonPropertyName("inlineClassNameAffectsLetterSpacing")]
        public bool? InlineClassNameAffectsLetterSpacing { get; set; }

        [JsonPropertyName("zIndex")]
        public int? ZIndex { get; set; }

        [JsonPropertyName("overviewRuler")]
        public ModelDecorationOverviewRulerOptions OverviewRuler { get; set; }

        [JsonPropertyName("minimap")]
        public ModelDecorationMinimapOptions Minimap { get; set; }

        [JsonPropertyName("glyphMarginHoverMessage")]
        public object GlyphMarginHoverMessage { get; set; }

        [JsonPropertyName("linesDecorationsClassName")]
        public string LinesDecorationsClassName { get; set; }

        [JsonPropertyName("firstLineDecorationClassName")]
        public string FirstLineDecorationClassName { get; set; }

        [JsonPropertyName("marginClassName")]
        public string MarginClassName { get; set; }
    }

    /// <summary>
    /// Options for rendering a model decoration in the overview ruler.
    /// </summary>
    public class ModelDecorationOverviewRulerOptions
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("darkColor")]
        public string DarkColor { get; set; }

        [JsonPropertyName("position")]
        public OverviewRulerLane Position { get; set; }
    }

    /// <summary>
    /// Options for rendering a model decoration in the minimap.
    /// </summary>
    public class ModelDecorationMinimapOptions : ModelDecorationOverviewRulerOptions
    {
    }

    /// <summary>
    /// Position in the overview ruler.
    /// </summary>
    public enum OverviewRulerLane
    {
        Left = 1,
        Center = 2,
        Right = 4,
        Full = 7
    }

    /// <summary>
    /// New model decoration.
    /// </summary>
    public class ModelDeltaDecoration
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("options")]
        public ModelDecorationOptions Options { get; set; }
    }

    /// <summary>
    /// A decoration in the model.
    /// </summary>
    public class ModelDecoration
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("options")]
        public ModelDecorationOptions Options { get; set; }
    }

    /// <summary>
    /// A marker data structure
    /// </summary>
    public class MarkerData
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("severity")]
        public MarkerSeverity Severity { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("startLineNumber")]
        public int StartLineNumber { get; set; }

        [JsonPropertyName("startColumn")]
        public int StartColumn { get; set; }

        [JsonPropertyName("endLineNumber")]
        public int EndLineNumber { get; set; }

        [JsonPropertyName("endColumn")]
        public int EndColumn { get; set; }

        [JsonPropertyName("relatedInformation")]
        public List<RelatedInformation> RelatedInformation { get; set; }

        [JsonPropertyName("tags")]
        public List<MarkerTag> Tags { get; set; }
    }

    /// <summary>
    /// Marker severity
    /// </summary>
    public enum MarkerSeverity
    {
        Hint = 1,
        Info = 2,
        Warning = 4,
        Error = 8
    }

    /// <summary>
    /// Marker tags
    /// </summary>
    public enum MarkerTag
    {
        Unnecessary = 1,
        Deprecated = 2
    }

    /// <summary>
    /// Related information for a marker
    /// </summary>
    public class RelatedInformation
    {
        [JsonPropertyName("resource")]
        public string Resource { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("startLineNumber")]
        public int StartLineNumber { get; set; }

        [JsonPropertyName("startColumn")]
        public int StartColumn { get; set; }

        [JsonPropertyName("endLineNumber")]
        public int EndLineNumber { get; set; }

        [JsonPropertyName("endColumn")]
        public int EndColumn { get; set; }
    }

    /// <summary>
    /// A command that should be invoked when the action is executed.
    /// </summary>
    public class Command
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("tooltip")]
        public string Tooltip { get; set; }

        [JsonPropertyName("arguments")]
        public List<object> Arguments { get; set; }
    }

    /// <summary>
    /// Completion item kinds
    /// </summary>
    public enum CompletionItemKind
    {
        Method = 0,
        Function = 1,
        Constructor = 2,
        Field = 3,
        Variable = 4,
        Class = 5,
        Struct = 6,
        Interface = 7,
        Module = 8,
        Property = 9,
        Event = 10,
        Operator = 11,
        Unit = 12,
        Value = 13,
        Constant = 14,
        Enum = 15,
        EnumMember = 16,
        Keyword = 17,
        Text = 18,
        Color = 19,
        File = 20,
        Reference = 21,
        Customcolor = 22,
        Folder = 23,
        TypeParameter = 24,
        User = 25,
        Issue = 26,
        Snippet = 27
    }

    /// <summary>
    /// Completion item tags
    /// </summary>
    public enum CompletionItemTag
    {
        Deprecated = 1
    }

    /// <summary>
    /// A completion item represents a text snippet that is proposed to complete text that is being typed.
    /// </summary>
    public class CompletionItem
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("kind")]
        public CompletionItemKind Kind { get; set; }

        [JsonPropertyName("tags")]
        public List<CompletionItemTag> Tags { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("documentation")]
        public string Documentation { get; set; }

        [JsonPropertyName("sortText")]
        public string SortText { get; set; }

        [JsonPropertyName("filterText")]
        public string FilterText { get; set; }

        [JsonPropertyName("preselect")]
        public bool Preselect { get; set; }

        [JsonPropertyName("insertText")]
        public string InsertText { get; set; }

        [JsonPropertyName("insertTextRules")]
        public CompletionItemInsertTextRule? InsertTextRules { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("commitCharacters")]
        public List<string> CommitCharacters { get; set; }

        [JsonPropertyName("additionalTextEdits")]
        public List<TextEdit> AdditionalTextEdits { get; set; }

        [JsonPropertyName("command")]
        public Command Command { get; set; }
    }

    /// <summary>
    /// Completion item insert text rules
    /// </summary>
    [Flags]
    public enum CompletionItemInsertTextRule
    {
        None = 0,
        KeepWhitespace = 1,
        InsertAsSnippet = 4
    }

    /// <summary>
    /// Represents a list of completion items
    /// </summary>
    public class CompletionList
    {
        [JsonPropertyName("suggestions")]
        public List<CompletionItem> Suggestions { get; set; }

        [JsonPropertyName("incomplete")]
        public bool Incomplete { get; set; }

        [JsonPropertyName("dispose")]
        public Action Dispose { get; set; }
    }

    /// <summary>
    /// Folding range
    /// </summary>
    public class FoldingRange
    {
        [JsonPropertyName("start")]
        public int Start { get; set; }

        [JsonPropertyName("end")]
        public int End { get; set; }

        [JsonPropertyName("kind")]
        public FoldingRangeKind? Kind { get; set; }
    }

    /// <summary>
    /// Folding range kinds
    /// </summary>
    public enum FoldingRangeKind
    {
        Comment,
        Imports,
        Region
    }

    /// <summary>
    /// Document highlight
    /// </summary>
    public class DocumentHighlight
    {
        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("kind")]
        public DocumentHighlightKind? Kind { get; set; }
    }

    /// <summary>
    /// Document highlight kinds
    /// </summary>
    public enum DocumentHighlightKind
    {
        Text = 0,
        Read = 1,
        Write = 2
    }

    /// <summary>
    /// Definition link
    /// </summary>
    public class DefinitionLink
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("targetSelectionRange")]
        public Range TargetSelectionRange { get; set; }
    }

    /// <summary>
    /// Document symbol
    /// </summary>
    public class DocumentSymbol
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("kind")]
        public SymbolKind Kind { get; set; }

        [JsonPropertyName("tags")]
        public List<SymbolTag> Tags { get; set; }

        [JsonPropertyName("containerName")]
        public string ContainerName { get; set; }

        [JsonPropertyName("range")]
        public Range Range { get; set; }

        [JsonPropertyName("selectionRange")]
        public Range SelectionRange { get; set; }

        [JsonPropertyName("children")]
        public List<DocumentSymbol> Children { get; set; }
    }

    /// <summary>
    /// Symbol kinds
    /// </summary>
    public enum SymbolKind
    {
        File = 0,
        Module = 1,
        Namespace = 2,
        Package = 3,
        Class = 4,
        Method = 5,
        Property = 6,
        Field = 7,
        Constructor = 8,
        Enum = 9,
        Interface = 10,
        Function = 11,
        Variable = 12,
        Constant = 13,
        String = 14,
        Number = 15,
        Boolean = 16,
        Array = 17,
        Object = 18,
        Key = 19,
        Null = 20,
        EnumMember = 21,
        Struct = 22,
        Event = 23,
        Operator = 24,
        TypeParameter = 25
    }

    /// <summary>
    /// Symbol tags
    /// </summary>
    public enum SymbolTag
    {
        Deprecated = 1
    }
}
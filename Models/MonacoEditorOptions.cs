using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Flynk.Apps.Maui.Monaco.Models
{
    /// <summary>
    /// Comprehensive Monaco Editor construction options matching the full Monaco API
    /// </summary>
    public class MonacoEditorOptions
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("language")]
        public string Language { get; set; } = "javascript";

        [JsonPropertyName("theme")]
        public string Theme { get; set; } = "vs-dark";

        [JsonPropertyName("automaticLayout")]
        public bool AutomaticLayout { get; set; } = true;

        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; } = 14;

        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; }

        [JsonPropertyName("fontWeight")]
        public string FontWeight { get; set; }

        [JsonPropertyName("lineHeight")]
        public int? LineHeight { get; set; }

        [JsonPropertyName("letterSpacing")]
        public double? LetterSpacing { get; set; }

        [JsonPropertyName("lineNumbers")]
        public string LineNumbers { get; set; } = "on"; // "on" | "off" | "relative" | "interval"

        [JsonPropertyName("rulers")]
        public int[] Rulers { get; set; }

        [JsonPropertyName("wordSeparators")]
        public string WordSeparators { get; set; }

        [JsonPropertyName("tabSize")]
        public int TabSize { get; set; } = 4;

        [JsonPropertyName("insertSpaces")]
        public bool InsertSpaces { get; set; } = true;

        [JsonPropertyName("detectIndentation")]
        public bool DetectIndentation { get; set; } = true;

        [JsonPropertyName("roundedSelection")]
        public bool RoundedSelection { get; set; } = true;

        [JsonPropertyName("scrollBeyondLastLine")]
        public bool ScrollBeyondLastLine { get; set; } = false;

        [JsonPropertyName("scrollBeyondLastColumn")]
        public int ScrollBeyondLastColumn { get; set; } = 5;

        [JsonPropertyName("smoothScrolling")]
        public bool SmoothScrolling { get; set; } = false;

        [JsonPropertyName("readOnly")]
        public bool ReadOnly { get; set; } = false;

        [JsonPropertyName("wordWrap")]
        public string WordWrap { get; set; } = "off"; // "off" | "on" | "wordWrapColumn" | "bounded"

        [JsonPropertyName("wordWrapColumn")]
        public int WordWrapColumn { get; set; } = 80;

        [JsonPropertyName("wordWrapBreakBeforeCharacters")]
        public string WordWrapBreakBeforeCharacters { get; set; }

        [JsonPropertyName("wordWrapBreakAfterCharacters")]
        public string WordWrapBreakAfterCharacters { get; set; }

        [JsonPropertyName("autoClosingBrackets")]
        public string AutoClosingBrackets { get; set; } = "languageDefined";

        [JsonPropertyName("autoClosingQuotes")]
        public string AutoClosingQuotes { get; set; } = "languageDefined";

        [JsonPropertyName("autoClosingComments")]
        public string AutoClosingComments { get; set; } = "languageDefined";

        [JsonPropertyName("autoSurround")]
        public string AutoSurround { get; set; } = "languageDefined";

        [JsonPropertyName("autoIndent")]
        public string AutoIndent { get; set; } = "advanced";

        [JsonPropertyName("formatOnType")]
        public bool FormatOnType { get; set; } = false;

        [JsonPropertyName("formatOnPaste")]
        public bool FormatOnPaste { get; set; } = false;

        [JsonPropertyName("suggestOnTriggerCharacters")]
        public bool SuggestOnTriggerCharacters { get; set; } = true;

        [JsonPropertyName("acceptSuggestionOnEnter")]
        public string AcceptSuggestionOnEnter { get; set; } = "on";

        [JsonPropertyName("acceptSuggestionOnCommitCharacter")]
        public bool AcceptSuggestionOnCommitCharacter { get; set; } = true;

        [JsonPropertyName("snippetSuggestions")]
        public string SnippetSuggestions { get; set; } = "inline";

        [JsonPropertyName("emptySelectionClipboard")]
        public bool EmptySelectionClipboard { get; set; } = true;

        [JsonPropertyName("copyWithSyntaxHighlighting")]
        public bool CopyWithSyntaxHighlighting { get; set; } = true;

        [JsonPropertyName("parameterHints")]
        public ParameterHintOptions ParameterHints { get; set; } = new ParameterHintOptions();

        [JsonPropertyName("quickSuggestions")]
        public QuickSuggestionOptions QuickSuggestions { get; set; } = new QuickSuggestionOptions();

        [JsonPropertyName("quickSuggestionsDelay")]
        public int QuickSuggestionsDelay { get; set; } = 10;

        [JsonPropertyName("suggestSelection")]
        public string SuggestSelection { get; set; } = "first";

        [JsonPropertyName("suggestFontSize")]
        public int SuggestFontSize { get; set; } = 0;

        [JsonPropertyName("suggestLineHeight")]
        public int SuggestLineHeight { get; set; } = 0;

        [JsonPropertyName("tabCompletion")]
        public string TabCompletion { get; set; } = "off";

        [JsonPropertyName("suggest")]
        public SuggestOptions Suggest { get; set; } = new SuggestOptions();

        [JsonPropertyName("gotoLocation")]
        public GotoLocationOptions GotoLocation { get; set; } = new GotoLocationOptions();

        [JsonPropertyName("inlayHints")]
        public InlayHintsOptions InlayHints { get; set; } = new InlayHintsOptions();

        [JsonPropertyName("selectionHighlight")]
        public bool SelectionHighlight { get; set; } = true;

        [JsonPropertyName("occurrencesHighlight")]
        public bool OccurrencesHighlight { get; set; } = true;

        [JsonPropertyName("codeLens")]
        public bool CodeLens { get; set; } = true;

        [JsonPropertyName("folding")]
        public bool Folding { get; set; } = true;

        [JsonPropertyName("foldingStrategy")]
        public string FoldingStrategy { get; set; } = "auto";

        [JsonPropertyName("foldingHighlight")]
        public bool FoldingHighlight { get; set; } = true;

        [JsonPropertyName("showFoldingControls")]
        public string ShowFoldingControls { get; set; } = "mouseover";

        [JsonPropertyName("unfoldOnClickAfterEndOfLine")]
        public bool UnfoldOnClickAfterEndOfLine { get; set; } = false;

        [JsonPropertyName("matchBrackets")]
        public string MatchBrackets { get; set; } = "always";

        [JsonPropertyName("renderWhitespace")]
        public string RenderWhitespace { get; set; } = "selection";

        [JsonPropertyName("renderControlCharacters")]
        public bool RenderControlCharacters { get; set; } = false;

        [JsonPropertyName("renderIndentGuides")]
        public bool RenderIndentGuides { get; set; } = true;

        [JsonPropertyName("renderLineHighlight")]
        public string RenderLineHighlight { get; set; } = "all";

        [JsonPropertyName("useTabStops")]
        public bool UseTabStops { get; set; } = true;

        [JsonPropertyName("minimap")]
        public MinimapOptions Minimap { get; set; } = new MinimapOptions();

        [JsonPropertyName("scrollbar")]
        public ScrollbarOptions Scrollbar { get; set; } = new ScrollbarOptions();

        [JsonPropertyName("find")]
        public FindOptions Find { get; set; } = new FindOptions();

        [JsonPropertyName("colorDecorators")]
        public bool ColorDecorators { get; set; } = true;

        [JsonPropertyName("lightbulb")]
        public LightbulbOptions Lightbulb { get; set; } = new LightbulbOptions();

        [JsonPropertyName("stickyTabStops")]
        public bool StickyTabStops { get; set; } = false;

        [JsonPropertyName("hover")]
        public HoverOptions Hover { get; set; } = new HoverOptions();

        [JsonPropertyName("links")]
        public bool Links { get; set; } = true;

        [JsonPropertyName("contextmenu")]
        public bool Contextmenu { get; set; } = true;

        [JsonPropertyName("cursorStyle")]
        public string CursorStyle { get; set; } = "line";

        [JsonPropertyName("cursorWidth")]
        public int CursorWidth { get; set; } = 0;

        [JsonPropertyName("cursorBlinking")]
        public string CursorBlinking { get; set; } = "blink";

        [JsonPropertyName("cursorSmoothCaretAnimation")]
        public bool CursorSmoothCaretAnimation { get; set; } = false;

        [JsonPropertyName("cursorSurroundingLines")]
        public int CursorSurroundingLines { get; set; } = 0;

        [JsonPropertyName("cursorSurroundingLinesStyle")]
        public string CursorSurroundingLinesStyle { get; set; } = "default";

        [JsonPropertyName("glyphMargin")]
        public bool GlyphMargin { get; set; } = true;

        [JsonPropertyName("lineDecorationsWidth")]
        public int LineDecorationsWidth { get; set; } = 10;

        [JsonPropertyName("lineNumbersMinChars")]
        public int LineNumbersMinChars { get; set; } = 5;

        [JsonPropertyName("overviewRulerBorder")]
        public bool OverviewRulerBorder { get; set; } = true;

        [JsonPropertyName("overviewRulerLanes")]
        public int OverviewRulerLanes { get; set; } = 3;

        [JsonPropertyName("padding")]
        public PaddingOptions Padding { get; set; } = new PaddingOptions();

        [JsonPropertyName("bracketPairColorization")]
        public BracketPairColorizationOptions BracketPairColorization { get; set; } = new BracketPairColorizationOptions();
    }

    public class QuickSuggestionOptions
    {
        [JsonPropertyName("other")]
        public bool Other { get; set; } = true;

        [JsonPropertyName("comments")]
        public bool Comments { get; set; } = false;

        [JsonPropertyName("strings")]
        public bool Strings { get; set; } = false;
    }

    public class ParameterHintOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("cycle")]
        public bool Cycle { get; set; } = false;
    }

    public class SuggestOptions
    {
        [JsonPropertyName("insertMode")]
        public string InsertMode { get; set; } = "insert";

        [JsonPropertyName("filterGraceful")]
        public bool FilterGraceful { get; set; } = true;

        [JsonPropertyName("snippetsPreventQuickSuggestions")]
        public bool SnippetsPreventQuickSuggestions { get; set; } = true;

        [JsonPropertyName("localityBonus")]
        public bool LocalityBonus { get; set; } = false;

        [JsonPropertyName("shareSuggestSelections")]
        public bool ShareSuggestSelections { get; set; } = false;

        [JsonPropertyName("showIcons")]
        public bool ShowIcons { get; set; } = true;

        [JsonPropertyName("showStatusBar")]
        public bool ShowStatusBar { get; set; } = false;

        [JsonPropertyName("preview")]
        public bool Preview { get; set; } = false;

        [JsonPropertyName("previewMode")]
        public string PreviewMode { get; set; } = "subwordSmart";

        [JsonPropertyName("showInlineDetails")]
        public bool ShowInlineDetails { get; set; } = true;

        [JsonPropertyName("showMethods")]
        public bool ShowMethods { get; set; } = true;

        [JsonPropertyName("showFunctions")]
        public bool ShowFunctions { get; set; } = true;

        [JsonPropertyName("showConstructors")]
        public bool ShowConstructors { get; set; } = true;

        [JsonPropertyName("showFields")]
        public bool ShowFields { get; set; } = true;

        [JsonPropertyName("showVariables")]
        public bool ShowVariables { get; set; } = true;

        [JsonPropertyName("showClasses")]
        public bool ShowClasses { get; set; } = true;

        [JsonPropertyName("showStructs")]
        public bool ShowStructs { get; set; } = true;

        [JsonPropertyName("showInterfaces")]
        public bool ShowInterfaces { get; set; } = true;

        [JsonPropertyName("showModules")]
        public bool ShowModules { get; set; } = true;

        [JsonPropertyName("showProperties")]
        public bool ShowProperties { get; set; } = true;

        [JsonPropertyName("showEvents")]
        public bool ShowEvents { get; set; } = true;

        [JsonPropertyName("showOperators")]
        public bool ShowOperators { get; set; } = true;

        [JsonPropertyName("showUnits")]
        public bool ShowUnits { get; set; } = true;

        [JsonPropertyName("showValues")]
        public bool ShowValues { get; set; } = true;

        [JsonPropertyName("showConstants")]
        public bool ShowConstants { get; set; } = true;

        [JsonPropertyName("showEnums")]
        public bool ShowEnums { get; set; } = true;

        [JsonPropertyName("showEnumMembers")]
        public bool ShowEnumMembers { get; set; } = true;

        [JsonPropertyName("showKeywords")]
        public bool ShowKeywords { get; set; } = true;

        [JsonPropertyName("showWords")]
        public bool ShowWords { get; set; } = true;

        [JsonPropertyName("showColors")]
        public bool ShowColors { get; set; } = true;

        [JsonPropertyName("showFiles")]
        public bool ShowFiles { get; set; } = true;

        [JsonPropertyName("showReferences")]
        public bool ShowReferences { get; set; } = true;

        [JsonPropertyName("showFolders")]
        public bool ShowFolders { get; set; } = true;

        [JsonPropertyName("showTypeParameters")]
        public bool ShowTypeParameters { get; set; } = true;

        [JsonPropertyName("showSnippets")]
        public bool ShowSnippets { get; set; } = true;

        [JsonPropertyName("showUsers")]
        public bool ShowUsers { get; set; } = true;

        [JsonPropertyName("showIssues")]
        public bool ShowIssues { get; set; } = true;
    }

    public class GotoLocationOptions
    {
        [JsonPropertyName("multiple")]
        public string Multiple { get; set; } = "peek";

        [JsonPropertyName("multipleDefinitions")]
        public string MultipleDefinitions { get; set; } = "peek";

        [JsonPropertyName("multipleTypeDefinitions")]
        public string MultipleTypeDefinitions { get; set; } = "peek";

        [JsonPropertyName("multipleDeclarations")]
        public string MultipleDeclarations { get; set; } = "peek";

        [JsonPropertyName("multipleImplementations")]
        public string MultipleImplementations { get; set; } = "peek";

        [JsonPropertyName("multipleReferences")]
        public string MultipleReferences { get; set; } = "peek";

        [JsonPropertyName("alternativeDefinitionCommand")]
        public string AlternativeDefinitionCommand { get; set; }

        [JsonPropertyName("alternativeTypeDefinitionCommand")]
        public string AlternativeTypeDefinitionCommand { get; set; }

        [JsonPropertyName("alternativeDeclarationCommand")]
        public string AlternativeDeclarationCommand { get; set; }

        [JsonPropertyName("alternativeImplementationCommand")]
        public string AlternativeImplementationCommand { get; set; }

        [JsonPropertyName("alternativeReferenceCommand")]
        public string AlternativeReferenceCommand { get; set; }
    }

    public class InlayHintsOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = false;

        [JsonPropertyName("fontSize")]
        public int FontSize { get; set; } = 0;

        [JsonPropertyName("fontFamily")]
        public string FontFamily { get; set; }

        [JsonPropertyName("padding")]
        public bool Padding { get; set; } = false;
    }

    public class MinimapOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("side")]
        public string Side { get; set; } = "right";

        [JsonPropertyName("size")]
        public string Size { get; set; } = "proportional";

        [JsonPropertyName("showSlider")]
        public string ShowSlider { get; set; } = "mouseover";

        [JsonPropertyName("renderCharacters")]
        public bool RenderCharacters { get; set; } = true;

        [JsonPropertyName("maxColumn")]
        public int MaxColumn { get; set; } = 120;

        [JsonPropertyName("scale")]
        public int Scale { get; set; } = 1;
    }

    public class ScrollbarOptions
    {
        [JsonPropertyName("arrowSize")]
        public int ArrowSize { get; set; } = 11;

        [JsonPropertyName("vertical")]
        public string Vertical { get; set; } = "auto";

        [JsonPropertyName("horizontal")]
        public string Horizontal { get; set; } = "auto";

        [JsonPropertyName("useShadows")]
        public bool UseShadows { get; set; } = true;

        [JsonPropertyName("verticalHasArrows")]
        public bool VerticalHasArrows { get; set; } = false;

        [JsonPropertyName("horizontalHasArrows")]
        public bool HorizontalHasArrows { get; set; } = false;

        [JsonPropertyName("handleMouseWheel")]
        public bool HandleMouseWheel { get; set; } = true;

        [JsonPropertyName("alwaysConsumeMouseWheel")]
        public bool AlwaysConsumeMouseWheel { get; set; } = true;

        [JsonPropertyName("verticalScrollbarSize")]
        public int VerticalScrollbarSize { get; set; } = 10;

        [JsonPropertyName("horizontalScrollbarSize")]
        public int HorizontalScrollbarSize { get; set; } = 10;

        [JsonPropertyName("verticalSliderSize")]
        public int VerticalSliderSize { get; set; } = 0;

        [JsonPropertyName("horizontalSliderSize")]
        public int HorizontalSliderSize { get; set; } = 0;

        [JsonPropertyName("scrollByPage")]
        public bool ScrollByPage { get; set; } = false;
    }

    public class FindOptions
    {
        [JsonPropertyName("cursorMoveOnType")]
        public bool CursorMoveOnType { get; set; } = true;

        [JsonPropertyName("seedSearchStringFromSelection")]
        public string SeedSearchStringFromSelection { get; set; } = "always";

        [JsonPropertyName("autoFindInSelection")]
        public string AutoFindInSelection { get; set; } = "never";

        [JsonPropertyName("addExtraSpaceOnTop")]
        public bool AddExtraSpaceOnTop { get; set; } = true;

        [JsonPropertyName("loop")]
        public bool Loop { get; set; } = true;
    }

    public class LightbulbOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;
    }

    public class HoverOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("delay")]
        public int Delay { get; set; } = 300;

        [JsonPropertyName("sticky")]
        public bool Sticky { get; set; } = true;
    }

    public class PaddingOptions
    {
        [JsonPropertyName("top")]
        public int Top { get; set; } = 0;

        [JsonPropertyName("bottom")]
        public int Bottom { get; set; } = 0;
    }

    public class BracketPairColorizationOptions
    {
        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; } = true;

        [JsonPropertyName("independentColorPoolPerBracketType")]
        public bool IndependentColorPoolPerBracketType { get; set; } = false;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using MauiMonaco.Models;

namespace MauiMonaco
{
    /// <summary>
    /// Comprehensive Monaco Editor wrapper with full API support
    /// </summary>
    public class MonacoEditor : MonacoEditorView
    {
        #region Events

        // Content events
        public event EventHandler<ContentChangedEventArgs> OnDidChangeModelContent;
        public event EventHandler<ModelLanguageChangedEventArgs> OnDidChangeModelLanguage;
        public event EventHandler<ModelOptionsChangedEventArgs> OnDidChangeModelOptions;
        public event EventHandler<ModelDecorationsChangedEventArgs> OnDidChangeModelDecorations;

        // Cursor events
        public event EventHandler<CursorPositionChangedEventArgs> OnDidChangeCursorPosition;
        public event EventHandler<CursorSelectionChangedEventArgs> OnDidChangeCursorSelection;

        // Focus events
        public event EventHandler OnDidFocusEditorText;
        public event EventHandler OnDidBlurEditorText;
        public event EventHandler OnDidFocusEditorWidget;
        public event EventHandler OnDidBlurEditorWidget;

        // Layout events
        public event EventHandler<LayoutChangedEventArgs> OnDidLayoutChange;
        public event EventHandler<ContentSizeChangedEventArgs> OnDidContentSizeChange;
        public event EventHandler<ScrollChangedEventArgs> OnDidScrollChange;
        public event EventHandler<VisibleRangesChangedEventArgs> OnDidChangeViewZones;

        // Mouse events
        public event EventHandler<MouseEventArgs> OnMouseDown;
        public event EventHandler<MouseEventArgs> OnMouseUp;
        public event EventHandler<MouseEventArgs> OnMouseMove;
        public event EventHandler<MouseEventArgs> OnMouseLeave;
        public event EventHandler<ContextMenuEventArgs> OnContextMenu;

        // Keyboard events
        public event EventHandler<KeyboardEventArgs> OnKeyDown;
        public event EventHandler<KeyboardEventArgs> OnKeyUp;

        // Composition events
        public event EventHandler<CompositionEventArgs> OnDidCompositionStart;
        public event EventHandler<CompositionEventArgs> OnDidCompositionEnd;

        // Other events
        public event EventHandler<PasteEventArgs> OnDidPaste;
        public event EventHandler<ConfigurationChangedEventArgs> OnDidChangeConfiguration;
        public event EventHandler<ModelChangedEventArgs> OnDidChangeModel;

        #endregion

        #region Properties

        private MonacoEditorOptions _options;
        private List<string> _decorationIds = new List<string>();
        private Dictionary<string, object> _commands = new Dictionary<string, object>();
        private Dictionary<string, object> _actions = new Dictionary<string, object>();

        public MonacoEditorOptions Options
        {
            get => _options;
            set
            {
                _options = value;
                if (_isInitialized)
                {
                    _ = UpdateOptions(value);
                }
            }
        }

        #endregion

        #region Constructor

        public MonacoEditor() : base()
        {
            _options = new MonacoEditorOptions();
            InitializeEventHandlers();
        }

        public MonacoEditor(MonacoEditorOptions options) : base()
        {
            _options = options ?? new MonacoEditorOptions();
            InitializeEventHandlers();
        }

        #endregion

        #region Initialization

        private void InitializeEventHandlers()
        {
            // Override base CodeChanged event to provide typed event args
            base.CodeChanged += (sender, code) =>
            {
                OnDidChangeModelContent?.Invoke(this, new ContentChangedEventArgs
                {
                    Changes = new List<ContentChange>
                    {
                        new ContentChange { Text = code }
                    }
                });
            };
        }

        protected override async Task InitializeEditor()
        {
            await base.InitializeEditor();

            if (_options != null)
            {
                await UpdateOptions(_options);
            }

            await RegisterEventListeners();
        }

        private async Task RegisterEventListeners()
        {
            // Register all event listeners with the JavaScript side
            var script = @"
                if (window.editor) {
                    // Content events
                    window.editor.onDidChangeModelContent((e) => {
                        window.invokeMonacoEvent('OnDidChangeModelContent', e);
                    });

                    window.editor.onDidChangeModelLanguage((e) => {
                        window.invokeMonacoEvent('OnDidChangeModelLanguage', e);
                    });

                    window.editor.onDidChangeModelOptions((e) => {
                        window.invokeMonacoEvent('OnDidChangeModelOptions', e);
                    });

                    window.editor.onDidChangeModelDecorations((e) => {
                        window.invokeMonacoEvent('OnDidChangeModelDecorations', e);
                    });

                    // Cursor events
                    window.editor.onDidChangeCursorPosition((e) => {
                        window.invokeMonacoEvent('OnDidChangeCursorPosition', e);
                    });

                    window.editor.onDidChangeCursorSelection((e) => {
                        window.invokeMonacoEvent('OnDidChangeCursorSelection', e);
                    });

                    // Focus events
                    window.editor.onDidFocusEditorText(() => {
                        window.invokeMonacoEvent('OnDidFocusEditorText', {});
                    });

                    window.editor.onDidBlurEditorText(() => {
                        window.invokeMonacoEvent('OnDidBlurEditorText', {});
                    });

                    // Layout events
                    window.editor.onDidLayoutChange((e) => {
                        window.invokeMonacoEvent('OnDidLayoutChange', e);
                    });

                    window.editor.onDidContentSizeChange((e) => {
                        window.invokeMonacoEvent('OnDidContentSizeChange', e);
                    });

                    window.editor.onDidScrollChange((e) => {
                        window.invokeMonacoEvent('OnDidScrollChange', e);
                    });

                    // Mouse events
                    window.editor.onMouseDown((e) => {
                        window.invokeMonacoEvent('OnMouseDown', e);
                    });

                    window.editor.onMouseUp((e) => {
                        window.invokeMonacoEvent('OnMouseUp', e);
                    });

                    window.editor.onMouseMove((e) => {
                        window.invokeMonacoEvent('OnMouseMove', e);
                    });

                    window.editor.onContextMenu((e) => {
                        window.invokeMonacoEvent('OnContextMenu', e);
                    });

                    // Keyboard events
                    window.editor.onKeyDown((e) => {
                        window.invokeMonacoEvent('OnKeyDown', e);
                    });

                    window.editor.onKeyUp((e) => {
                        window.invokeMonacoEvent('OnKeyUp', e);
                    });

                    // Other events
                    window.editor.onDidPaste((e) => {
                        window.invokeMonacoEvent('OnDidPaste', e);
                    });

                    window.editor.onDidChangeConfiguration((e) => {
                        window.invokeMonacoEvent('OnDidChangeConfiguration', e);
                    });
                }

                // Define event invocation helper
                window.invokeMonacoEvent = function(eventName, eventData) {
                    // This would be connected to C# event handlers
                    console.log('Event:', eventName, eventData);
                };
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Core Editor Methods

        /// <summary>
        /// Update editor options dynamically
        /// </summary>
        public async Task UpdateOptions(MonacoEditorOptions options)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.updateOptions({json})");
        }

        /// <summary>
        /// Get the current value of the editor
        /// </summary>
        public async Task<string> GetValue()
        {
            return await GetEditorValue();
        }

        /// <summary>
        /// Set the value of the editor
        /// </summary>
        public async Task SetValue(string value)
        {
            await SetValueSafely(value);
        }

        /// <summary>
        /// Get the current model of the editor
        /// </summary>
        public async Task<object> GetModel()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? window.editor.getModel() : null");
            return result;
        }

        /// <summary>
        /// Set a new model for the editor
        /// </summary>
        public async Task SetModel(string value, string language = null, string uri = null)
        {
            if (!_isInitialized) return;

            var script = $@"
                if (window.editor && monaco) {{
                    var model = monaco.editor.createModel(
                        {JsonSerializer.Serialize(value)},
                        {JsonSerializer.Serialize(language ?? "javascript")},
                        {(uri != null ? $"monaco.Uri.parse({JsonSerializer.Serialize(uri)})" : "null")}
                    );
                    window.editor.setModel(model);
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Position & Selection Methods

        /// <summary>
        /// Get the current cursor position
        /// </summary>
        public async Task<Position> GetPosition()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? JSON.stringify(window.editor.getPosition()) : null");
            if (result != null)
            {
                return JsonSerializer.Deserialize<Position>(result.ToString());
            }
            return null;
        }

        /// <summary>
        /// Set the cursor position
        /// </summary>
        public async Task SetPosition(Position position)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(position);
            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.setPosition({json})");
        }

        /// <summary>
        /// Get the current selection
        /// </summary>
        public async Task<Selection> GetSelection()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? JSON.stringify(window.editor.getSelection()) : null");
            if (result != null)
            {
                return JsonSerializer.Deserialize<Selection>(result.ToString());
            }
            return null;
        }

        /// <summary>
        /// Set the selection
        /// </summary>
        public async Task SetSelection(Selection selection)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(selection);
            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.setSelection({json})");
        }

        /// <summary>
        /// Get all selections
        /// </summary>
        public async Task<List<Selection>> GetSelections()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? JSON.stringify(window.editor.getSelections()) : null");
            if (result != null)
            {
                return JsonSerializer.Deserialize<List<Selection>>(result.ToString());
            }
            return null;
        }

        /// <summary>
        /// Set multiple selections
        /// </summary>
        public async Task SetSelections(List<Selection> selections)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(selections);
            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.setSelections({json})");
        }

        #endregion

        #region Text Manipulation Methods

        /// <summary>
        /// Execute edits on the model
        /// </summary>
        public async Task<bool> ExecuteEdits(string source, List<IdentifiedSingleEditOperation> edits)
        {
            if (!_isInitialized) return false;

            var json = JsonSerializer.Serialize(new { source, edits });
            var result = await _webView.EvaluateJavaScriptAsync($"window.editor ? window.editor.executeEdits({json.source}, {json.edits}) : false");
            return result?.ToString() == "true";
        }

        /// <summary>
        /// Get the value in a range
        /// </summary>
        public async Task<string> GetValueInRange(Range range)
        {
            if (!_isInitialized) return null;

            var json = JsonSerializer.Serialize(range);
            var result = await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.getModel() ? window.editor.getModel().getValueInRange({json}) : null");
            return result?.ToString();
        }

        /// <summary>
        /// Get line content
        /// </summary>
        public async Task<string> GetLineContent(int lineNumber)
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.getModel() ? window.editor.getModel().getLineContent({lineNumber}) : null");
            return result?.ToString();
        }

        /// <summary>
        /// Get line count
        /// </summary>
        public async Task<int> GetLineCount()
        {
            if (!_isInitialized) return 0;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor && window.editor.getModel() ? window.editor.getModel().getLineCount() : 0");
            if (int.TryParse(result?.ToString(), out var count))
            {
                return count;
            }
            return 0;
        }

        #endregion

        #region Decoration Methods

        /// <summary>
        /// Apply decorations to the editor
        /// </summary>
        public async Task<List<string>> DeltaDecorations(List<string> oldDecorationIds, List<ModelDeltaDecoration> newDecorations)
        {
            if (!_isInitialized) return new List<string>();

            var json = JsonSerializer.Serialize(new { oldDecorationIds, newDecorations });
            var result = await _webView.EvaluateJavaScriptAsync($@"
                window.editor ? JSON.stringify(
                    window.editor.deltaDecorations(
                        {json.oldDecorationIds},
                        {json.newDecorations}
                    )
                ) : '[]'
            ");

            if (result != null)
            {
                var ids = JsonSerializer.Deserialize<List<string>>(result.ToString());
                _decorationIds = ids;
                return ids;
            }
            return new List<string>();
        }

        /// <summary>
        /// Remove all decorations
        /// </summary>
        public async Task RemoveAllDecorations()
        {
            if (_decorationIds.Any())
            {
                await DeltaDecorations(_decorationIds, new List<ModelDeltaDecoration>());
                _decorationIds.Clear();
            }
        }

        /// <summary>
        /// Get decorations in a range
        /// </summary>
        public async Task<List<ModelDecoration>> GetDecorationsInRange(Range range)
        {
            if (!_isInitialized) return null;

            var json = JsonSerializer.Serialize(range);
            var result = await _webView.EvaluateJavaScriptAsync($"window.editor ? JSON.stringify(window.editor.getModel().getDecorationsInRange({json})) : null");

            if (result != null)
            {
                return JsonSerializer.Deserialize<List<ModelDecoration>>(result.ToString());
            }
            return null;
        }

        #endregion

        #region Marker Methods

        /// <summary>
        /// Set model markers
        /// </summary>
        public async Task SetModelMarkers(string owner, List<MarkerData> markers)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(markers);
            var script = $@"
                if (window.editor && monaco) {{
                    var model = window.editor.getModel();
                    if (model) {{
                        monaco.editor.setModelMarkers(model, {JsonSerializer.Serialize(owner)}, {json});
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Get model markers
        /// </summary>
        public async Task<List<MarkerData>> GetModelMarkers(string owner = null)
        {
            if (!_isInitialized) return null;

            var script = owner != null
                ? $"monaco.editor.getModelMarkers({{owner: {JsonSerializer.Serialize(owner)}}})"
                : "monaco.editor.getModelMarkers({})";

            var result = await _webView.EvaluateJavaScriptAsync($"window.editor && monaco ? JSON.stringify({script}) : null");

            if (result != null)
            {
                return JsonSerializer.Deserialize<List<MarkerData>>(result.ToString());
            }
            return null;
        }

        /// <summary>
        /// Remove all markers
        /// </summary>
        public async Task RemoveAllMarkers(string owner)
        {
            await SetModelMarkers(owner, new List<MarkerData>());
        }

        #endregion

        #region Action & Command Methods

        /// <summary>
        /// Add an action to the editor
        /// </summary>
        public async Task AddAction(string id, string label, List<int> keybindings, string precondition, Action<MonacoEditor> run)
        {
            if (!_isInitialized) return;

            _actions[id] = run;

            var keybindingsJson = JsonSerializer.Serialize(keybindings ?? new List<int>());
            var script = $@"
                window.editor && window.editor.addAction({{
                    id: {JsonSerializer.Serialize(id)},
                    label: {JsonSerializer.Serialize(label)},
                    keybindings: {keybindingsJson},
                    precondition: {JsonSerializer.Serialize(precondition)},
                    run: function(editor) {{
                        window.invokeMonacoAction({JsonSerializer.Serialize(id)});
                    }}
                }});
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Add a command to the editor
        /// </summary>
        public async Task<string> AddCommand(int keybinding, Action<MonacoEditor> handler, string context = null)
        {
            if (!_isInitialized) return null;

            var commandId = Guid.NewGuid().ToString();
            _commands[commandId] = handler;

            var script = $@"
                window.editor ? window.editor.addCommand(
                    {keybinding},
                    function() {{
                        window.invokeMonacoCommand({JsonSerializer.Serialize(commandId)});
                    }},
                    {JsonSerializer.Serialize(context)}
                ) : null
            ";

            var result = await _webView.EvaluateJavaScriptAsync(script);
            return result?.ToString();
        }

        /// <summary>
        /// Execute a command
        /// </summary>
        public async Task<object> ExecuteCommand(string command, params object[] args)
        {
            if (!_isInitialized) return null;

            var argsJson = args != null && args.Length > 0
                ? string.Join(", ", args.Select(a => JsonSerializer.Serialize(a)))
                : "";

            var script = $"window.editor ? window.editor.trigger('', {JsonSerializer.Serialize(command)}, {{{argsJson}}}) : null";
            var result = await _webView.EvaluateJavaScriptAsync(script);
            return result;
        }

        /// <summary>
        /// Get supported actions
        /// </summary>
        public async Task<List<object>> GetSupportedActions()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? JSON.stringify(window.editor.getSupportedActions()) : null");
            if (result != null)
            {
                return JsonSerializer.Deserialize<List<object>>(result.ToString());
            }
            return null;
        }

        #endregion

        #region Navigation Methods

        /// <summary>
        /// Reveal line in the editor
        /// </summary>
        public async Task RevealLine(int lineNumber, string scrollType = "smooth")
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.revealLine({lineNumber}, '{scrollType}')");
        }

        /// <summary>
        /// Reveal line in center
        /// </summary>
        public async Task RevealLineInCenter(int lineNumber, string scrollType = "smooth")
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.revealLineInCenter({lineNumber}, '{scrollType}')");
        }

        /// <summary>
        /// Reveal line in center if outside viewport
        /// </summary>
        public async Task RevealLineInCenterIfOutsideViewport(int lineNumber, string scrollType = "smooth")
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.revealLineInCenterIfOutsideViewport({lineNumber}, '{scrollType}')");
        }

        /// <summary>
        /// Reveal position
        /// </summary>
        public async Task RevealPosition(Position position, string scrollType = "smooth")
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(position);
            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.revealPosition({json}, '{scrollType}')");
        }

        /// <summary>
        /// Reveal range
        /// </summary>
        public async Task RevealRange(Range range, string scrollType = "smooth")
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(range);
            await _webView.EvaluateJavaScriptAsync($"window.editor && window.editor.revealRange({json}, '{scrollType}')");
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Focus the editor
        /// </summary>
        public async Task Focus()
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync("window.editor && window.editor.focus()");
        }

        /// <summary>
        /// Check if the editor has focus
        /// </summary>
        public async Task<bool> HasFocus()
        {
            if (!_isInitialized) return false;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? window.editor.hasWidgetFocus() : false");
            return result?.ToString() == "true";
        }

        /// <summary>
        /// Trigger editor layout
        /// </summary>
        public async Task Layout()
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync("window.editor && window.editor.layout()");
        }

        /// <summary>
        /// Format the document
        /// </summary>
        public async Task FormatDocument()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("editor.action.formatDocument");
        }

        /// <summary>
        /// Format selection
        /// </summary>
        public async Task FormatSelection()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("editor.action.formatSelection");
        }

        /// <summary>
        /// Undo last operation
        /// </summary>
        public async Task Undo()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("undo");
        }

        /// <summary>
        /// Redo last operation
        /// </summary>
        public async Task Redo()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("redo");
        }

        /// <summary>
        /// Find text
        /// </summary>
        public async Task Find()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("actions.find");
        }

        /// <summary>
        /// Replace text
        /// </summary>
        public async Task Replace()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("editor.action.startFindReplaceAction");
        }

        /// <summary>
        /// Go to line
        /// </summary>
        public async Task GotoLine()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("editor.action.gotoLine");
        }

        /// <summary>
        /// Toggle word wrap
        /// </summary>
        public async Task ToggleWordWrap()
        {
            if (!_isInitialized) return;

            await ExecuteCommand("editor.action.toggleWordWrap");
        }

        /// <summary>
        /// Get editor ID
        /// </summary>
        public async Task<string> GetEditorId()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? window.editor.getId() : null");
            return result?.ToString();
        }

        /// <summary>
        /// Get editor type
        /// </summary>
        public async Task<string> GetEditorType()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync("window.editor ? window.editor.getEditorType() : null");
            return result?.ToString();
        }

        /// <summary>
        /// Dispose the editor
        /// </summary>
        public async Task Dispose()
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync("window.editor && window.editor.dispose()");
            _decorationIds.Clear();
            _commands.Clear();
            _actions.Clear();
        }

        #endregion
    }
}
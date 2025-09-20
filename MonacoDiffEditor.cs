using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using MauiMonaco.Models;

namespace MauiMonaco
{
    /// <summary>
    /// Monaco Diff Editor for comparing two files or text content
    /// </summary>
    public class MonacoDiffEditor : Grid
    {
        private WebView _webView;
        private bool _isInitialized = false;
        private ActivityIndicator _loadingIndicator;
        private Label _loadingLabel;
        private Grid _loadingOverlay;

        #region Properties

        public static readonly BindableProperty OriginalProperty = BindableProperty.Create(
            nameof(Original),
            typeof(string),
            typeof(MonacoDiffEditor),
            string.Empty,
            propertyChanged: OnOriginalPropertyChanged);

        public static readonly BindableProperty ModifiedProperty = BindableProperty.Create(
            nameof(Modified),
            typeof(string),
            typeof(MonacoDiffEditor),
            string.Empty,
            propertyChanged: OnModifiedPropertyChanged);

        public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(MonacoDiffEditor),
            "javascript",
            propertyChanged: OnLanguagePropertyChanged);

        public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
            nameof(Theme),
            typeof(string),
            typeof(MonacoDiffEditor),
            "vs-dark",
            propertyChanged: OnThemePropertyChanged);

        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(
            nameof(ReadOnly),
            typeof(bool),
            typeof(MonacoDiffEditor),
            false,
            propertyChanged: OnReadOnlyPropertyChanged);

        public static readonly BindableProperty RenderSideBySideProperty = BindableProperty.Create(
            nameof(RenderSideBySide),
            typeof(bool),
            typeof(MonacoDiffEditor),
            true,
            propertyChanged: OnRenderSideBySidePropertyChanged);

        public static readonly BindableProperty IgnoreTrimWhitespaceProperty = BindableProperty.Create(
            nameof(IgnoreTrimWhitespace),
            typeof(bool),
            typeof(MonacoDiffEditor),
            true,
            propertyChanged: OnIgnoreTrimWhitespacePropertyChanged);

        public static readonly BindableProperty EnableSplitViewResizingProperty = BindableProperty.Create(
            nameof(EnableSplitViewResizing),
            typeof(bool),
            typeof(MonacoDiffEditor),
            true);

        public string Original
        {
            get => (string)GetValue(OriginalProperty);
            set => SetValue(OriginalProperty, value);
        }

        public string Modified
        {
            get => (string)GetValue(ModifiedProperty);
            set => SetValue(ModifiedProperty, value);
        }

        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public bool ReadOnly
        {
            get => (bool)GetValue(ReadOnlyProperty);
            set => SetValue(ReadOnlyProperty, value);
        }

        public bool RenderSideBySide
        {
            get => (bool)GetValue(RenderSideBySideProperty);
            set => SetValue(RenderSideBySideProperty, value);
        }

        public bool IgnoreTrimWhitespace
        {
            get => (bool)GetValue(IgnoreTrimWhitespaceProperty);
            set => SetValue(IgnoreTrimWhitespaceProperty, value);
        }

        public bool EnableSplitViewResizing
        {
            get => (bool)GetValue(EnableSplitViewResizingProperty);
            set => SetValue(EnableSplitViewResizingProperty, value);
        }

        #endregion

        #region Events

        public event EventHandler<DiffEditorChangedEventArgs> OnDidUpdateDiff;
        public event EventHandler<string> OriginalEditorChanged;
        public event EventHandler<string> ModifiedEditorChanged;

        #endregion

        #region Constructor

        public MonacoDiffEditor()
        {
            BuildUI();
            InitializeWebView();
        }

        #endregion

        #region UI Building

        private void BuildUI()
        {
            _webView = new WebView();

            _loadingOverlay = new Grid
            {
                BackgroundColor = Colors.Black.WithAlpha(0.8f),
                IsVisible = true
            };

            var loadingContent = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 10
            };

            _loadingIndicator = new ActivityIndicator
            {
                Color = Colors.White,
                IsRunning = true,
                WidthRequest = 50,
                HeightRequest = 50
            };

            _loadingLabel = new Label
            {
                Text = "Loading Monaco Diff Editor...",
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 14
            };

            loadingContent.Children.Add(_loadingIndicator);
            loadingContent.Children.Add(_loadingLabel);
            _loadingOverlay.Children.Add(loadingContent);

            this.Children.Add(_webView);
            this.Children.Add(_loadingOverlay);
        }

        private void InitializeWebView()
        {
            var html = GetMonacoDiffHtml();
            _webView.Source = new HtmlWebViewSource { Html = html };
            _webView.Navigated += OnNavigated;
        }

        #endregion

        #region Initialization

        private async void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success && !_isInitialized)
            {
                _isInitialized = true;
                await Task.Delay(1000);

                var ready = await CheckEditorReady();
                if (ready)
                {
                    await InitializeEditor();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _loadingOverlay.IsVisible = false;
                        _loadingIndicator.IsRunning = false;
                    });
                }
            }
        }

        private async Task<bool> CheckEditorReady()
        {
            for (int i = 0; i < 15; i++)
            {
                var result = await _webView.EvaluateJavaScriptAsync("window.diffEditorReady === true");
                if (result?.ToString()?.ToLower() == "true")
                {
                    return true;
                }
                await Task.Delay(200);
            }
            return false;
        }

        private async Task InitializeEditor()
        {
            await SetTheme(Theme);
            await SetLanguage(Language);
            await SetReadOnly(ReadOnly);
            await SetRenderSideBySide(RenderSideBySide);
            await SetIgnoreTrimWhitespace(IgnoreTrimWhitespace);

            if (!string.IsNullOrEmpty(Original) || !string.IsNullOrEmpty(Modified))
            {
                await SetModels(Original, Modified);
            }
        }

        #endregion

        #region Property Changed Handlers

        private static async void OnOriginalPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized)
            {
                await editor.SetOriginalModel(newValue as string ?? "");
            }
        }

        private static async void OnModifiedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized)
            {
                await editor.SetModifiedModel(newValue as string ?? "");
            }
        }

        private static async void OnLanguagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized && newValue != null)
            {
                await editor.SetLanguage(newValue.ToString());
            }
        }

        private static async void OnThemePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized && newValue != null)
            {
                await editor.SetTheme(newValue.ToString());
            }
        }

        private static async void OnReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized)
            {
                await editor.SetReadOnly((bool)newValue);
            }
        }

        private static async void OnRenderSideBySidePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized)
            {
                await editor.SetRenderSideBySide((bool)newValue);
            }
        }

        private static async void OnIgnoreTrimWhitespacePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoDiffEditor editor && editor._isInitialized)
            {
                await editor.SetIgnoreTrimWhitespace((bool)newValue);
            }
        }

        #endregion

        #region Public Methods

        public async Task SetModels(string original, string modified)
        {
            if (!_isInitialized) return;

            var script = $@"
                if (window.diffEditor && monaco) {{
                    var originalModel = monaco.editor.createModel(
                        {JsonSerializer.Serialize(original ?? "")},
                        {JsonSerializer.Serialize(Language)}
                    );
                    var modifiedModel = monaco.editor.createModel(
                        {JsonSerializer.Serialize(modified ?? "")},
                        {JsonSerializer.Serialize(Language)}
                    );
                    window.diffEditor.setModel({{
                        original: originalModel,
                        modified: modifiedModel
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        public async Task SetOriginalModel(string content)
        {
            if (!_isInitialized) return;

            var script = $@"
                if (window.diffEditor && monaco) {{
                    var model = window.diffEditor.getModel();
                    if (model && model.original) {{
                        model.original.setValue({JsonSerializer.Serialize(content ?? "")});
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        public async Task SetModifiedModel(string content)
        {
            if (!_isInitialized) return;

            var script = $@"
                if (window.diffEditor && monaco) {{
                    var model = window.diffEditor.getModel();
                    if (model && model.modified) {{
                        model.modified.setValue({JsonSerializer.Serialize(content ?? "")});
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        public async Task<string> GetOriginalValue()
        {
            if (!_isInitialized) return "";

            var result = await _webView.EvaluateJavaScriptAsync(@"
                window.diffEditor && window.diffEditor.getModel()
                    ? window.diffEditor.getModel().original.getValue()
                    : ''
            ");
            return result?.ToString() ?? "";
        }

        public async Task<string> GetModifiedValue()
        {
            if (!_isInitialized) return "";

            var result = await _webView.EvaluateJavaScriptAsync(@"
                window.diffEditor && window.diffEditor.getModel()
                    ? window.diffEditor.getModel().modified.getValue()
                    : ''
            ");
            return result?.ToString() ?? "";
        }

        public async Task SetLanguage(string language)
        {
            if (!_isInitialized) return;

            var script = $@"
                if (window.diffEditor && monaco) {{
                    var model = window.diffEditor.getModel();
                    if (model) {{
                        if (model.original) {{
                            monaco.editor.setModelLanguage(model.original, {JsonSerializer.Serialize(language)});
                        }}
                        if (model.modified) {{
                            monaco.editor.setModelLanguage(model.modified, {JsonSerializer.Serialize(language)});
                        }}
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        public async Task SetTheme(string theme)
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($"monaco && monaco.editor.setTheme({JsonSerializer.Serialize(theme)})");
        }

        public async Task SetReadOnly(bool readOnly)
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($@"
                window.diffEditor && window.diffEditor.updateOptions({{
                    readOnly: {readOnly.ToString().ToLower()}
                }})
            ");
        }

        public async Task SetRenderSideBySide(bool sideBySide)
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($@"
                window.diffEditor && window.diffEditor.updateOptions({{
                    renderSideBySide: {sideBySide.ToString().ToLower()}
                }})
            ");
        }

        public async Task SetIgnoreTrimWhitespace(bool ignore)
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync($@"
                window.diffEditor && window.diffEditor.updateOptions({{
                    ignoreTrimWhitespace: {ignore.ToString().ToLower()}
                }})
            ");
        }

        public async Task UpdateOptions(DiffEditorOptions options)
        {
            if (!_isInitialized) return;

            var json = JsonSerializer.Serialize(options, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            await _webView.EvaluateJavaScriptAsync($"window.diffEditor && window.diffEditor.updateOptions({json})");
        }

        public async Task Layout()
        {
            if (!_isInitialized) return;

            await _webView.EvaluateJavaScriptAsync("window.diffEditor && window.diffEditor.layout()");
        }

        public async Task<DiffStatistics> GetDiffStatistics()
        {
            if (!_isInitialized) return null;

            var result = await _webView.EvaluateJavaScriptAsync(@"
                if (window.diffEditor && window.diffEditor.getLineChanges) {
                    var changes = window.diffEditor.getLineChanges();
                    if (changes) {
                        var stats = {
                            additions: 0,
                            deletions: 0,
                            modifications: 0
                        };
                        changes.forEach(function(change) {
                            if (change.originalStartLineNumber === 0) {
                                stats.additions += change.modifiedEndLineNumber - change.modifiedStartLineNumber + 1;
                            } else if (change.modifiedStartLineNumber === 0) {
                                stats.deletions += change.originalEndLineNumber - change.originalStartLineNumber + 1;
                            } else {
                                stats.modifications += Math.max(
                                    change.modifiedEndLineNumber - change.modifiedStartLineNumber,
                                    change.originalEndLineNumber - change.originalStartLineNumber
                                ) + 1;
                            }
                        });
                        JSON.stringify(stats);
                    }
                }
            ");

            if (result != null)
            {
                return JsonSerializer.Deserialize<DiffStatistics>(result.ToString());
            }
            return null;
        }

        #endregion

        #region HTML Generation

        private string GetMonacoDiffHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8' />
    <title>Monaco Diff Editor</title>
    <style>
        body { margin: 0; padding: 0; overflow: hidden; }
        #container { position: absolute; left: 0; top: 0; right: 0; bottom: 0; }
    </style>
</head>
<body>
    <div id='container'></div>
    <script src='https://cdn.jsdelivr.net/npm/monaco-editor@0.44.0/min/vs/loader.js'></script>
    <script>
        window.diffEditorReady = false;

        require.config({
            paths: {
                vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.44.0/min/vs'
            }
        });

        require(['vs/editor/editor.main'], function() {
            // Create diff editor
            window.diffEditor = monaco.editor.createDiffEditor(document.getElementById('container'), {
                automaticLayout: true,
                renderSideBySide: true,
                ignoreTrimWhitespace: true,
                readOnly: false,
                theme: 'vs-dark',
                originalEditable: false,
                scrollBeyondLastLine: false,
                minimap: {
                    enabled: true
                }
            });

            // Set up event handlers
            var originalEditor = window.diffEditor.getOriginalEditor();
            var modifiedEditor = window.diffEditor.getModifiedEditor();

            originalEditor.onDidChangeModelContent(function() {
                if (window.onOriginalChange) {
                    window.onOriginalChange(originalEditor.getValue());
                }
            });

            modifiedEditor.onDidChangeModelContent(function() {
                if (window.onModifiedChange) {
                    window.onModifiedChange(modifiedEditor.getValue());
                }
            });

            window.diffEditor.onDidUpdateDiff(function() {
                if (window.onDiffUpdate) {
                    var lineChanges = window.diffEditor.getLineChanges();
                    window.onDiffUpdate(lineChanges);
                }
            });

            // Mark as ready
            window.diffEditorReady = true;
        });

        // Error handler
        window.onerror = function(msg, url, lineNo, columnNo, error) {
            console.error('JavaScript error:', msg);
            return false;
        };
    </script>
</body>
</html>";
        }

        #endregion
    }

    #region Supporting Classes

    public class DiffEditorOptions
    {
        [JsonPropertyName("renderSideBySide")]
        public bool? RenderSideBySide { get; set; }

        [JsonPropertyName("ignoreTrimWhitespace")]
        public bool? IgnoreTrimWhitespace { get; set; }

        [JsonPropertyName("readOnly")]
        public bool? ReadOnly { get; set; }

        [JsonPropertyName("originalEditable")]
        public bool? OriginalEditable { get; set; }

        [JsonPropertyName("enableSplitViewResizing")]
        public bool? EnableSplitViewResizing { get; set; }

        [JsonPropertyName("renderIndicators")]
        public bool? RenderIndicators { get; set; }

        [JsonPropertyName("maxComputationTime")]
        public int? MaxComputationTime { get; set; }

        [JsonPropertyName("maxFileSize")]
        public int? MaxFileSize { get; set; }

        [JsonPropertyName("followsCaret")]
        public bool? FollowsCaret { get; set; }

        [JsonPropertyName("renderOverviewRuler")]
        public bool? RenderOverviewRuler { get; set; }
    }

    public class DiffEditorChangedEventArgs : EventArgs
    {
        public List<LineChange> LineChanges { get; set; }
    }

    public class LineChange
    {
        [JsonPropertyName("originalStartLineNumber")]
        public int OriginalStartLineNumber { get; set; }

        [JsonPropertyName("originalEndLineNumber")]
        public int OriginalEndLineNumber { get; set; }

        [JsonPropertyName("modifiedStartLineNumber")]
        public int ModifiedStartLineNumber { get; set; }

        [JsonPropertyName("modifiedEndLineNumber")]
        public int ModifiedEndLineNumber { get; set; }

        [JsonPropertyName("charChanges")]
        public List<CharChange> CharChanges { get; set; }
    }

    public class CharChange
    {
        [JsonPropertyName("originalStartLineNumber")]
        public int OriginalStartLineNumber { get; set; }

        [JsonPropertyName("originalStartColumn")]
        public int OriginalStartColumn { get; set; }

        [JsonPropertyName("originalEndLineNumber")]
        public int OriginalEndLineNumber { get; set; }

        [JsonPropertyName("originalEndColumn")]
        public int OriginalEndColumn { get; set; }

        [JsonPropertyName("modifiedStartLineNumber")]
        public int ModifiedStartLineNumber { get; set; }

        [JsonPropertyName("modifiedStartColumn")]
        public int ModifiedStartColumn { get; set; }

        [JsonPropertyName("modifiedEndLineNumber")]
        public int ModifiedEndLineNumber { get; set; }

        [JsonPropertyName("modifiedEndColumn")]
        public int ModifiedEndColumn { get; set; }
    }

    public class DiffStatistics
    {
        [JsonPropertyName("additions")]
        public int Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }

        [JsonPropertyName("modifications")]
        public int Modifications { get; set; }

        public int TotalChanges => Additions + Deletions + Modifications;
    }

    #endregion
}
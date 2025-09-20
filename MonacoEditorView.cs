using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace Flynk.Apps.Maui.Monaco
{
    public class MonacoEditorView : Grid
    {
        protected WebView _webView;
        private ActivityIndicator _loadingIndicator;
        private Label _loadingLabel;
        private Grid _loadingOverlay;
        private string _cachedCode;
        private string _cachedLanguage;
        private string _cachedTheme;

        public static readonly BindableProperty CodeProperty = BindableProperty.Create(
            nameof(Code),
            typeof(string),
            typeof(MonacoEditorView),
            string.Empty,
            propertyChanged: OnCodePropertyChanged);

        public static readonly BindableProperty LanguageProperty = BindableProperty.Create(
            nameof(Language),
            typeof(string),
            typeof(MonacoEditorView),
            "javascript",
            propertyChanged: OnLanguagePropertyChanged);

        public static readonly BindableProperty ThemeProperty = BindableProperty.Create(
            nameof(Theme),
            typeof(string),
            typeof(MonacoEditorView),
            "vs-dark",
            propertyChanged: OnThemePropertyChanged);

        public static readonly BindableProperty ReadOnlyProperty = BindableProperty.Create(
            nameof(ReadOnly),
            typeof(bool),
            typeof(MonacoEditorView),
            false,
            propertyChanged: OnReadOnlyPropertyChanged);

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
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

        public event EventHandler<string> CodeChanged;

        protected bool _isInitialized = false;

        public MonacoEditorView()
        {
            BuildUI();
            InitializeWebView();
        }

        private void BuildUI()
        {
            // Create WebView
            _webView = new WebView();

            // Create loading overlay
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
                Text = "Loading Monaco Editor...",
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 14
            };

            loadingContent.Children.Add(_loadingIndicator);
            loadingContent.Children.Add(_loadingLabel);
            _loadingOverlay.Children.Add(loadingContent);

            // Add to grid
            this.Children.Add(_webView);
            this.Children.Add(_loadingOverlay);
        }

        private void InitializeWebView()
        {
            var html = GetMonacoHtml();
            var baseUrl = "https://localhost";

            #if WINDOWS
            _webView.Source = new HtmlWebViewSource { Html = html, BaseUrl = baseUrl };
            #else
            _webView.Source = new HtmlWebViewSource { Html = html };
            #endif

            _webView.Navigated += OnNavigated;
        }

        private async void OnNavigated(object sender, WebNavigatedEventArgs e)
        {
            if (e.Result == WebNavigationResult.Success && !_isInitialized)
            {
                _isInitialized = true;
                Console.WriteLine("[MonacoEditorView] WebView navigated successfully");

                // Initial delay for Monaco to load
                await Task.Delay(500);

                // Update loading label
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _loadingLabel.Text = "Initializing editor...";
                });

                // Check if editor is ready with exponential backoff
                int delay = 100;
                bool editorReady = false;

                for (int i = 0; i < 15; i++)
                {
                    var ready = await _webView.EvaluateJavaScriptAsync("window.editorReady === true");
                    Console.WriteLine($"[MonacoEditorView] Editor ready check {i}: {ready} (delay: {delay}ms)");

                    if (ready?.ToString()?.ToLower() == "true")
                    {
                        Console.WriteLine("[MonacoEditorView] Editor is ready!");
                        editorReady = true;
                        break;
                    }

                    // Update loading label with progress
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _loadingLabel.Text = $"Loading editor... ({i+1}/15)";
                    });

                    await Task.Delay(delay);
                    // Exponential backoff with max delay of 2000ms
                    delay = Math.Min(delay * 2, 2000);
                }

                if (editorReady)
                {
                    await InitializeEditor();
                    await ApplyCachedContent();

                    // Hide loading overlay
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _loadingOverlay.IsVisible = false;
                        _loadingIndicator.IsRunning = false;
                    });
                }
                else
                {
                    // Show error if editor failed to load
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _loadingLabel.Text = "Failed to load editor";
                        _loadingLabel.TextColor = Colors.Red;
                    });
                }

                // Get debug logs
                var logs = await _webView.EvaluateJavaScriptAsync("JSON.stringify(window.debugLog || [])");
                Console.WriteLine($"[MonacoEditorView] Debug logs from JavaScript: {logs}");
            }
        }

        protected virtual async Task InitializeEditor()
        {
            // Use idempotent setters for initial configuration
            await _webView.EvaluateJavaScriptAsync($"window.setEditorTheme('{Theme}')");
            await _webView.EvaluateJavaScriptAsync($"window.setEditorLanguage('{Language}')");

            // Set read-only if needed
            if (ReadOnly)
            {
                await _webView.EvaluateJavaScriptAsync($@"
                    window.queueOrExecute(function() {{
                        if (window.editor) {{
                            window.editor.updateOptions({{ readOnly: true }});
                        }}
                    }});
                ");
            }

        }

        private async Task ApplyCachedContent()
        {
            Console.WriteLine("[MonacoEditorView] Checking for cached content...");

            // Apply cached theme if exists
            if (!string.IsNullOrEmpty(_cachedTheme))
            {
                Console.WriteLine($"[MonacoEditorView] Applying cached theme: {_cachedTheme}");
                await _webView.EvaluateJavaScriptAsync($"window.setEditorTheme('{_cachedTheme}')");
            }

            // Apply cached language if exists
            if (!string.IsNullOrEmpty(_cachedLanguage))
            {
                Console.WriteLine($"[MonacoEditorView] Applying cached language: {_cachedLanguage}");
                await _webView.EvaluateJavaScriptAsync($"window.setEditorLanguage('{_cachedLanguage}')");
            }

            // Apply cached code if exists
            if (!string.IsNullOrEmpty(_cachedCode))
            {
                Console.WriteLine($"[MonacoEditorView] Applying cached code: {_cachedCode.Length} chars");
                await SetValueSafely(_cachedCode);
                _cachedCode = null; // Clear cache after applying
            }
        }

        private static async void OnCodePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoEditorView editor)
            {
                var code = newValue as string ?? "";

                if (editor._isInitialized)
                {
                    // Editor is ready, set directly
                    await editor.SetValueSafely(code);
                }
                else
                {
                    // Editor not ready, cache for later
                    Console.WriteLine($"[MonacoEditorView] Editor not ready, caching code ({code.Length} chars)");
                    editor._cachedCode = code;
                }
            }
        }

        private static async void OnLanguagePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoEditorView editor && newValue != null)
            {
                if (editor._isInitialized)
                {
                    // Use idempotent language setter
                    await editor._webView.EvaluateJavaScriptAsync($"window.setEditorLanguage('{newValue}')");
                }
                else
                {
                    // Cache for later
                    Console.WriteLine($"[MonacoEditorView] Editor not ready, caching language: {newValue}");
                    editor._cachedLanguage = newValue.ToString();
                }
            }
        }

        private static async void OnThemePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoEditorView editor && newValue != null)
            {
                if (editor._isInitialized)
                {
                    // Use idempotent theme setter
                    await editor._webView.EvaluateJavaScriptAsync($"window.setEditorTheme('{newValue}')");
                }
                else
                {
                    // Cache for later
                    Console.WriteLine($"[MonacoEditorView] Editor not ready, caching theme: {newValue}");
                    editor._cachedTheme = newValue.ToString();
                }
            }
        }

        private static async void OnReadOnlyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is MonacoEditorView editor && editor._isInitialized)
            {
                await editor._webView.EvaluateJavaScriptAsync($"if (window.editor) {{ window.editor.updateOptions({{ readOnly: {newValue.ToString().ToLower()} }}); }}");
            }
        }

        public async Task<string> GetEditorValue()
        {
            if (_isInitialized)
            {
                var result = await _webView.EvaluateJavaScriptAsync("window.editor ? window.editor.getValue() : ''");
                return result?.ToString() ?? string.Empty;
            }
            return Code;
        }

        public async Task<bool> SetValueSafely(string code)
        {
            try
            {
                Console.WriteLine($"[MonacoEditorView] SetValueSafely called with code length: {code?.Length ?? 0}");

                if (!_isInitialized)
                {
                    Console.WriteLine("[MonacoEditorView] Editor not initialized, waiting...");
                    await Task.Delay(1000);
                }

                var cleanCode = CodeEditorHelper.CleanLambdaCode(code);
                Console.WriteLine($"[MonacoEditorView] Cleaned code length: {cleanCode.Length}");

                // Method 1: Try base64 encoding
                var base64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cleanCode));
                var script = $"window.setEditorValue('{base64}')";;
                var result = await _webView.EvaluateJavaScriptAsync(script);
                Console.WriteLine($"[MonacoEditorView] Base64 method result: {result}");

                if (result?.ToString()?.Contains("success") == true)
                {
                    return true;
                }

                // Method 2: Try direct with heavy escaping
                var escaped = System.Text.Json.JsonSerializer.Serialize(cleanCode);
                script = $"window.setEditorValueDirect({escaped})";
                result = await _webView.EvaluateJavaScriptAsync(script);
                Console.WriteLine($"[MonacoEditorView] Direct method result: {result}");

                if (result?.ToString()?.Contains("success") == true)
                {
                    return true;
                }

                // Method 3: Fallback to property
                Console.WriteLine("[MonacoEditorView] Falling back to property setter");
                Code = cleanCode;

                // Get debug logs
                var logs = await _webView.EvaluateJavaScriptAsync("JSON.stringify(window.debugLog || [])");
                Console.WriteLine($"[MonacoEditorView] JavaScript debug logs: {logs}");

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MonacoEditorView] Error in SetValueSafely: {ex.Message}");
                return false;
            }
        }

        private string GetMonacoHtml()
        {
            return @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>Monaco Editor</title>
    <style>
        body { margin: 0; padding: 0; overflow: hidden; }
        #container { position: absolute; left: 0; top: 0; right: 0; bottom: 0; }
    </style>
</head>
<body>
    <div id=""container""></div>
    <script src=""https://cdn.jsdelivr.net/npm/monaco-editor@0.44.0/min/vs/loader.js""></script>
    <script>
        // Debug logging
        window.debugLog = [];
        window.logDebug = function(msg) {
            console.log('[Monaco]', msg);
            window.debugLog.push(msg);
        };

        // Queue system for operations
        window.operationQueue = [];
        window.isProcessingQueue = false;
        window.currentLanguage = 'javascript';
        window.currentTheme = 'vs-dark';

        // Process queued operations
        window.processQueue = function() {
            if (window.isProcessingQueue || !window.editorReady || window.operationQueue.length === 0) {
                return;
            }

            window.isProcessingQueue = true;
            window.logDebug('Processing queue with ' + window.operationQueue.length + ' operations');

            while (window.operationQueue.length > 0) {
                var operation = window.operationQueue.shift();
                try {
                    operation();
                } catch (e) {
                    window.logDebug('Error processing queued operation: ' + e.toString());
                }
            }

            window.isProcessingQueue = false;
        };

        // Queue an operation or execute immediately if ready
        window.queueOrExecute = function(operation) {
            if (window.editorReady) {
                try {
                    operation();
                } catch (e) {
                    window.logDebug('Error executing operation: ' + e.toString());
                }
            } else {
                window.logDebug('Editor not ready, queuing operation');
                window.operationQueue.push(operation);
            }
        };

        // Idempotent language setter
        window.setEditorLanguage = function(language) {
            window.queueOrExecute(function() {
                if (!window.editor) return;

                var model = window.editor.getModel();
                if (!model) return;

                var currentLang = model.getLanguageId();
                if (currentLang !== language) {
                    window.logDebug('Changing language from ' + currentLang + ' to ' + language);
                    monaco.editor.setModelLanguage(model, language);
                    window.currentLanguage = language;
                } else {
                    window.logDebug('Language already set to ' + language + ', skipping');
                }
            });
        };

        // Idempotent theme setter
        window.setEditorTheme = function(theme) {
            window.queueOrExecute(function() {
                if (window.currentTheme !== theme) {
                    window.logDebug('Changing theme from ' + window.currentTheme + ' to ' + theme);
                    monaco.editor.setTheme(theme);
                    window.currentTheme = theme;
                } else {
                    window.logDebug('Theme already set to ' + theme + ', skipping');
                }
            });
        };

        // Create a more robust setValue function
        window.setEditorValue = function(base64Value) {
            window.logDebug('setEditorValue called with base64 length: ' + (base64Value ? base64Value.length : 0));

            var setValue = function() {
                try {
                    if (!window.editor) {
                        window.logDebug('Editor still not available');
                        return 'not-ready';
                    }

                    // Decode from base64
                    var decoded = atob(base64Value);
                    window.logDebug('Decoded length: ' + decoded.length);

                    // Convert to proper UTF-8
                    var code = decodeURIComponent(escape(decoded));
                    window.logDebug('Final code length: ' + code.length);

                    // Set the value
                    window.editor.setValue(code);
                    window.logDebug('Value set successfully');

                    return 'success';
                } catch (e) {
                    window.logDebug('Error in setValue: ' + e.toString());
                    return 'error: ' + e.toString();
                }
            };

            if (window.editorReady) {
                return setValue();
            } else {
                window.logDebug('Editor not ready, queuing setValue operation');
                window.queueOrExecute(setValue);
                return 'queued';
            }
        };

        // Alternative direct set method
        window.setEditorValueDirect = function(value) {
            try {
                window.logDebug('setEditorValueDirect called with length: ' + (value ? value.length : 0));

                if (!window.editor) {
                    window.logDebug('Editor not initialized yet (direct)');
                    window.pendingValueDirect = value;
                    return 'pending';
                }

                window.editor.setValue(value);
                window.logDebug('Direct value set successfully');
                return 'success';
            } catch (e) {
                window.logDebug('Error in setEditorValueDirect: ' + e.toString());
                return 'error: ' + e.toString();
            }
        };

        // Check if editor is truly ready
        window.isEditorReady = function() {
            return window.editor &&
                   window.editor.getModel &&
                   window.editor.getModel() !== null &&
                   typeof monaco !== 'undefined' &&
                   monaco.editor;
        };

        require.config({ paths: { vs: 'https://cdn.jsdelivr.net/npm/monaco-editor@0.44.0/min/vs' } });

        window.logDebug('Starting Monaco initialization');

        require(['vs/editor/editor.main'], function() {
            window.logDebug('Monaco loaded, creating editor');

            window.editor = monaco.editor.create(document.getElementById('container'), {
                value: '',
                language: window.currentLanguage,
                theme: window.currentTheme,
                automaticLayout: true,
                minimap: { enabled: true },
                fontSize: 14,
                wordWrap: 'on',
                scrollBeyondLastLine: false,
                renderWhitespace: 'selection',
                folding: true,
                lineNumbers: 'on',
                lineDecorationsWidth: 10,
                lineNumbersMinChars: 4,
                glyphMargin: true,
                contextmenu: true,
                quickSuggestions: true,
                suggestOnTriggerCharacters: true,
                acceptSuggestionOnEnter: 'on',
                tabCompletion: 'on',
                wordBasedSuggestions: true
            });

            window.logDebug('Editor created successfully');

            // Set up change event
            window.editor.onDidChangeModelContent(function() {
                if (window.onEditorChange) {
                    window.onEditorChange(window.editor.getValue());
                }
            });

            // Wait a bit for the editor to fully initialize
            setTimeout(function() {
                if (window.isEditorReady()) {
                    // Signal that editor is ready
                    window.editorReady = true;
                    window.logDebug('Editor fully initialized and ready');

                    // Process any queued operations
                    window.processQueue();
                } else {
                    window.logDebug('Editor not fully ready, waiting more...');
                    // Try again
                    setTimeout(function() {
                        window.editorReady = true;
                        window.logDebug('Editor ready after extended wait');
                        window.processQueue();
                    }, 500);
                }
            }, 100);
        });

        // Error handler
        window.onerror = function(msg, url, lineNo, columnNo, error) {
            window.logDebug('JavaScript error: ' + msg);
            return false;
        };
    </script>
</body>
</html>";
        }
    }
}
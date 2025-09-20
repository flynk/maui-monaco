using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Flynk.Apps.Maui.Monaco.Models;

namespace Flynk.Apps.Maui.Monaco.Services
{
    /// <summary>
    /// Service for managing Monaco language features, intellisense, and custom language definitions
    /// </summary>
    public class MonacoLanguageService
    {
        private readonly WebView _webView;
        private readonly Dictionary<string, object> _completionProviders = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _hoverProviders = new Dictionary<string, object>();
        private readonly Dictionary<string, object> _signatureProviders = new Dictionary<string, object>();

        public MonacoLanguageService(WebView webView)
        {
            _webView = webView;
        }

        #region Language Registration

        /// <summary>
        /// Register a new language with Monaco
        /// </summary>
        public async Task RegisterLanguage(string id, LanguageConfiguration config)
        {
            var json = JsonSerializer.Serialize(new { id, config }, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages) {{
                    var config = {json};
                    monaco.languages.register({{ id: config.id }});
                    if (config.config) {{
                        monaco.languages.setLanguageConfiguration(config.id, config.config);
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Set language configuration
        /// </summary>
        public async Task SetLanguageConfiguration(string languageId, LanguageConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.setLanguageConfiguration({JsonSerializer.Serialize(languageId)}, {json});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Set monarch tokens provider for syntax highlighting
        /// </summary>
        public async Task SetMonarchTokensProvider(string languageId, MonarchLanguage tokens)
        {
            var json = JsonSerializer.Serialize(tokens, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.setMonarchTokensProvider({JsonSerializer.Serialize(languageId)}, {json});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Completion Provider

        /// <summary>
        /// Register a completion item provider
        /// </summary>
        public async Task RegisterCompletionItemProvider(string languageId, CompletionItemProvider provider)
        {
            var providerId = Guid.NewGuid().ToString();
            _completionProviders[providerId] = provider;

            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerCompletionItemProvider({JsonSerializer.Serialize(languageId)}, {{
                        triggerCharacters: {JsonSerializer.Serialize(provider.TriggerCharacters ?? new List<string>())},
                        provideCompletionItems: function(model, position, context, token) {{
                            return window.invokeCompletionProvider(
                                {JsonSerializer.Serialize(providerId)},
                                model.uri.toString(),
                                position,
                                context
                            );
                        }},
                        resolveCompletionItem: function(item, token) {{
                            return window.resolveCompletionItem(
                                {JsonSerializer.Serialize(providerId)},
                                item
                            );
                        }}
                    }});
                }}

                window.invokeCompletionProvider = async function(providerId, modelUri, position, context) {{
                    // This would be connected to C# completion provider
                    console.log('Completion requested for', providerId, modelUri, position);
                    return {{
                        suggestions: []
                    }};
                }};

                window.resolveCompletionItem = async function(providerId, item) {{
                    // This would be connected to C# completion resolver
                    console.log('Resolve completion item', providerId, item);
                    return item;
                }};
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Set static completion items for a language
        /// </summary>
        public async Task SetStaticCompletionItems(string languageId, List<CompletionItem> items)
        {
            var json = JsonSerializer.Serialize(items, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages) {{
                    var staticItems = {json};
                    monaco.languages.registerCompletionItemProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideCompletionItems: function(model, position, context, token) {{
                            return {{
                                suggestions: staticItems
                            }};
                        }}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Hover Provider

        /// <summary>
        /// Register a hover provider
        /// </summary>
        public async Task RegisterHoverProvider(string languageId, HoverProvider provider)
        {
            var providerId = Guid.NewGuid().ToString();
            _hoverProviders[providerId] = provider;

            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerHoverProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideHover: function(model, position, token) {{
                            return window.invokeHoverProvider(
                                {JsonSerializer.Serialize(providerId)},
                                model.uri.toString(),
                                position
                            );
                        }}
                    }});
                }}

                window.invokeHoverProvider = async function(providerId, modelUri, position) {{
                    // This would be connected to C# hover provider
                    console.log('Hover requested for', providerId, modelUri, position);
                    return null;
                }};
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Definition Provider

        /// <summary>
        /// Register a definition provider
        /// </summary>
        public async Task RegisterDefinitionProvider(string languageId)
        {
            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerDefinitionProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideDefinition: function(model, position, token) {{
                            // Custom definition logic
                            return null;
                        }}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Format Provider

        /// <summary>
        /// Register document formatting provider
        /// </summary>
        public async Task RegisterDocumentFormattingEditProvider(string languageId)
        {
            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerDocumentFormattingEditProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideDocumentFormattingEdits: function(model, options, token) {{
                            // Custom formatting logic
                            return [];
                        }}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Register range formatting provider
        /// </summary>
        public async Task RegisterDocumentRangeFormattingEditProvider(string languageId)
        {
            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerDocumentRangeFormattingEditProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideDocumentRangeFormattingEdits: function(model, range, options, token) {{
                            // Custom range formatting logic
                            return [];
                        }}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Code Actions Provider

        /// <summary>
        /// Register code actions provider (quick fixes)
        /// </summary>
        public async Task RegisterCodeActionProvider(string languageId)
        {
            var script = $@"
                if (monaco && monaco.languages) {{
                    monaco.languages.registerCodeActionProvider({JsonSerializer.Serialize(languageId)}, {{
                        provideCodeActions: function(model, range, context, token) {{
                            // Custom code actions logic
                            return {{
                                actions: [],
                                dispose: function() {{}}
                            }};
                        }}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region Diagnostic Providers

        /// <summary>
        /// Set diagnostics/markers for a model
        /// </summary>
        public async Task SetModelMarkers(string owner, string resourceUri, List<MarkerData> markers)
        {
            var markersJson = JsonSerializer.Serialize(markers, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.editor) {{
                    var uri = monaco.Uri.parse({JsonSerializer.Serialize(resourceUri)});
                    monaco.editor.setModelMarkers(
                        monaco.editor.getModel(uri),
                        {JsonSerializer.Serialize(owner)},
                        {markersJson}
                    );
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region JSON Schema Support

        /// <summary>
        /// Configure JSON schemas for validation
        /// </summary>
        public async Task ConfigureJsonDefaults(JsonDefaultsConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages && monaco.languages.json) {{
                    var config = {json};
                    monaco.languages.json.jsonDefaults.setDiagnosticsOptions({{
                        validate: config.validate || true,
                        schemas: config.schemas || [],
                        allowComments: config.allowComments || false,
                        enableSchemaRequest: config.enableSchemaRequest || true
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region TypeScript/JavaScript Support

        /// <summary>
        /// Configure TypeScript/JavaScript defaults
        /// </summary>
        public async Task ConfigureJavaScriptDefaults(JavaScriptDefaultsConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages && monaco.languages.typescript) {{
                    var config = {json};

                    // Configure JavaScript defaults
                    monaco.languages.typescript.javascriptDefaults.setCompilerOptions({{
                        target: monaco.languages.typescript.ScriptTarget.ES2020,
                        allowNonTsExtensions: true,
                        moduleResolution: monaco.languages.typescript.ModuleResolutionKind.NodeJs,
                        module: monaco.languages.typescript.ModuleKind.CommonJS,
                        noLib: config.noLib || false,
                        allowJs: true,
                        checkJs: config.checkJs || false
                    }});

                    // Configure TypeScript defaults
                    monaco.languages.typescript.typescriptDefaults.setCompilerOptions({{
                        target: monaco.languages.typescript.ScriptTarget.ES2020,
                        allowNonTsExtensions: true,
                        moduleResolution: monaco.languages.typescript.ModuleResolutionKind.NodeJs,
                        module: monaco.languages.typescript.ModuleKind.CommonJS,
                        noLib: config.noLib || false,
                        allowJs: true,
                        strict: config.strict || false
                    }});

                    // Add extra libraries if provided
                    if (config.extraLibs) {{
                        config.extraLibs.forEach(function(lib) {{
                            monaco.languages.typescript.javascriptDefaults.addExtraLib(lib.content, lib.filePath);
                            monaco.languages.typescript.typescriptDefaults.addExtraLib(lib.content, lib.filePath);
                        }});
                    }}
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion

        #region HTML/CSS Support

        /// <summary>
        /// Configure HTML defaults
        /// </summary>
        public async Task ConfigureHtmlDefaults(HtmlDefaultsConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages && monaco.languages.html) {{
                    var config = {json};
                    monaco.languages.html.htmlDefaults.setOptions({{
                        format: config.format || {{}},
                        suggest: config.suggest || {{}}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        /// <summary>
        /// Configure CSS defaults
        /// </summary>
        public async Task ConfigureCssDefaults(CssDefaultsConfiguration config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var script = $@"
                if (monaco && monaco.languages && monaco.languages.css) {{
                    var config = {json};
                    monaco.languages.css.cssDefaults.setOptions({{
                        validate: config.validate || true,
                        lint: config.lint || {{}}
                    }});
                }}
            ";

            await _webView.EvaluateJavaScriptAsync(script);
        }

        #endregion
    }

    #region Supporting Classes

    public class LanguageConfiguration
    {
        [JsonPropertyName("comments")]
        public CommentRule Comments { get; set; }

        [JsonPropertyName("brackets")]
        public List<List<string>> Brackets { get; set; }

        [JsonPropertyName("wordPattern")]
        public string WordPattern { get; set; }

        [JsonPropertyName("indentationRules")]
        public IndentationRule IndentationRules { get; set; }

        [JsonPropertyName("onEnterRules")]
        public List<OnEnterRule> OnEnterRules { get; set; }

        [JsonPropertyName("autoClosingPairs")]
        public List<AutoClosingPair> AutoClosingPairs { get; set; }

        [JsonPropertyName("surroundingPairs")]
        public List<AutoClosingPair> SurroundingPairs { get; set; }

        [JsonPropertyName("folding")]
        public FoldingRules Folding { get; set; }
    }

    public class CommentRule
    {
        [JsonPropertyName("lineComment")]
        public string LineComment { get; set; }

        [JsonPropertyName("blockComment")]
        public List<string> BlockComment { get; set; }
    }

    public class IndentationRule
    {
        [JsonPropertyName("increaseIndentPattern")]
        public string IncreaseIndentPattern { get; set; }

        [JsonPropertyName("decreaseIndentPattern")]
        public string DecreaseIndentPattern { get; set; }

        [JsonPropertyName("indentNextLinePattern")]
        public string IndentNextLinePattern { get; set; }

        [JsonPropertyName("unIndentedLinePattern")]
        public string UnIndentedLinePattern { get; set; }
    }

    public class OnEnterRule
    {
        [JsonPropertyName("beforeText")]
        public string BeforeText { get; set; }

        [JsonPropertyName("afterText")]
        public string AfterText { get; set; }

        [JsonPropertyName("action")]
        public EnterAction Action { get; set; }
    }

    public class EnterAction
    {
        [JsonPropertyName("indentAction")]
        public string IndentAction { get; set; }

        [JsonPropertyName("appendText")]
        public string AppendText { get; set; }

        [JsonPropertyName("removeText")]
        public int? RemoveText { get; set; }
    }

    public class AutoClosingPair
    {
        [JsonPropertyName("open")]
        public string Open { get; set; }

        [JsonPropertyName("close")]
        public string Close { get; set; }

        [JsonPropertyName("notIn")]
        public List<string> NotIn { get; set; }
    }

    public class FoldingRules
    {
        [JsonPropertyName("offSide")]
        public bool? OffSide { get; set; }

        [JsonPropertyName("markers")]
        public FoldingMarkers Markers { get; set; }
    }

    public class FoldingMarkers
    {
        [JsonPropertyName("start")]
        public string Start { get; set; }

        [JsonPropertyName("end")]
        public string End { get; set; }
    }

    public class MonarchLanguage
    {
        [JsonPropertyName("tokenizer")]
        public Dictionary<string, List<TokenRule>> Tokenizer { get; set; }

        [JsonPropertyName("keywords")]
        public List<string> Keywords { get; set; }

        [JsonPropertyName("typeKeywords")]
        public List<string> TypeKeywords { get; set; }

        [JsonPropertyName("operators")]
        public List<string> Operators { get; set; }

        [JsonPropertyName("symbols")]
        public string Symbols { get; set; }

        [JsonPropertyName("escapes")]
        public string Escapes { get; set; }

        [JsonPropertyName("digits")]
        public string Digits { get; set; }

        [JsonPropertyName("octaldigits")]
        public string OctalDigits { get; set; }

        [JsonPropertyName("binarydigits")]
        public string BinaryDigits { get; set; }

        [JsonPropertyName("hexdigits")]
        public string HexDigits { get; set; }
    }

    public class TokenRule
    {
        [JsonPropertyName("regex")]
        public string Regex { get; set; }

        [JsonPropertyName("action")]
        public object Action { get; set; }
    }

    public class CompletionItemProvider
    {
        public List<string> TriggerCharacters { get; set; }
        public Func<string, Position, Task<CompletionList>> ProvideCompletionItems { get; set; }
        public Func<CompletionItem, Task<CompletionItem>> ResolveCompletionItem { get; set; }
    }

    public class HoverProvider
    {
        public Func<string, Position, Task<Hover>> ProvideHover { get; set; }
    }

    public class Hover
    {
        [JsonPropertyName("contents")]
        public List<MarkdownString> Contents { get; set; }

        [JsonPropertyName("range")]
        public Models.Range Range { get; set; }
    }

    public class MarkdownString
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("isTrusted")]
        public bool IsTrusted { get; set; }

        [JsonPropertyName("supportHtml")]
        public bool SupportHtml { get; set; }
    }

    public class JsonDefaultsConfiguration
    {
        [JsonPropertyName("validate")]
        public bool Validate { get; set; } = true;

        [JsonPropertyName("schemas")]
        public List<JsonSchema> Schemas { get; set; }

        [JsonPropertyName("allowComments")]
        public bool AllowComments { get; set; }

        [JsonPropertyName("enableSchemaRequest")]
        public bool EnableSchemaRequest { get; set; } = true;
    }

    public class JsonSchema
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("fileMatch")]
        public List<string> FileMatch { get; set; }

        [JsonPropertyName("schema")]
        public object Schema { get; set; }
    }

    public class JavaScriptDefaultsConfiguration
    {
        [JsonPropertyName("noLib")]
        public bool NoLib { get; set; }

        [JsonPropertyName("checkJs")]
        public bool CheckJs { get; set; }

        [JsonPropertyName("strict")]
        public bool Strict { get; set; }

        [JsonPropertyName("extraLibs")]
        public List<ExtraLib> ExtraLibs { get; set; }
    }

    public class ExtraLib
    {
        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }
    }

    public class HtmlDefaultsConfiguration
    {
        [JsonPropertyName("format")]
        public HtmlFormatConfiguration Format { get; set; }

        [JsonPropertyName("suggest")]
        public HtmlSuggestConfiguration Suggest { get; set; }
    }

    public class HtmlFormatConfiguration
    {
        [JsonPropertyName("indentSize")]
        public int IndentSize { get; set; }

        [JsonPropertyName("wrapLineLength")]
        public int WrapLineLength { get; set; }

        [JsonPropertyName("unformatted")]
        public string Unformatted { get; set; }

        [JsonPropertyName("contentUnformatted")]
        public string ContentUnformatted { get; set; }

        [JsonPropertyName("indentInnerHtml")]
        public bool IndentInnerHtml { get; set; }

        [JsonPropertyName("preserveNewLines")]
        public bool PreserveNewLines { get; set; }

        [JsonPropertyName("maxPreserveNewLines")]
        public int MaxPreserveNewLines { get; set; }

        [JsonPropertyName("indentHandlebars")]
        public bool IndentHandlebars { get; set; }

        [JsonPropertyName("endWithNewline")]
        public bool EndWithNewline { get; set; }

        [JsonPropertyName("extraLiners")]
        public string ExtraLiners { get; set; }

        [JsonPropertyName("wrapAttributes")]
        public string WrapAttributes { get; set; }
    }

    public class HtmlSuggestConfiguration
    {
        [JsonPropertyName("html5")]
        public bool Html5 { get; set; } = true;

        [JsonPropertyName("angular1")]
        public bool Angular1 { get; set; }

        [JsonPropertyName("ionic")]
        public bool Ionic { get; set; }
    }

    public class CssDefaultsConfiguration
    {
        [JsonPropertyName("validate")]
        public bool Validate { get; set; } = true;

        [JsonPropertyName("lint")]
        public CssLintConfiguration Lint { get; set; }
    }

    public class CssLintConfiguration
    {
        [JsonPropertyName("compatibleVendorPrefixes")]
        public string CompatibleVendorPrefixes { get; set; }

        [JsonPropertyName("vendorPrefix")]
        public string VendorPrefix { get; set; }

        [JsonPropertyName("duplicateProperties")]
        public string DuplicateProperties { get; set; }

        [JsonPropertyName("emptyRules")]
        public string EmptyRules { get; set; }

        [JsonPropertyName("importStatement")]
        public string ImportStatement { get; set; }

        [JsonPropertyName("boxModel")]
        public string BoxModel { get; set; }

        [JsonPropertyName("universalSelector")]
        public string UniversalSelector { get; set; }

        [JsonPropertyName("zeroUnits")]
        public string ZeroUnits { get; set; }

        [JsonPropertyName("fontFaceProperties")]
        public string FontFaceProperties { get; set; }

        [JsonPropertyName("hexColorLength")]
        public string HexColorLength { get; set; }

        [JsonPropertyName("argumentsInColorFunction")]
        public string ArgumentsInColorFunction { get; set; }

        [JsonPropertyName("unknownProperties")]
        public string UnknownProperties { get; set; }

        [JsonPropertyName("ieHack")]
        public string IeHack { get; set; }

        [JsonPropertyName("unknownVendorSpecificProperties")]
        public string UnknownVendorSpecificProperties { get; set; }

        [JsonPropertyName("propertyIgnoredDueToDisplay")]
        public string PropertyIgnoredDueToDisplay { get; set; }

        [JsonPropertyName("important")]
        public string Important { get; set; }

        [JsonPropertyName("float")]
        public string Float { get; set; }

        [JsonPropertyName("idSelector")]
        public string IdSelector { get; set; }
    }

    #endregion
}
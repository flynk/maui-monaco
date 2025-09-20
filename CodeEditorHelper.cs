using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Flynk.Apps.Maui.Monaco
{
    public static class CodeEditorHelper
    {
        /// <summary>
        /// Properly escape code for JavaScript string literal
        /// This handles all special characters that can break JavaScript
        /// </summary>
        public static string EscapeForJavaScript(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            // Log diagnostic information
            Console.WriteLine($"[CodeEditorHelper] Original code length: {code.Length}");
            Console.WriteLine($"[CodeEditorHelper] Contains quotes: {code.Contains("\"")}");
            Console.WriteLine($"[CodeEditorHelper] Contains newlines: {code.Contains("\n")}");
            Console.WriteLine($"[CodeEditorHelper] Contains backslashes: {code.Contains("\\")}");

            // Remove BOM if present
            if (code.StartsWith("\uFEFF"))
            {
                Console.WriteLine("[CodeEditorHelper] Removing BOM marker");
                code = code.Substring(1);
            }

            // Remove null bytes
            if (code.Contains('\0'))
            {
                Console.WriteLine("[CodeEditorHelper] Removing null bytes");
                code = code.Replace("\0", "");
            }

            // Method 1: Use JSON serialization for proper escaping
            var jsonEscaped = System.Text.Json.JsonSerializer.Serialize(code);
            // Remove the surrounding quotes that JSON serialization adds
            jsonEscaped = jsonEscaped.Substring(1, jsonEscaped.Length - 2);

            Console.WriteLine($"[CodeEditorHelper] Escaped code length: {jsonEscaped.Length}");
            Console.WriteLine($"[CodeEditorHelper] First 100 chars of escaped: {jsonEscaped.Substring(0, Math.Min(100, jsonEscaped.Length))}");

            return jsonEscaped;
        }

        /// <summary>
        /// Alternative: Convert to Base64 to avoid all escaping issues
        /// </summary>
        public static string ToBase64(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            var bytes = Encoding.UTF8.GetBytes(code);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Create a safe JavaScript command to set editor value
        /// </summary>
        public static string CreateSetValueScript(string code, bool useBase64 = false)
        {
            if (useBase64)
            {
                var base64 = ToBase64(code);
                return $@"
                    (function() {{
                        try {{
                            if (typeof window.editor === 'undefined' || !window.editor) {{
                                console.error('Editor not initialized');
                                return false;
                            }}
                            var base64 = '{base64}';
                            var decoded = atob(base64);
                            var code = decodeURIComponent(escape(decoded));
                            window.editor.setValue(code);
                            console.log('Code set successfully via base64, length: ' + code.length);
                            return true;
                        }} catch (e) {{
                            console.error('Error setting editor value:', e);
                            return false;
                        }}
                    }})()";
            }
            else
            {
                var escaped = EscapeForJavaScript(code);
                return $@"
                    (function() {{
                        try {{
                            if (typeof window.editor === 'undefined' || !window.editor) {{
                                console.error('Editor not initialized');
                                return false;
                            }}
                            var code = ""{escaped}"";
                            window.editor.setValue(code);
                            console.log('Code set successfully, length: ' + code.length);
                            return true;
                        }} catch (e) {{
                            console.error('Error setting editor value:', e);
                            return false;
                        }}
                    }})()";
            }
        }

        /// <summary>
        /// Set Monaco editor value with retry logic
        /// </summary>
        public static async Task<bool> SetMonacoValue(WebView webView, string code, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // First check if editor is ready
                    var checkScript = "typeof window.editor !== 'undefined' && window.editor !== null";
                    var readyResult = await webView.EvaluateJavaScriptAsync(checkScript);

                    if (readyResult?.ToString()?.ToLower() != "true")
                    {
                        Console.WriteLine($"[CodeEditorHelper] Editor not ready, attempt {i + 1}/{maxRetries}");
                        await Task.Delay(500 * (i + 1)); // Progressive delay
                        continue;
                    }

                    // Try regular escaping first
                    var script = CreateSetValueScript(code, useBase64: false);
                    var result = await webView.EvaluateJavaScriptAsync(script);

                    if (result?.ToString()?.ToLower() == "true")
                    {
                        Console.WriteLine("[CodeEditorHelper] Successfully set code with regular escaping");
                        return true;
                    }

                    // If regular escaping failed, try base64
                    Console.WriteLine("[CodeEditorHelper] Regular escaping failed, trying base64");
                    script = CreateSetValueScript(code, useBase64: true);
                    result = await webView.EvaluateJavaScriptAsync(script);

                    if (result?.ToString()?.ToLower() == "true")
                    {
                        Console.WriteLine("[CodeEditorHelper] Successfully set code with base64");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CodeEditorHelper] Error on attempt {i + 1}: {ex.Message}");
                }

                if (i < maxRetries - 1)
                    await Task.Delay(1000);
            }

            Console.WriteLine("[CodeEditorHelper] Failed to set editor value after all retries");
            return false;
        }

        /// <summary>
        /// Set CodeMirror value with retry logic
        /// </summary>
        public static async Task<bool> SetCodeMirrorValue(WebView webView, string code, int maxRetries = 3)
        {
            // Same implementation as SetMonacoValue since both use window.editor
            return await SetMonacoValue(webView, code, maxRetries);
        }

        /// <summary>
        /// Clean and validate code from AWS Lambda
        /// </summary>
        public static string CleanLambdaCode(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            Console.WriteLine($"[CodeEditorHelper] Cleaning Lambda code, original length: {code.Length}");

            // Check if it's base64 encoded
            try
            {
                // Simple heuristic: if it doesn't contain newlines and looks like base64
                if (!code.Contains('\n') && Regex.IsMatch(code.Trim(), @"^[a-zA-Z0-9+/]*={0,2}$"))
                {
                    Console.WriteLine("[CodeEditorHelper] Detected base64 encoded code, decoding...");
                    var bytes = Convert.FromBase64String(code);
                    code = Encoding.UTF8.GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CodeEditorHelper] Not base64 or decode failed: {ex.Message}");
            }

            // Remove common issues
            code = code.Replace("\r\n", "\n"); // Normalize line endings
            code = code.Replace("\0", "");      // Remove null bytes
            code = code.TrimStart('\uFEFF');    // Remove BOM

            // Check for double-escaped content
            if (code.Contains("\\n") && !code.Contains("\n"))
            {
                Console.WriteLine("[CodeEditorHelper] Detected double-escaped content, unescaping...");
                code = Regex.Unescape(code);
            }

            Console.WriteLine($"[CodeEditorHelper] Cleaned code length: {code.Length}");
            return code;
        }
    }
}
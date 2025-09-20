using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;

namespace Flynk.Apps.Maui.Monaco
{
    /// <summary>
    /// Helper to create loading overlay for WebView-based editors
    /// </summary>
    public static class WebViewEditorHelper
    {
        public static Grid CreateEditorWithLoadingOverlay(WebView editorWebView, out ActivityIndicator loadingIndicator, out Label loadingLabel, out Grid loadingOverlay)
        {
            var containerGrid = new Grid();

            // Add the WebView
            containerGrid.Children.Add(editorWebView);

            // Create loading overlay
            loadingOverlay = new Grid
            {
                BackgroundColor = Colors.Black.WithAlpha(0.85f),
                IsVisible = true
            };

            var loadingContent = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 15
            };

            loadingIndicator = new ActivityIndicator
            {
                Color = Colors.White,
                IsRunning = true,
                WidthRequest = 50,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center
            };

            loadingLabel = new Label
            {
                Text = "Loading Code Editor...",
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 14
            };

            loadingContent.Children.Add(loadingIndicator);
            loadingContent.Children.Add(loadingLabel);
            loadingOverlay.Children.Add(loadingContent);

            // Add overlay on top
            containerGrid.Children.Add(loadingOverlay);

            return containerGrid;
        }

        public static void ShowLoadingOverlay(Grid loadingOverlay, ActivityIndicator loadingIndicator, Label loadingLabel, string message = "Loading...")
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                loadingLabel.Text = message;
                loadingLabel.TextColor = Colors.White;
                loadingIndicator.IsRunning = true;
                loadingOverlay.IsVisible = true;
            });
        }

        public static void HideLoadingOverlay(Grid loadingOverlay, ActivityIndicator loadingIndicator)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                loadingOverlay.IsVisible = false;
                loadingIndicator.IsRunning = false;
            });
        }

        public static void UpdateLoadingProgress(Label loadingLabel, int current, int total)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                loadingLabel.Text = $"Initializing editor... ({current}/{total})";
            });
        }

        public static void ShowLoadingError(Grid loadingOverlay, Label loadingLabel, string error = "Failed to load editor")
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                loadingLabel.Text = error;
                loadingLabel.TextColor = Colors.Red;
            });
        }
    }
}
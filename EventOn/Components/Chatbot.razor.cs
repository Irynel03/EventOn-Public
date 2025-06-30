using EventOn.BusinessLogic.Helpers;
using EventOn.BusinessLogic.Models;
using EventOn.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Controls;

namespace EventOn.Components;

public partial class Chatbot
{
    [Inject]
    private NavigationManager Navigation { get; set; }
    [Inject]
    private IEventOnApiService _eventOnApiService { get; set; }

    private Toast toast;
    private static List<ChatMessage> _conversation = [];
    private static string _userMessage = string.Empty;
    private bool _isExpanded = false;
    private bool _canSendMessage = true;

    private void ToggleChatbot()
    {
        _isExpanded = !_isExpanded;
    }

    private async Task SendMessage()
    {
        if (!string.IsNullOrWhiteSpace(_userMessage) && _canSendMessage)
        {
            _canSendMessage = false;
            _conversation.Add(new ChatMessage { Sender = "User", Message = _userMessage });

            var convertedConversation = ConvertChatMessageList(_conversation);

            var chatbotResponseResult = await _eventOnApiService.GetChatbotResponse(UserData.Instance.Username, convertedConversation);
            if (chatbotResponseResult.HasErrors)
            {
                toast.ShowToast(chatbotResponseResult.ErrorMessages);
                return;
            }

            var chatbotMessage = ParseChatbotMessage(chatbotResponseResult.Data);
            _conversation.Add(chatbotMessage);

            _canSendMessage = true;
            _userMessage = string.Empty;
        }
    }

    private static ChatMessage ParseChatbotMessage(string response)
    {
        var message = response.Trim();
        string? eventId = null;

        var parts = message.Split("Id:", StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1)
        {
            message = parts[0].Trim();
            eventId = parts[1].Trim();
        }

        return new ChatMessage { Sender = "Gemini", Message = message, EventId = eventId };
    }

    private static List<string> ConvertChatMessageList(List<ChatMessage> conversation)
    {
        return [.. conversation.Select(message => message.Message)];
    }

    private void NavigateToEvent(string eventId)
    {
        Navigation.NavigateTo($"/event/{eventId}");
    }
}
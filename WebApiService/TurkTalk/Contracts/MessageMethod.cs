﻿using OLabWebAPI.TurkTalk.Contracts;
using System.Text.Json;

namespace OLabWebAPI.Services.TurkTalk.Contracts
{
  /// <summary>
  /// Defines a Atrium Update command method
  /// </summary>
  public class MessageMethod : Method
  {
    public string Data { get; set; }
    public string SessionId { get; set; }
    public string From { get; set; }

    // message for specific group
    public MessageMethod(MessagePayload payload) : base(payload.Envelope.To, "message")
    {
      Data = payload.Data;
      SessionId = payload.SessionId;
      From = payload.Envelope.From.UserId;
    }

  }
}
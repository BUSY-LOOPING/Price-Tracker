﻿namespace PriceTracker.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string toEmail, string subject, string body);
    }
}
